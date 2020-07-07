using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;
using Accounts.Api.BackgroundWorkers;
using Accounts.Api.Dto;
using Accounts.Api.HealthChecks;
using Accounts.Api.MvcFilters;
using Accounts.Api.OpenApiFilters;
using Accounts.Application.Common;
using Accounts.Application.Handlers;
using Accounts.Domain.Commands;
using Accounts.Domain.Common;
using Accounts.Domain.Events;
using Accounts.Infrastructure;
using Accounts.ReadModel;
using EventStore.ClientAPI;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;

namespace Accounts.Api
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var sqlServerConnectionString = _config.GetConnectionString("SqlServer");
            var eventStoreConnectionString = _config.GetConnectionString("EventStore");

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Accounts",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Accounts Team"
                    }
                });
                options.SchemaFilter<FluentValidationRules>();
                options.SchemaFilter<IgnoreReadOnlySchemaFilter>();
                options.OperationFilter<FluentValidationOperationFilter>();
            });

            services.AddControllers(options =>
                {
                    options.Filters.Add(new ExceptionFilter());
                    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(string), (int)HttpStatusCode.BadRequest));
                    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(string), (int)HttpStatusCode.NotFound));
                    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(string), (int)HttpStatusCode.InternalServerError));
                })
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
                .AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()))
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<OpenAccountDto.Validator>());

            services.AddHealthChecks()
                .AddSqlServer(name: "SQL Connection", connectionString: sqlServerConnectionString);

            services.AddSingleton(_ =>
            {
                var connection = EventStoreConnection.Create(eventStoreConnectionString);
                connection.ConnectAsync().GetAwaiter().GetResult();
                return connection;
            });

            services.AddSingleton(_ =>
            {
                var eventTypeInfo = typeof(AccountOpened).GetTypeInfo();
                return new EventStoreSerializer(eventTypeInfo.Assembly, eventTypeInfo.Namespace);
            });

            services.AddScoped<IEventStore, Infrastructure.EventStore>();

            services.AddScoped<MessageBus>();

            services.AddScoped<MessageContextProvider>();
            services.AddScoped(provider => provider.GetRequiredService<MessageContextProvider>().Context);

            services.AddScoped(typeof(IEventSourcedRepository<>), typeof(EventSourcedRepository<>));

            services.AddScoped<IHandler<OpenAccount>, AccountCommandHandlers>();
            services.AddScoped<IHandler<DepositToAccount>, AccountCommandHandlers>();
            services.AddScoped<IHandler<WithdrawFromAccount>, AccountCommandHandlers>();
            services.AddScoped<IHandler<AddInterestsToAccount>, AccountCommandHandlers>();
            services.AddScoped<IHandler<FreezeAccount>, AccountCommandHandlers>();
            services.AddScoped<IHandler<UnfreezeAccount>, AccountCommandHandlers>();

            services.AddDbContext<AccountDbContext>(
                optionsBuilder => optionsBuilder.UseSqlServer(sqlServerConnectionString),
                optionsLifetime: ServiceLifetime.Singleton);

            services.AddScoped<IAccountReadModel, AccountDbContext>();

            services.AddScoped<AccountView>();

            services.AddHostedService<AccountViewSynchronizer>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = HealthCheckResponseWriter.WriteAsync
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(routeBuilder => routeBuilder.MapControllers());

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Accounts"));
        }
    }
}
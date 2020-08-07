using System.Net;
using System.Text.Json.Serialization;
using Accounts.Api.Dto;
using Accounts.Api.HealthChecks;
using Accounts.Api.MvcFilters;
using Accounts.Api.OpenApiFilters;
using Accounts.Application.Common;
using Accounts.Application.Handlers;
using Accounts.Domain.Commands;
using Accounts.Domain.Common;
using Accounts.Infrastructure;
using Accounts.ReadModel;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;

namespace Accounts.Api
{
    public sealed class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
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
                .AddPrivateMemoryHealthCheck(
                    name: "Private memory",
                    maximumMemoryBytes: 1_000_000_000,
                    tags: new[] { HealthCheckTag.Liveness })
                .AddCheck<EventStoreConnectionHealthCheck>(
                    name: "Event Store",
                    tags: new[] { HealthCheckTag.Readiness })
                .AddDbContextCheck<AccountDbContext>(
                    name: "SQL Server",
                    tags: new[] { HealthCheckTag.Readiness });

            services.AddSingleton<EventStoreConnectionHealthCheck>();

            services.AddSingleton(provider =>
            {
                var connectionString = _config.GetConnectionString("EventStore");
                var connection = EventStoreConnection.Create(connectionString);
                var healthCheck = provider.GetRequiredService<EventStoreConnectionHealthCheck>();
                
                connection.Connected += (sender, args) => healthCheck.IsConnected = true;
                connection.Disconnected += (sender, args) => healthCheck.IsConnected = false;
                connection.Closed += (sender, args) => healthCheck.IsConnected = false;
                connection.AuthenticationFailed += (sender, args) => throw new NotAuthenticatedException(args.Reason);
                connection.ErrorOccurred += (sender, args) => throw args.Exception;

                return connection;
            });

            services.AddDbContextPool<AccountDbContext>(optionsBuilder =>
            {
                var connectionString = _config.GetConnectionString("SqlServer");
                optionsBuilder.UseSqlServer(connectionString);
            });

            services.AddScoped<IAccountReadModel, AccountDbContext>();

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
        }

        public void Configure(IApplicationBuilder app, IEventStoreConnection eventStoreConnection)
        {
            eventStoreConnection.ConnectAsync().GetAwaiter().GetResult();

            app.UseHttpsRedirection();
            
            app.UseHealthChecks();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(routeBuilder => routeBuilder.MapControllers());

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Accounts"));
        }
    }
}
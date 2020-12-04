using System.Net;
using System.Text.Json.Serialization;
using Accounting.Common;
using Accounting.Common.HealthChecks;
using Accounts.Api.Dto;
using Accounts.Api.MvcFilters;
using Accounts.Application.Common;
using Accounts.Application.Handlers;
using Accounts.Domain;
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
            services.BindOptions<EventStoreSettings>(_config);
            services.BindOptions<SqlSettings>(_config);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(
                    name: "v1",
                    info: new()
                    {
                        Title = "Accounts",
                        Version = "v1",
                        Contact = new()
                        {
                            Name = "Accounts Team"
                        }
                    });
                options.SchemaFilter<FluentValidationRules>();
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
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<OpenAccountDto.Validator>());

            services.AddHealthChecks()
                .AddCheck<EventStoreConnectionHealthCheck>(
                    name: "Event Store",
                    tags: new[] { HealthCheckTag.Readiness })
                .AddDbContextCheck<AccountDbContext>(
                    name: "SQL Server",
                    tags: new[] { HealthCheckTag.Readiness });

            services.AddSingleton<EventStoreConnectionHealthCheck>();

            services.AddSingleton(provider =>
            {
                var eventStoreSettings = provider.GetRequiredService<EventStoreSettings>();
                var connection = EventStoreConnection.Create(eventStoreSettings.ConnectionString);
                var healthCheck = provider.GetRequiredService<EventStoreConnectionHealthCheck>();

                connection.Connected += (_, _) => healthCheck.IsConnected = true;
                connection.Disconnected += (_, _) => healthCheck.IsConnected = false;
                connection.Closed += (_, _) => healthCheck.IsConnected = false;
                connection.AuthenticationFailed += (_, args) => throw new NotAuthenticatedException(args.Reason);
                connection.ErrorOccurred += (_, args) => throw args.Exception;

                return connection;
            });

            services.AddDbContextPool<AccountDbContext>((provider, optionsBuilder) =>
            {
                var sqlSettings = provider.GetRequiredService<SqlSettings>();
                optionsBuilder.UseSqlServer(sqlSettings.ConnectionString);
            });

            services.AddScoped<IAccountReadModel, AccountDbContext>();

            services.AddScoped<IEventStore, Infrastructure.EventStore>();

            services.AddScoped<Mediator>();

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
            eventStoreConnection.ConnectAsync().Wait();

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
using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;
using Accounts.Api.BackgroundWorkers;
using Accounts.Api.Filters;
using Accounts.Api.HealthChecks;
using Accounts.Application.Commands;
using Accounts.Application.Handlers;
using Accounts.Domain.Events;
using Accounts.ReadModel;
using EventStore.ClientAPI;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Domain;
using MediatR;
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

            services
                .AddControllers(options =>
                {
                    options.Filters.Add(new ExceptionFilter());
                    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(string), (int)HttpStatusCode.BadRequest));
                    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(string), (int)HttpStatusCode.InternalServerError));
                })
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
                .AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()))
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<OpenAccount.Validator>());

            services
                .AddHealthChecks()
                .AddSqlServer(name: "Sql Connection", connectionString: sqlServerConnectionString);

            services
                .AddDbContext<AccountDbContext>(
                    optionsBuilder => optionsBuilder.UseSqlServer(sqlServerConnectionString),
                    optionsLifetime: ServiceLifetime.Singleton)
                .AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
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
                })
                .AddMediatR(typeof(AccountsCommandHandler).GetTypeInfo().Assembly)
                .AddSingleton<EventSourcedAggregateFactory>()
                .AddSingleton(_ =>
                {
                    var connection = EventStoreConnection.Create(eventStoreConnectionString);
                    connection.ConnectAsync().GetAwaiter().GetResult();
                    return connection;
                })
                .AddSingleton(_ =>
                {
                    var eventTypeInfo = typeof(AccountOpened).GetTypeInfo();
                    return new EventStoreSerializer(eventTypeInfo.Assembly, eventTypeInfo.Namespace);
                })
                .AddSingleton(typeof(IEventSourcedRepository<>), typeof(EventStoreRepository<>))
                .AddSingleton<AccountReadModelGenerator>()
                .AddScoped<IAccountReadModel, AccountDbContext>()
                .AddHostedService<ReadModelSynchronizer>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app
                .UseHttpsRedirection()
                .UseHealthChecks(
                    "/health",
                    new HealthCheckOptions
                    {
                        Predicate = _ => true,
                        ResponseWriter = HealthCheckResponseWriter.WriteAsync
                    })
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(routeBuilder => routeBuilder.MapControllers())
                .UseSwagger()
                .UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Accounts"));
        }
    }
}
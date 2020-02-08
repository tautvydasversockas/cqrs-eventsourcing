using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;
using Accounts.Api.BackgroundWorkers;
using Accounts.Api.Filters;
using Accounts.Application.Commands;
using Accounts.Application.Handlers;
using Accounts.Domain.Events;
using Accounts.ReadModel;
using EventStore.ClientAPI;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
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
                .AddControllers(opt =>
                {
                    opt.Filters.Add(new ExceptionFilter());
                    opt.Filters.Add(new ProducesResponseTypeAttribute(typeof(string), (int)HttpStatusCode.BadRequest));
                    opt.Filters.Add(new ProducesResponseTypeAttribute(typeof(string), (int)HttpStatusCode.InternalServerError));
                })
                .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<OpenAccount.Validator>())
                .AddNewtonsoftJson(opt => opt.SerializerSettings.Converters.Add(new StringEnumConverter()));

            services
                .AddHealthChecks()
                .AddSqlServer(name: "Sql Connection", connectionString: sqlServerConnectionString);

            services
                .AddDbContext<AccountDbContext>(
                    options => options.UseSqlServer(sqlServerConnectionString),
                    optionsLifetime: ServiceLifetime.Singleton)
                .AddSwaggerGen(opt =>
                {
                    opt.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Accounts",
                        Version = "v1",
                        Contact = new OpenApiContact
                        {
                            Name = "Accounts Team"
                        }
                    });

                    opt.SchemaFilter<FluentValidationRules>();
                    opt.SchemaFilter<IgnoreReadOnlySchemaFilter>();
                    opt.OperationFilter<FluentValidationOperationFilter>();
                })
                .AddMediatR(typeof(AccountsCommandHandler).GetTypeInfo().Assembly)
                .AddSingleton<EventSourcedAggregateFactory>()
                .AddSingleton(provider =>
                {
                    var connection = EventStoreConnection.Create(eventStoreConnectionString);
                    connection.ConnectAsync().GetAwaiter().GetResult();
                    return connection;
                })
                .AddSingleton(_ =>
                {
                    var typeInfo = typeof(AccountOpened).GetTypeInfo();
                    return new EventStoreSerializer(typeInfo.Assembly, typeInfo.Namespace);
                })
                .AddSingleton(typeof(IEventSourcedRepository<,>), typeof(EventStoreRepository<,>))
                .AddSingleton<AccountReadModelGenerator>()
                .AddScoped<IAccountReadModel, AccountDbContext>()
                .AddHostedService<ReadModelSynchronizer>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app
                .UseHttpsRedirection()
                .UseHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                })
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints => endpoints.MapControllers())
                .UseSwagger()
                .UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Accounts"));
        }
    }
}
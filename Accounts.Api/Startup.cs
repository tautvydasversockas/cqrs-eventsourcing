using Accounts.Api.Dto;
using Accounts.Api.MvcFilters;
using Accounts.Application.Handlers;
using Accounts.Domain;
using Accounts.Domain.Common;
using Accounts.Infrastructure;
using Accounts.Infrastructure.HealthChecks;
using Accounts.ReadModel;
using EventStore.Client;
using FluentValidation.AspNetCore;
using MediatR;
using MicroElements.Swashbuckle.FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Net;
using System.Text.Json.Serialization;

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
                options.SwaggerDoc(
                    name: "v1",
                    info: new OpenApiInfo
                    {
                        Title = "Accounts",
                        Version = "v1",
                        Contact = new OpenApiContact
                        {
                            Name = "Accounts Team"
                        }
                    });
                options.SchemaFilter<FluentValidationRules>();
                options.OperationFilter<FluentValidationOperationFilter>();
            });

            services
                .AddControllers(options =>
                {
                    options.Filters.Add(new ExceptionFilter());
                    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(string), (int)HttpStatusCode.BadRequest));
                    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(string), (int)HttpStatusCode.Conflict));
                    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(string), (int)HttpStatusCode.NotFound));
                    options.Filters.Add(new ProducesResponseTypeAttribute(typeof(string), (int)HttpStatusCode.InternalServerError));
                })
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<OpenAccountDto.Validator>());

            services
                .AddHealthChecks()
                .AddCheck<EventStoreHealthCheck>(
                    name: "Event Store",
                    tags: new[] { HealthCheckTag.Readiness })
                .AddDbContextCheck<AccountDbContext>(
                    name: "SQL Server",
                    tags: new[] { HealthCheckTag.Readiness });

            services.AddMediatR(typeof(AccountCommandHandlers));

            services.AddEventStoreClient(settings =>
            {
                settings.ConnectionName = "Accounts.Api";
                settings.DefaultCredentials = new UserCredentials(_config["EventStore:Username"], _config["EventStore:Password"]);
                settings.ConnectivitySettings.Address = new Uri(_config["EventStore:Address"]);
            });

            services.AddDbContextPool<AccountDbContext>(optionsBuilder =>
                optionsBuilder.UseSqlServer(_config["Sql:ConnectionString"]));

            services.AddScoped<IAccountReadModel, AccountDbContext>();

            services.AddScoped(typeof(IEventSourcedRepository<,>), typeof(EventSourcedRepository<,>));

            services.AddScoped<IAccountRepository, AccountRepository>();
        }

        public void Configure(IApplicationBuilder app)
        {
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
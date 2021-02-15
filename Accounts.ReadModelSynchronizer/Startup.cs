using Accounts.Infrastructure.HealthChecks;
using Accounts.ReadModel;
using EventStore.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Accounts.ReadModelSynchronizer
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
            services
                .AddHealthChecks()
                .AddCheck(
                    instance: new BackgroundServiceHealthCheck(TimeSpan.FromMinutes(5)),
                    name: "Processing",
                    tags: new[] { HealthCheckTag.Liveness });

            services.AddEventStorePersistentSubscriptionsClient(settings =>
            {
                settings.ConnectionName = "Accounts.ReadModelSynchronizer";
                settings.DefaultCredentials = new UserCredentials(_config["EventStore:Username"], _config["EventStore:Password"]);
                settings.ConnectivitySettings.Address = new Uri(_config["EventStore:Address"]);
            });

            services.AddDbContextPool<AccountDbContext>(optionsBuilder =>
                optionsBuilder.UseSqlServer(_config["Sql:ConnectionString"]));

            services.AddScoped<AccountView>();

            services.AddHostedService<App>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseHealthChecks();

            app.UseRouting();
        }
    }
}
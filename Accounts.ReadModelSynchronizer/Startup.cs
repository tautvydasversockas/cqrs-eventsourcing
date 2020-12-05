using System;
using Accounting.Common;
using Accounts.Infrastructure.HealthChecks;
using Accounts.ReadModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddHealthChecks()
                .AddCheck<BackgroundServiceHealthCheck>(
                    name: "Processing",
                    tags: new[] { HealthCheckTag.Liveness });

            services.AddSingleton(new BackgroundServiceHealthCheck(
                timeout: TimeSpan.FromMinutes(5)));

            services.AddEventStorePersistentSubscriptionsClient(settings =>
            {
                settings.ConnectionName = "Accounts.ReadModelSynchronizer";
                settings.DefaultCredentials = new(
                    username: _config.GetValue<string>("EventStore:Username"),
                    password: _config.GetValue<string>("EventStore:Password"));
                settings.ConnectivitySettings.Address = new(_config.GetValue<string>("EventStore:Address"));
            });

            services.AddDbContextPool<AccountDbContext>((provider, optionsBuilder) =>
                optionsBuilder.UseSqlServer(_config.GetValue<string>("Sql:ConnectionString")));

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
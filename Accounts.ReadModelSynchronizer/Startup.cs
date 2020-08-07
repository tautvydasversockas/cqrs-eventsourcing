using System;
using Accounts.Api.HealthChecks;
using Accounts.ReadModel;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
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
                .AddPrivateMemoryHealthCheck(
                    name: "Private memory",
                    maximumMemoryBytes: 1_000_000_000,
                    tags: new[] { HealthCheckTag.Liveness })
                .AddCheck<BackgroundServiceHealthCheck>(
                    name: "Processing",
                    tags: new[] { HealthCheckTag.Liveness });

            services.AddSingleton(new BackgroundServiceHealthCheck(
                timeBetweenProcess: TimeSpan.FromSeconds(90)));

            services.AddSingleton(provider =>
            {
                var connectionString = _config.GetConnectionString("EventStore");
                var connection = EventStoreConnection.Create(connectionString);

                connection.AuthenticationFailed += (sender, args) => throw new NotAuthenticatedException(args.Reason);
                connection.ErrorOccurred += (sender, args) => throw args.Exception;

                return connection;
            });

            services.AddDbContextPool<AccountDbContext>(optionsBuilder =>
            {
                var connectionString = _config.GetConnectionString("SqlServer");
                optionsBuilder.UseSqlServer(connectionString);
            });

            services.AddScoped<AccountView>();

            services.AddHostedService<AccountViewSynchronizer>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseHealthChecks();

            app.UseRouting();
        }
    }
}
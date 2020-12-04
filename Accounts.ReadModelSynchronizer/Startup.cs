using System;
using Accounting.Common;
using Accounting.Common.HealthChecks;
using Accounts.Infrastructure;
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
            services.BindOptions<EventStoreSettings>(_config);
            services.BindOptions<SqlSettings>(_config);

            services.AddHealthChecks()
                .AddCheck<BackgroundServiceHealthCheck>(
                    name: "Processing",
                    tags: new[] { HealthCheckTag.Liveness });

            services.AddSingleton(new BackgroundServiceHealthCheck(
                timeout: TimeSpan.FromMinutes(5)));

            services.AddSingleton(provider =>
            {
                var eventStoreSettings = provider.GetRequiredService<EventStoreSettings>();
                var connection = EventStoreConnection.Create(eventStoreSettings.ConnectionString);

                connection.AuthenticationFailed += (_, args) => throw new NotAuthenticatedException(args.Reason);
                connection.ErrorOccurred += (_, args) => throw args.Exception;

                return connection;
            });

            services.AddDbContextPool<AccountDbContext>((provider, optionsBuilder) =>
            {
                var sqlSettings = provider.GetRequiredService<SqlSettings>();
                optionsBuilder.UseSqlServer(sqlSettings.ConnectionString);
            });

            services.AddScoped<AccountView>();

            services.AddHostedService<App>();
        }

        public void Configure(IApplicationBuilder app, IEventStoreConnection eventStoreConnection)
        {
            eventStoreConnection.ConnectAsync().Wait();

            app.UseHttpsRedirection();

            app.UseHealthChecks();

            app.UseRouting();
        }
    }
}
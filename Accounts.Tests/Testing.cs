using Accounts.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NUnit.Framework;

namespace Accounts.Tests
{
    [SetUpFixture]
    public sealed class Testing
    {
        private static readonly ServiceCollection Services = new();

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .AddEnvironmentVariables()
                .Build();
            var startup = new Startup(config);
            startup.ConfigureServices(Services);
        }

        public static ServiceCollection GetServices()
        {
            var services = new ServiceCollection();

            foreach (var service in Services)
                services.Add(service);

            return services;
        }
    }
}
using System;
using System.Diagnostics.CodeAnalysis;
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
        [AllowNull] private static IServiceCollection _services;

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var config = new ConfigurationBuilder().Build();
            var startup = new Startup(config);
            _services = new ServiceCollection();
            startup.ConfigureServices(_services);
        }

        public static IServiceProvider GetServiceProvider(Action<IServiceCollection> setup)
        {
            var services = new ServiceCollection();

            foreach (var service in _services)
                services.Add(service);

            setup(services);

            return services.BuildServiceProvider();
        }
    }
}
namespace Accounts.Tests;

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
        Services.SetupServices(config);
    }

    public static ServiceCollection GetServices()
    {
        var services = new ServiceCollection();

        foreach (var service in Services)
            services.Add(service);

        return services;
    }
}

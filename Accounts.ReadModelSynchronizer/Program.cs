var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(opt =>
{
    opt.ValidateOnBuild = true;
    opt.ValidateScopes = true;
});

var mySqlConnectionString = builder.Configuration["MySql:ConnectionString"];

builder.Services
    .AddHealthChecks()
    .AddCheck<BackgroundServiceHealthCheck>(
        name: "Processing",
        tags: new[] { HealthCheckTag.Liveness })
    .AddMySql(
        name: "MySql",
        connectionString: mySqlConnectionString,
        tags: new[] { HealthCheckTag.Readiness });

builder.Services.AddEventStorePersistentSubscriptionsClient(settings =>
{
    settings.ConnectionName = "Accounts.ReadModelSynchronizer";

    settings.DefaultCredentials = new UserCredentials(
        builder.Configuration["EventStore:Username"],
        builder.Configuration["EventStore:Password"]);

    settings.ConnectivitySettings.Address = new Uri(
        builder.Configuration["EventStore:Address"]);
});

builder.Services.AddSingleton(new MySqlConnectionFactory(mySqlConnectionString));

builder.Services.AddSingleton<AccountView>();

builder.Services.AddSingleton<AccountReadModel>();

builder.Services.AddHostedService<App>();

await using var app = builder.Build();

app.UseHttpsRedirection();

app.UseHealthChecks();

app.UseRouting();

await app.RunAsync();
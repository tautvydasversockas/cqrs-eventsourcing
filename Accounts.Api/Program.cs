var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(opt =>
{
    opt.ValidateOnBuild = true;
    opt.ValidateScopes = true;
});

builder.Services.SetupServices(builder.Configuration);

await using var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

app.UseHealthChecks();

app.UseRouting();

app.UseEndpoints(routeBuilder => routeBuilder.MapControllers());

app.UseOpenApi();

await app.RunAsync();

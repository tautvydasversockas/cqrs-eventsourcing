namespace Accounts.Api
{
    public static class ServiceConfigurator
    {
        public static void SetupServices(this IServiceCollection services, IConfiguration config)
        {
            var mySqlConnectionString = config["MySql:ConnectionString"];

            services
                .AddFluentMigratorCore()
                .ConfigureRunner(builder => builder
                    .AddMySql5()
                    .WithGlobalConnectionString(mySqlConnectionString)
                    .WithGlobalCommandTimeout(TimeSpan.FromMinutes(5))
                    .ScanIn(typeof(Create_Accounts_table).Assembly)
                    .For
                    .Migrations())
                .AddLogging(builder => builder
                    .AddFluentMigratorConsole());

            services.AddOpenApi();

            services
                .AddControllers(opt =>
                {
                    opt.Filters.Add(new ProducesResponseTypeAttribute(typeof(string), StatusCodes.Status400BadRequest));
                    opt.Filters.Add(new ProducesResponseTypeAttribute(typeof(string), StatusCodes.Status409Conflict));
                    opt.Filters.Add(new ProducesResponseTypeAttribute(typeof(string), StatusCodes.Status404NotFound));
                    opt.Filters.Add(new ProducesResponseTypeAttribute(typeof(string), StatusCodes.Status500InternalServerError));
                })
                .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
                .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<OpenAccountDto.Validator>());

            services
                .AddHealthChecks()
                .AddCheck<EventStoreHealthCheck>(
                    name: "Event Store",
                    tags: new[] { HealthCheckTag.Readiness })
                .AddMySql(
                    name: "MySql",
                    connectionString: mySqlConnectionString,
                    tags: new[] { HealthCheckTag.Readiness });

            services.AddMediatR(typeof(AccountCommandHandlers));

            services.AddEventStoreClient(settings =>
            {
                settings.ConnectionName = "Accounts.Api";

                settings.DefaultCredentials = new UserCredentials(
                    config["EventStore:Username"], 
                    config["EventStore:Password"]);

                settings.ConnectivitySettings.Address = new Uri(
                    config["EventStore:Address"]);
            });

            services.AddSingleton(new MySqlConnectionFactory(mySqlConnectionString));
            
            services.AddSingleton<AccountReadModel>();

            services.AddSingleton(typeof(IEventSourcedRepository<,>), typeof(EventSourcedRepository<,>));

            services.AddSingleton<IAccountRepository, AccountRepository>();
        }
    }
}


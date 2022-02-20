namespace Accounts.Api.OpenApi
{
    public static class OpenApiExtensions
    {
        public static IServiceCollection AddOpenApi(this IServiceCollection services)
        {
            return services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc(
                    name: "v1",
                    info: new OpenApiInfo
                    {
                        Title = "Accounts",
                        Version = "v1",
                        Contact = new OpenApiContact
                        {
                            Name = "Accounts Team"
                        }
                    });
                opt.SchemaFilter<FluentValidationRules>();
                opt.OperationFilter<FluentValidationOperationFilter>();
            });
        }

        public static IApplicationBuilder UseOpenApi(this IApplicationBuilder app)
        {
            return app
                .UseSwagger()
                .UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Accounts"));
        }
    }
}

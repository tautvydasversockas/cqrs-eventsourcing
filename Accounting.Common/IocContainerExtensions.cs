using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Accounting.Common
{
    public static class IocContainerExtensions
    {
        public static void BindOptions<TOptions>(this IServiceCollection services, IConfiguration rootConfig)
            where TOptions : class, new()
        {
            var optionsSectionName = typeof(TOptions).Name
                .RemoveFromEnd("settings")
                .RemoveFromEnd("options")
                .RemoveFromEnd("config");

            var optionsSection = rootConfig.GetSection(optionsSectionName);

            services.AddOptions<TOptions>()
               .Bind(optionsSection)
               .ValidateDataAnnotations();

            services.AddSingleton(provider => provider.GetRequiredService<IOptions<TOptions>>().Value);
        }
    }
}
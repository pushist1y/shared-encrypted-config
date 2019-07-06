using System.IO;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SharedProtectedConfigurationLib
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureProtected<TOptions>(this IServiceCollection services, IConfigurationSection section) where TOptions : class, new()
        {
            return services.AddSingleton(provider =>
            {
                var dataProtectionProvider = provider.GetRequiredService<IDataProtectionProvider>();
                section = new ProtectedConfigurationSection(dataProtectionProvider, section);

                var options = section.Get<TOptions>();
                return Options.Create(options);
            });
        }
    }
}

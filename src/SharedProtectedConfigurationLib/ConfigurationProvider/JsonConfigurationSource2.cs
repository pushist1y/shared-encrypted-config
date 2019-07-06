using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace SharedProtectedConfigurationLib
{
    public class JsonConfigurationSource2 : JsonConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new JsonConfigurationProvider2(this);
        }
    }
}
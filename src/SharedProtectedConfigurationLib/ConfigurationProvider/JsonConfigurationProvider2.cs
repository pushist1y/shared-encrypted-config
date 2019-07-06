using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration.Json;

namespace SharedProtectedConfigurationLib
{
    public class JsonConfigurationProvider2 : JsonConfigurationProvider
    {
        private readonly IDataProtectionProvider _dpProvider;
        public JsonConfigurationProvider2(JsonConfigurationSource source) : base(source)
        {
            _dpProvider = DataProtectionProvider.Create("");
        }

        public override void Load(Stream stream)
        {
            base.Load(stream);

            var unprotectedKeys = Data.Where(kv => kv.Key.StartsWith("Unprotected:"))
                .Select(kv => new {Key = kv.Key.Replace("Unprotected:", ""), kv.Value});

            var protectedKeys = Data.Where(kv => kv.Key.StartsWith("Protected:"))
                .Select(kv => new {Key = kv.Key.Replace("Protected:", ""), kv.Value});

            var protector = _dpProvider.CreateProtector(Constants.ProtectionPurpose);

            var dict = new Dictionary<string, string>();
            foreach (var unprotectedKey in unprotectedKeys)
            {
                dict[unprotectedKey.Key] = unprotectedKey.Value;
            }

            foreach (var protectedKey in protectedKeys)
            {
                dict[protectedKey.Key] = protector.Unprotect(protectedKey.Value);
            }

            Data = dict;
        }
    }
}
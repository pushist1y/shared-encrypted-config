using System;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SharedProtectedConfigurationLib
{
    public class JsonFileProtector
    {
        private readonly Lazy<IDataProtector> _protector;
        public JsonFileProtector(IDataProtectionProvider dataProtectionProvider)
        {
            _protector = new Lazy<IDataProtector>(() => dataProtectionProvider.CreateProtector(Constants.ProtectionPurpose));
        }

        public void ProtectFile(string jsonFilePath)
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(jsonFilePath));
            var protectedObject = new JObject();

            ProtectOrUnprotectNode(jObject, protectedObject, ProtectionMode.Protect);

            File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(protectedObject, Formatting.Indented));
        }

        public void UnprotectFile(string jsonFilePath)
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(jsonFilePath));
            var unprotectedObject = new JObject();

            ProtectOrUnprotectNode(jObject, unprotectedObject, ProtectionMode.Unprotect);

            File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(unprotectedObject, Formatting.Indented));
        }

        private string ProtectOrUnprotectValue(string value, ProtectionMode mode)
        {
            if (mode == ProtectionMode.Protect)
            {
                try
                {
                    var temp = _protector.Value.Unprotect(value);

                    //value already protected
                    return value;
                }
                catch
                {
                    // ignore
                }

                return _protector.Value.Protect(value);
            }
            else
            {
                return _protector.Value.Unprotect(value);
            }
        }

        private enum ProtectionMode
        {
            Protect = 0,
            Unprotect = 1
        }

        private void ProtectOrUnprotectNode(JObject nodeSource, JObject nodeDest, ProtectionMode mode)
        {
            foreach (var jProp in nodeSource.Children<JProperty>())
            {
                if (jProp.Value.Type == JTokenType.Object)
                {
                    var childObj = jProp.Value as JObject;
                    if (!nodeDest.TryGetValue(jProp.Name, out var destProp))
                    {
                        nodeDest.Add(jProp.Name, new JObject());
                        destProp = nodeDest.GetValue(jProp.Name);
                    }
                    ProtectOrUnprotectNode(childObj, destProp as JObject, mode);
                }
                else if (jProp.Value.Type == JTokenType.Array)
                {
                    if (!nodeDest.ContainsKey(jProp.Name))
                    {
                        nodeDest[jProp.Name] = new JArray();
                    }

                    foreach (var jToken in (JArray)jProp.Value)
                    {
                        var strVal = jToken.Value<string>();
                        ((JArray)nodeDest[jProp.Name]).Add(ProtectOrUnprotectValue(strVal, mode));
                    }
                }
                else
                {
                    nodeDest[jProp.Name] = ProtectOrUnprotectValue(jProp.Value.Value<string>(), mode);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Example.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SharedProtectedConfigurationLib;

namespace Example.Bar.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var env = context.HostingEnvironment;

                    var sharedFolder = Path.Combine(env.ContentRootPath, "..", "SharedConfiguration");
                    var sharedConfigName = Constants.SharedConfigName;

                    builder.AddJsonFile("appsettings.json", optional: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                        .AddJsonFile(Path.Combine(sharedFolder, sharedConfigName), true, true)
                        .AddJsonFile(sharedConfigName, true, true)
                        ;

                    builder.AddEnvironmentVariables();
                })
                .UseStartup<Startup>();
    }
}

using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Equinor.Procosys.Preservation.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var kvSettings = new KeyVaultSettings();
                    config.Build().GetSection("KeyVault").Bind(kvSettings);
                    if (kvSettings.Enabled)
                    {
                        config.AddAzureKeyVault(kvSettings.Uri, kvSettings.ClientId, kvSettings.ClientSecret);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options =>
                    {
                        options.AddServerHeader = false;
                        options.Limits.MaxRequestBodySize = null;
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}

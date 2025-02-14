using System;
using Azure.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace Equinor.ProCoSys.Preservation.WebApi.DiModules;

public static class SetupAzureAppConfig
{
    public static void ConfigureAzureAppConfig(this WebApplicationBuilder builder, TokenCredential credential)
    {
        var azConfig = builder.Configuration.GetValue<bool>("Application:UseAzureAppConfiguration");
        if (!azConfig)
        {
            return;
        }

        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            var appConfigUrl = builder.Configuration["Application:AppConfigurationUrl"]!;
            
            options.Connect(new Uri(appConfigUrl), credential)
                .ConfigureKeyVault(kv =>
                {
                    kv.SetCredential(credential);
                })
                .Select(KeyFilter.Any)
                .Select(KeyFilter.Any, builder.Environment.EnvironmentName)
                .ConfigureRefresh(refreshOptions =>
                {
                    refreshOptions.Register("Sentinel", true);
                    refreshOptions.SetRefreshInterval(TimeSpan.FromSeconds(30));
                });
        });
    }
}

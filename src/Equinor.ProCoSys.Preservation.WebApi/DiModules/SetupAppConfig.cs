using System;
using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace Equinor.ProCoSys.Preservation.WebApi.DiModules;

public static class SetupAzureAppConfig
{
    public static void ConfigureAzureAppConfig(this WebApplicationBuilder builder)
    {
        var azConfig = builder.Configuration.GetValue<bool>("Application:UseAzureAppConfiguration");
        if (!azConfig)
        {
            return;
        }

        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            var connectionString = builder.Configuration["ConnectionStrings:AppConfig"];
            options.Connect(connectionString)
                .ConfigureKeyVault(kv =>
                {
                    kv.SetCredential(new ManagedIdentityCredential());
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

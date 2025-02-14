using Azure.Core;
using Azure.Identity;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

namespace Equinor.ProCoSys.Preservation.WebApi.DiModules;

public static class AuthenticationConfig
{
    public static void ConfigureAuthentication(this WebApplicationBuilder builder, out TokenCredential credential)
    {
        var devOnLocalhost = builder.Configuration.IsDevOnLocalhost();

        // ChainedTokenCredential iterates through each credential passed to it in order, when running locally
        // DefaultAzureCredential will probably fail locally, so if an instance of Azure Cli is logged in, those credentials will be used
        // If those credentials fail, the next credentials will be those of the current user logged into the local Visual Studio Instance
        // which is also the most likely case
        credential = devOnLocalhost switch
        {
            true
                => new ChainedTokenCredential(
                    new AzureCliCredential(),
                    new VisualStudioCredential(),
                    new DefaultAzureCredential()
                ),
            false => new DefaultAzureCredential()
        };

        builder.Services.AddSingleton(credential);

        builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration)
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();
    }
}

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
        
        credential = new DefaultAzureCredential(includeInteractiveCredentials:devOnLocalhost);

        builder.Services.AddSingleton(credential);

        builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration)
            .EnableTokenAcquisitionToCallDownstreamApi()
            .AddInMemoryTokenCaches();
    }
}

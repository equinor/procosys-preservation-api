using Microsoft.Extensions.Configuration;

namespace Equinor.ProCoSys.Preservation.WebApi.ServiceBus.Misc;

public static class IsDevOnLocalhostExtension
{
    public static bool IsDevOnLocalhost(this IConfiguration configuration)
        => configuration.GetValue<bool>("Application:DevOnLocalhost");
}

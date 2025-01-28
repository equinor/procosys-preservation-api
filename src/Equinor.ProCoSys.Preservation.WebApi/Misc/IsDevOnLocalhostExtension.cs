using Microsoft.Extensions.Configuration;

namespace Equinor.ProCoSys.Preservation.WebApi.Misc;

public static class IsDevOnLocalhostExtension
{
    public static bool IsDevOnLocalhost(this IConfiguration configuration)
        => configuration.GetValue<bool>("Application:DevOnLocalhost");
}

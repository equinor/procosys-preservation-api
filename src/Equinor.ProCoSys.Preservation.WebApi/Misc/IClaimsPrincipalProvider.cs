using System.Security.Claims;

namespace Equinor.ProCoSys.Preservation.WebApi.Misc
{
    public interface IClaimsPrincipalProvider
    {
        ClaimsPrincipal GetCurrentClaimsPrincipal();
    }
}

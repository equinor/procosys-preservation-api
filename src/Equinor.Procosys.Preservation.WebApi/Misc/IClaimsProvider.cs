using System.Security.Claims;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public interface IClaimsProvider
    {
        ClaimsPrincipal GetCurrentUser();
    }
}

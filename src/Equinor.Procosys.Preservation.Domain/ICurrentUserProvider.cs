using System;
using System.Security.Claims;

namespace Equinor.Procosys.Preservation.Domain
{
    public interface ICurrentUserProvider
    {
        Guid GetCurrentUserOid();
        Guid? TryGetCurrentUserOid();
        bool IsCurrentUserAuthenticated();
        ClaimsPrincipal GetCurrentUser();
    }
}

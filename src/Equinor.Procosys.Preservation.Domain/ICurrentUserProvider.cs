using System;

namespace Equinor.Procosys.Preservation.Domain
{
    public interface ICurrentUserProvider
    {
        Guid GetCurrentUser(); // todo rename to GetCurrentUserOid
        Guid? TryGetCurrentUserOid();
    }
}

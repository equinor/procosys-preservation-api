using System;

namespace Equinor.ProCoSys.Preservation.Domain
{
    public interface ICurrentUserProvider
    {
        Guid GetCurrentUserOid();
    }
}

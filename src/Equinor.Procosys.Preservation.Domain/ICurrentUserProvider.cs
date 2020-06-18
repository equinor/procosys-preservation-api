using System;

namespace Equinor.Procosys.Preservation.Domain
{
    public interface ICurrentUserProvider
    {
        Guid GetCurrentUserOid();
    }
}

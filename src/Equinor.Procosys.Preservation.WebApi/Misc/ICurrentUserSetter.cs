using System;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public interface ICurrentUserSetter
    {
        void SetCurrentUserOid(Guid oid);
    }
}

using System;

namespace Equinor.ProCoSys.Preservation.WebApi.Misc
{
    public interface ICurrentUserSetter
    {
        void SetCurrentUserOid(Guid oid);
    }
}

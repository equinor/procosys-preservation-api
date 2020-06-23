using System;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public interface ICurrentUserSetter
    {
        void SetCurrentUser(Guid oid);
    }
}

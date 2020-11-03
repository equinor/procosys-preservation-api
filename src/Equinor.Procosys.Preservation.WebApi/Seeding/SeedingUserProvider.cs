using System;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.WebApi.Seeding
{
    public class SeedingUserProvider : ICurrentUserProvider
    {
        private readonly Guid _oid;

        public SeedingUserProvider(Guid oid) => _oid = oid;
        
        public Guid GetCurrentUserOid() => _oid;
    }
}

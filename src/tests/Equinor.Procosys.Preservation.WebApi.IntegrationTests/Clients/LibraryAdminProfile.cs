using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Clients
{
    // Authenticated client with necessary roles to Create and Update in Library
    public static class LibraryAdminProfile
    {
        public static Profile Tokens
            => new Profile
            {
                Oid = "00000000-0000-0000-0000-000000000001",
                FullName = "Arne Admin"
            };

        public static IList<string> ProCoSysPermissions
            => new List<string>
            {
                Permissions.LIBRARY_PRESERVATION_CREATE,
                Permissions.LIBRARY_PRESERVATION_DELETE,
                Permissions.LIBRARY_PRESERVATION_READ,
                Permissions.LIBRARY_PRESERVATION_VOIDUNVOID,
                Permissions.LIBRARY_PRESERVATION_WRITE
            };
    }
}

using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Clients
{
    // Authenticated client with necessary roles to Update tags as a Preserver
    public static class PreserverClient
    {
        public static TestTokens Tokens
            => new TestTokens
            {
                Oid = "00000000-0000-0000-0000-000000000003",
                FullName = "Peder Preserver"
            };

        public static IList<string> ProCoSysPermissions
            => new List<string>
            {
                Permissions.LIBRARY_PRESERVATION_READ,
                Permissions.PRESERVATION_CREATE,
                Permissions.PRESERVATION_DELETE,
                Permissions.PRESERVATION_READ,
                Permissions.PRESERVATION_WRITE,
                Permissions.PRESERVATION_ATTACHFILE,
                Permissions.PRESERVATION_DETACHFILE
            };
    }
}

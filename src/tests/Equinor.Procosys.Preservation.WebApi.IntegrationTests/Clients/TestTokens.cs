using System.Collections.Generic;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Clients
{
    public class TestTokens
    {
        public string Oid { get; set; }
        public string FullName { get; set; }
        public bool IsAppToken { get; set; } = false;
        public List<string> Roles { get; set; } = new List<string>();
    }
}

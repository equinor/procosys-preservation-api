using System.Collections.Generic;
using System.Net.Http;
using Equinor.Procosys.Preservation.MainApi.Permission;
using Equinor.Procosys.Preservation.MainApi.Plant;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Clients
{
    public class TestUser : ITestUser
    {
        public TestProfile Profile { get; set; }
        public List<ProcosysPlant> ProCoSysPlants { get; set; }
        public List<ProcosysProject> ProCoSysProjects { get; set; }
        public List<string> ProCoSysPermissions { get; set; }
        public List<string> ProCoSysRestrictions { get; set; }
        public HttpClient HttpClient { get; set; }

        public override string ToString() => Profile?.ToString();
    }
}

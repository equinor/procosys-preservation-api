using System.Collections.Generic;
using System.Net.Http;
using Equinor.ProCoSys.Auth.Permission;
using Equinor.ProCoSys.Auth.Person;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests
{
    public class TestUser : ITestUser
    {
        public TestProfile Profile { get; set; }
        public ProCoSysPerson AuthProCoSysPerson => Profile?.AsAuthProCoSysPerson();
        public List<AccessablePlant> AccessablePlants { get; set; }
        public List<AccessableProject> AccessableProjects { get; set; }
        public List<string> Permissions { get; set; }
        public List<string> Restrictions { get; set; }
        public HttpClient HttpClient { get; set; }

        public override string ToString() => Profile?.ToString();
    }
}

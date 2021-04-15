using System.Collections.Generic;
using System.Net.Http;
using Equinor.ProCoSys.Preservation.MainApi.Permission;
using Equinor.ProCoSys.Preservation.MainApi.Plant;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests
{
    public interface ITestUser
    {
        TestProfile Profile { get; set; }
        List<PCSPlant> ProCoSysPlants { get; set; }
        List<PCSProject> ProCoSysProjects { get; set; }
        List<string> ProCoSysPermissions { get; set; }
        List<string> ProCoSysRestrictions { get; set; }
        HttpClient HttpClient { get; set; }
    }
}

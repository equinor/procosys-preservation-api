using System.Collections.Generic;
using System.Net.Http;
using Equinor.ProCoSys.Preservation.MainApi.Permission;
using Equinor.ProCoSys.Preservation.MainApi.Person;
using Equinor.ProCoSys.Preservation.MainApi.Plant;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests
{
    public interface ITestUser
    {
        TestProfile Profile { get; set; }
        ProCoSysPerson AuthProCoSysPerson { get; }
        List<ProCoSysPlant> ProCoSysPlants { get; set; }
        List<ProCoSysProject> ProCoSysProjects { get; set; }
        List<string> ProCoSysPermissions { get; set; }
        List<string> ProCoSysRestrictions { get; set; }
        HttpClient HttpClient { get; set; }
    }
}

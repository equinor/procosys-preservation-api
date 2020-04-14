using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.WebApi.ProjectAccess
{
    public interface IProjectAccessChecker
    {
        bool HasCurrentUserAccessToProject(string projectName);
    }
}

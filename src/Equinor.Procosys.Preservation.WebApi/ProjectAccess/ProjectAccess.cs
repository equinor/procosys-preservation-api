using System.Linq;
using System.Reflection;
using Equinor.Procosys.Preservation.Domain.ProjectAccess;

namespace Equinor.Procosys.Preservation.WebApi.ProjectAccess
{
    public class ProjectAccess : IProjectAccess
    {
        public ProjectAccessFailure ValidateAccess(object request)
        {
            var customAttributes = request.GetType().GetTypeInfo()?.CustomAttributes;
            var pathToProject = customAttributes?.FirstOrDefault(ca => ca.AttributeType == typeof(PathToProjectAttribute));
            if (pathToProject != null)
            {
                return new ProjectAccessFailure("Test", PathToProjectType.Tag);
            }

            return null;
        }
    }
}

using System;
using System.Reflection;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.ProjectAccess;
using MediatR;

namespace Equinor.Procosys.Preservation.WebApi.ProjectAccess
{
    public class ProjectAccessValidator : IProjectAccessValidator
    {
        private readonly IProjectAccessChecker _projectAccessChecker;
        private readonly IProjectHelper _projectHelper;

        public ProjectAccessValidator(IProjectAccessChecker projectAccessChecker, IProjectHelper projectHelper)
        {
            _projectAccessChecker = projectAccessChecker;
            _projectHelper = projectHelper;
        }

        public async Task<bool> ValidateAsync<TRequest>(TRequest request) where TRequest: IBaseRequest
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var pathToProject = request.GetType().GetCustomAttribute<PathToProjectAttribute>();
            if (pathToProject != null)
            {
                var propertyValue = GetPropertyValue(request, pathToProject.PropertyName);

                string projectName;
                switch (pathToProject.PathToProjectType)
                {
                    case PathToProjectType.ProjectName:
                        projectName = (string)propertyValue;
                        break;
                    case PathToProjectType.TagId:
                        projectName = await _projectHelper.GetProjectNameFromTagIdAsync((int)propertyValue);
                        break;
                    case PathToProjectType.ActionId:
                        projectName = await _projectHelper.GetProjectNameFromActionIdAsync((int)propertyValue);
                        break;
                    case PathToProjectType.RequirementId:
                        projectName = await _projectHelper.GetProjectNameFromRequirementIdAsync((int)propertyValue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (!string.IsNullOrEmpty(projectName))
                {
                    return ValidateAccessToProject(projectName);
                }
            }

            return true;
        }

        private bool ValidateAccessToProject(string projectName)
            => _projectAccessChecker.HasCurrentUserAccessToProject(projectName);

        private object GetPropertyValue(object request, string propertyName)
        {
            var propertyInfo = request.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new Exception($"Property {propertyName} do not exist in class of type {nameof(request.GetType)}");
            }

            return propertyInfo.GetValue(request, null);
        }
    }
}

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

        public async Task<bool> ValidateAsync<TRequest>(TRequest request) where TRequest : IBaseRequest
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // If ProjectAccessCheckAttribute found on request, user need access to Project to allow request execution
            var projectAccessCheck = request.GetType().GetCustomAttribute<ProjectAccessCheckAttribute>();
            if (projectAccessCheck == null)
            {
                return true;
            }

            // ProjectAccessAttribute "describes" how to find ProjectName from current request
            var projectName = projectAccessCheck.PathToProjectType switch
            {
                PathToProjectType.ProjectName
                    => GetPropertyValue<string>(request, projectAccessCheck.PropertyName),

                PathToProjectType.TagId
                    => await _projectHelper.GetProjectNameFromTagIdAsync(
                        GetPropertyValue<int>(request, projectAccessCheck.PropertyName)),

                _ => throw new ArgumentOutOfRangeException()
            };

            return _projectAccessChecker.HasCurrentUserAccessToProject(projectName);
        }

        private T GetPropertyValue<T>(IBaseRequest request, string propertyName)
        {
            var propertyInfo = request.GetType().GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new Exception($"Property {propertyName} do not exist in class of type {nameof(request.GetType)}");
            }

            return (T)propertyInfo.GetValue(request, null);
        }
    }
}

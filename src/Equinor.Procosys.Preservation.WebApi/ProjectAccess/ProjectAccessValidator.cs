using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
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

            if (request is IProjectRequest projectRequest)
            {
                return HasCurrentUserAccessToProject(projectRequest);
            }

            if (request is ITagRequest tagRequest)
            {
                return await HasCurrentUserAccessToProjectAsync(tagRequest);
            }

            return true;
        }

        private async Task<bool> HasCurrentUserAccessToProjectAsync(ITagRequest tagRequest)
        {
            var projectName = await _projectHelper.GetProjectNameFromTagIdAsync(tagRequest.TagId);
            return _projectAccessChecker.HasCurrentUserAccessToProject(projectName);
        }

        private bool HasCurrentUserAccessToProject(IProjectRequest projectRequest)
            => _projectAccessChecker.HasCurrentUserAccessToProject(projectRequest.ProjectName);
    }
}

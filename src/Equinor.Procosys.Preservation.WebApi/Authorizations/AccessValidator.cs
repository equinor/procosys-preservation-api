using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query;
using Equinor.Procosys.Preservation.WebApi.Misc;
using MediatR;

namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public class AccessValidator : IAccessValidator
    {
        private readonly IProjectAccessChecker _projectAccessChecker;
        private readonly IContentRestrictionsChecker _contentRestrictionsChecker;
        private readonly ITagHelper _tagHelper;

        public AccessValidator(
            IProjectAccessChecker projectAccessChecker,
            IContentRestrictionsChecker contentRestrictionsChecker,
            ITagHelper tagHelper)
        {
            _projectAccessChecker = projectAccessChecker;
            _contentRestrictionsChecker = contentRestrictionsChecker;
            _tagHelper = tagHelper;
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

            if (request is ITagCommandRequest tagCommandRequest)
            {
                return await HasCurrentUserAccessToProjectAsync(tagCommandRequest);
            }

            if (request is ITagQueryRequest tagQueryRequest)
            {
                return await HasCurrentUserAccessToProjectAsync(tagQueryRequest) && 
                    await HasCurrentUserAccessToContentAsync(tagQueryRequest);
            }

            return true;
        }

        private async Task<bool> HasCurrentUserAccessToContentAsync(ITagQueryRequest tagQueryRequest)
        {
            if (_contentRestrictionsChecker.HasCurrentUserAnyRestrictions())
            {
                var responsibleCode = await _tagHelper.GetResponsibleCodeAsync(tagQueryRequest.TagId);
                return _contentRestrictionsChecker.HasCurrentUserExplicitAccessToContent(responsibleCode);
            }

            return true;
        }

        private async Task<bool> HasCurrentUserAccessToProjectAsync(ITagCommandRequest tagCommandRequest)
            => await HasCurrentUserAccessToProjectAsync(tagCommandRequest.TagId);

        private async Task<bool> HasCurrentUserAccessToProjectAsync(ITagQueryRequest tagQueryRequest)
            => await HasCurrentUserAccessToProjectAsync(tagQueryRequest.TagId);

        private async Task<bool> HasCurrentUserAccessToProjectAsync(int tagId)
        {
            var projectName = await _tagHelper.GetProjectNameAsync(tagId);
            return _projectAccessChecker.HasCurrentUserAccessToProject(projectName);
        }

        private bool HasCurrentUserAccessToProject(IProjectRequest projectRequest)
            => _projectAccessChecker.HasCurrentUserAccessToProject(projectRequest.ProjectName);
    }
}

using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Query;
using Equinor.Procosys.Preservation.WebApi.Misc;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public class AccessValidator : IAccessValidator
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IProjectAccessChecker _projectAccessChecker;
        private readonly IContentRestrictionsChecker _contentRestrictionsChecker;
        private readonly ITagHelper _tagHelper;
        private readonly ILogger<AccessValidator> _logger;

        public AccessValidator(
            ICurrentUserProvider currentUserProvider, 
            IProjectAccessChecker projectAccessChecker,
            IContentRestrictionsChecker contentRestrictionsChecker,
            ITagHelper tagHelper, 
            ILogger<AccessValidator> logger)
        {
            _currentUserProvider = currentUserProvider;
            _projectAccessChecker = projectAccessChecker;
            _contentRestrictionsChecker = contentRestrictionsChecker;
            _tagHelper = tagHelper;
            _logger = logger;
        }

        public async Task<bool> ValidateAsync<TRequest>(TRequest request) where TRequest : IBaseRequest
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var userOid = _currentUserProvider.GetCurrentUserOid();
            if (request is IProjectRequest projectRequest && !_projectAccessChecker.HasCurrentUserAccessToProject(projectRequest.ProjectName))
            {
                _logger.LogWarning($"Current user {userOid} don't have access to project {projectRequest.ProjectName}");
                return false;
            }

            if (request is ITagCommandRequest tagCommandRequest)
            {
                var projectName = await _tagHelper.GetProjectNameAsync(tagCommandRequest.TagId);
                var accessToProject = _projectAccessChecker.HasCurrentUserAccessToProject(projectName);

                if (!accessToProject)
                {
                    _logger.LogWarning($"Current user {userOid} don't have access to project {projectName}");
                }

                var accessToContent = await HasCurrentUserAccessToContentAsync(tagCommandRequest);
                if (!accessToContent)
                {
                    _logger.LogWarning($"Current user {userOid} don't have access to content {tagCommandRequest.TagId}");
                }
                return accessToProject && accessToContent;
            }

            if (request is ITagQueryRequest tagQueryRequest)
            {
                var projectName = await _tagHelper.GetProjectNameAsync(tagQueryRequest.TagId);
                if (!_projectAccessChecker.HasCurrentUserAccessToProject(projectName))
                {
                    _logger.LogWarning($"Current user {userOid} don't have access to project {projectName}");
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> HasCurrentUserAccessToContentAsync(ITagCommandRequest tagCommandRequest)
        {
            if (_contentRestrictionsChecker.HasCurrentUserExplicitNoRestrictions())
            {
                return true;
            }

            var responsibleCode = await _tagHelper.GetResponsibleCodeAsync(tagCommandRequest.TagId);
            return _contentRestrictionsChecker.HasCurrentUserExplicitAccessToContent(responsibleCode);
        }
    }
}

using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Query;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.Preservation.WebApi.Authorizations
{
    public class AccessValidator : IAccessValidator
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IProjectAccessChecker _projectAccessChecker;
        private readonly ICrossPlantAccessChecker _crossPlantAccessChecker;
        private readonly IContentRestrictionsChecker _contentRestrictionsChecker;
        private readonly ITagHelper _tagHelper;
        private readonly ILogger<AccessValidator> _logger;

        public AccessValidator(
            ICurrentUserProvider currentUserProvider, 
            IProjectAccessChecker projectAccessChecker,
            ICrossPlantAccessChecker crossPlantAccessChecker,
            IContentRestrictionsChecker contentRestrictionsChecker,
            ITagHelper tagHelper, 
            ILogger<AccessValidator> logger)
        {
            _currentUserProvider = currentUserProvider;
            _projectAccessChecker = projectAccessChecker;
            _crossPlantAccessChecker = crossPlantAccessChecker;
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
                if (!await HasCurrentUserAccessToProjectAsync(tagCommandRequest.TagId, userOid))
                {
                    return false;
                }

                if (!await HasCurrentUserAccessToContentAsync(tagCommandRequest))
                {
                    _logger.LogWarning($"Current user {userOid} don't have access to content {tagCommandRequest.TagId}");
                    return false;
                }
            }

            if (request is ITagQueryRequest tagQueryRequest)
            {
                if (!await HasCurrentUserAccessToProjectAsync(tagQueryRequest.TagId, userOid))
                {
                    return false;
                }
            }

            if (request is ICrossPlantQueryRequest)
            {
                if (!IsUserCrossPlantUser(userOid))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsUserCrossPlantUser(Guid userOid)
        {
            var accessCrossPlant = _crossPlantAccessChecker.HasCurrentUserAccessToCrossPlant();

            if (!accessCrossPlant)
            {
                _logger.LogWarning($"Current user {userOid} don't have cross plant access");
                return false;
            }

            return true;
        }

        private async Task<bool> HasCurrentUserAccessToProjectAsync(int tagId, Guid userOid)
        {
            var projectName = await _tagHelper.GetProjectNameAsync(tagId);
            if (projectName != null)
            {
                var accessToProject = _projectAccessChecker.HasCurrentUserAccessToProject(projectName);

                if (!accessToProject)
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

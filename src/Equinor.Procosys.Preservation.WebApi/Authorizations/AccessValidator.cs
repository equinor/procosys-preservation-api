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
        private readonly IPlantProvider _plantProvider;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IPlantAccessChecker _plantAccessChecker;
        private readonly IProjectAccessChecker _projectAccessChecker;
        private readonly IContentRestrictionsChecker _contentRestrictionsChecker;
        private readonly ITagHelper _tagHelper;
        private readonly ILogger<AccessValidator> _logger;

        public AccessValidator(
            IPlantProvider plantProvider, 
            ICurrentUserProvider currentUserProvider, 
            IPlantAccessChecker plantAccessChecker,
            IProjectAccessChecker projectAccessChecker,
            IContentRestrictionsChecker contentRestrictionsChecker,
            ITagHelper tagHelper, 
            ILogger<AccessValidator> logger)
        {
            _plantProvider = plantProvider;
            _currentUserProvider = currentUserProvider;
            _plantAccessChecker = plantAccessChecker;
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

            var plantId = _plantProvider.Plant;
            if (string.IsNullOrEmpty(plantId))
            {
                // if request don't require access to any plant, access is given
                return true;
            }

            if (!_plantAccessChecker.HasCurrentUserAccessToPlant(plantId))
            {
                _logger.LogWarning($"Current user {_currentUserProvider.TryGetCurrentUserOid()} don't have access to plant {plantId}");
                return false;
            }

            if (request is IProjectRequest projectRequest && ! _projectAccessChecker.HasCurrentUserAccessToProject(projectRequest.ProjectName))
            {
                _logger.LogWarning($"Current user {_currentUserProvider.TryGetCurrentUserOid()} don't have access to project {projectRequest.ProjectName}");
                return false;
            }

            if (request is ITagCommandRequest tagCommandRequest)
            {
                var projectName = await _tagHelper.GetProjectNameAsync(tagCommandRequest.TagId);
                var accessToProject = _projectAccessChecker.HasCurrentUserAccessToProject(projectName);
                var accessToContent = await HasCurrentUserAccessToContentAsync(tagCommandRequest);

                if (!accessToProject)
                {
                    _logger.LogWarning($"Current user {_currentUserProvider.TryGetCurrentUserOid()} don't have access to project {projectName}");
                }
                if (!accessToContent)
                {
                    _logger.LogWarning($"Current user {_currentUserProvider.TryGetCurrentUserOid()} don't have access to content {tagCommandRequest.TagId}");
                }
                return accessToProject && accessToContent;
            }

            if (request is ITagQueryRequest tagQueryRequest)
            {
                var projectName = await _tagHelper.GetProjectNameAsync(tagQueryRequest.TagId);
                if (!_projectAccessChecker.HasCurrentUserAccessToProject(projectName))
                {
                    _logger.LogWarning($"Current user {_currentUserProvider.TryGetCurrentUserOid()} don't have access to project {projectName}");
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> HasCurrentUserAccessToContentAsync(ITagCommandRequest tagCommandRequest)
        {
            if (_contentRestrictionsChecker.HasCurrentUserAnyRestrictions())
            {
                var responsibleCode = await _tagHelper.GetResponsibleCodeAsync(tagCommandRequest.TagId);
                return _contentRestrictionsChecker.HasCurrentUserExplicitAccessToContent(responsibleCode);
            }

            return true;
        }
    }
}

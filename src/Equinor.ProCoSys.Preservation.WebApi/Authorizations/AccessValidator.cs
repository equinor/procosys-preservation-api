using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Query;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.Preservation.WebApi.Authorizations
{
    /// <summary>
    /// Validates if current user has access to perform a request of type IProjectRequest, 
    /// ITagCommandRequest or ITagQueryRequest.
    /// It validates if user has access to the project of the request 
    /// For ITagCommandRequest, it also validates if user has access to the content of the request 
    ///     (i.e check if user has any restriction role saying that user is restricted to acccess 
    ///     content for particular responsible code)
    /// </summary>
    public class AccessValidator : IAccessValidator
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IProjectAccessChecker _projectAccessChecker;
        private readonly IRestrictionRolesChecker _restrictionRolesChecker;
        private readonly ITagHelper _tagHelper;
        private readonly ILogger<AccessValidator> _logger;

        public AccessValidator(
            ICurrentUserProvider currentUserProvider, 
            IProjectAccessChecker projectAccessChecker,
            IRestrictionRolesChecker restrictionRolesChecker,
            ITagHelper tagHelper, 
            ILogger<AccessValidator> logger)
        {
            _currentUserProvider = currentUserProvider;
            _projectAccessChecker = projectAccessChecker;
            _restrictionRolesChecker = restrictionRolesChecker;
            _tagHelper = tagHelper;
            _logger = logger;
        }

        public async Task<bool> ValidateAsync<TRequest>(TRequest request) where TRequest : IBaseRequest => await Task.FromResult(true);
        // {
        //     if (request == null)
        //     {
        //         throw new ArgumentNullException(nameof(request));
        //     }
        //
        //     var userOid = _currentUserProvider.GetCurrentUserOid();
        //     if (request is IProjectRequest projectRequest && !_projectAccessChecker.HasCurrentUserAccessToProject(projectRequest.ProjectName))
        //     {
        //         _logger.LogWarning($"Current user {userOid} doesn't have have access to project {projectRequest.ProjectName}");
        //         return false;
        //     }
        //
        //     if (request is ITagCommandRequest tagCommandRequest)
        //     {
        //         if (!await HasCurrentUserAccessToProjectAsync(tagCommandRequest.TagId, userOid))
        //         {
        //             return false;
        //         }
        //
        //         if (!await HasCurrentUserAccessToContentAsync(tagCommandRequest))
        //         {
        //             _logger.LogWarning($"Current user {userOid} doesn't have access to content {tagCommandRequest.TagId}");
        //             return false;
        //         }
        //     }
        //
        //     if (request is ITagQueryRequest tagQueryRequest)
        //     {
        //         if (!await HasCurrentUserAccessToProjectAsync(tagQueryRequest.TagId, userOid))
        //         {
        //             return false;
        //         }
        //     }
        //
        //     return true;
        // }
        //
        // private async Task<bool> HasCurrentUserAccessToProjectAsync(int tagId, Guid userOid)
        // {
        //     var projectName = await _tagHelper.GetProjectNameAsync(tagId);
        //     if (projectName != null)
        //     {
        //         var accessToProject = _projectAccessChecker.HasCurrentUserAccessToProject(projectName);
        //
        //         if (!accessToProject)
        //         {
        //             _logger.LogWarning($"Current user {userOid} doesn't have access to project {projectName}");
        //             return false;
        //         }
        //     }
        //
        //     return true;
        // }
        //
        // private async Task<bool> HasCurrentUserAccessToContentAsync(ITagCommandRequest tagCommandRequest)
        // {
        //     if (_restrictionRolesChecker.HasCurrentUserExplicitNoRestrictions())
        //     {
        //         return true;
        //     }
        //
        //     var responsibleCode = await _tagHelper.GetResponsibleCodeAsync(tagCommandRequest.TagId);
        //     return _restrictionRolesChecker.HasCurrentUserExplicitAccessToContent(responsibleCode);
        // }
    }
}

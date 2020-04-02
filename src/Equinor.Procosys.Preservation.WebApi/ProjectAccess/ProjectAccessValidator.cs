using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.ProjectAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TagAction = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.WebApi.ProjectAccess
{
    public class ProjectAccessValidator : IProjectAccessValidator
    {
        private readonly IProjectAccessChecker _projectAccessChecker;
        private readonly IReadOnlyContext _context;

        public ProjectAccessValidator(IProjectAccessChecker projectAccessChecker, IReadOnlyContext context)
        {
            _projectAccessChecker = projectAccessChecker;
            _context = context;
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
                        projectName = await GetProjectNameFromTagIdAsync((int)propertyValue);
                        break;
                    case PathToProjectType.ActionId:
                        projectName = await GetProjectNameFromActionIdAsync((int)propertyValue);
                        break;
                    case PathToProjectType.RequirementId:
                        projectName = await GetProjectNameFromRequirementIdAsync((int)propertyValue);
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

        private async Task<string> GetProjectNameFromRequirementIdAsync(int requirementId)
        {
            var projectName = await (from p in _context.QuerySet<Project>() 
                join tag in _context.QuerySet<Tag>() on p.Id equals EF.Property<int>(tag, "ProjectId")
                join req in _context.QuerySet<Requirement>() on tag.Id equals EF.Property<int>(req, "TagId")
                where req.Id == requirementId
                select p.Name).FirstOrDefaultAsync();
            
            return projectName;
        }

        private async Task<string> GetProjectNameFromActionIdAsync(int actionId)
        {
            var projectName = await (from p in _context.QuerySet<Project>() 
                join tag in _context.QuerySet<Tag>() on p.Id equals EF.Property<int>(tag, "ProjectId")
                join action in _context.QuerySet<TagAction>() on tag.Id equals EF.Property<int>(action, "TagId")
                where action.Id == actionId
                select p.Name).FirstOrDefaultAsync();
            
            return projectName;
        }

        private async Task<string> GetProjectNameFromTagIdAsync(int tagId)
        {
            var projectName = await (from p in _context.QuerySet<Project>() 
                join tag in _context.QuerySet<Tag>() on p.Id equals EF.Property<int>(tag, "ProjectId")
                where tag.Id == tagId
                select p.Name).FirstOrDefaultAsync();
            
            return projectName;
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

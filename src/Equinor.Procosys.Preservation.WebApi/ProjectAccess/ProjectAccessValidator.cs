using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.ProjectAccess;
using Microsoft.EntityFrameworkCore;

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

        public async Task<bool> ValidateAsync(object request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            string projectName;

            var pathToProject = request.GetType().GetCustomAttribute<PathToProjectAttribute>();
            if (pathToProject != null)
            {
                var propertyValue = GetPropertyValue(request, pathToProject.PropertyName);

                switch (pathToProject.PathToProjectType)
                {
                    case PathToProjectType.ProjectName:
                        projectName = propertyValue as string;
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
            return null;
        }

        private async Task<string> GetProjectNameFromActionIdAsync(int actionId)
        {
            return null;
        }

        private async Task<string> GetProjectNameFromTagIdAsync(int tagId)
        {
            var projectName = await (from tag in _context.QuerySet<Tag>()
                join p in _context.QuerySet<Project>() on EF.Property<int>(tag, "ProjectId") equals p.Id
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

using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.WebApi.ProjectAccess
{
    public class ProjectHelper : IProjectHelper
    {
        private readonly IReadOnlyContext _context;

        public ProjectHelper(IReadOnlyContext context) => _context = context;

        public async Task<string> GetProjectNameFromTagIdAsync(int tagId)
        {
            var projectName = await (from p in _context.QuerySet<Project>() 
                join tag in _context.QuerySet<Tag>() on p.Id equals EF.Property<int>(tag, "ProjectId")
                where tag.Id == tagId
                select p.Name).FirstOrDefaultAsync();
            
            return projectName;
        }

        public async Task<string> GetProjectNameFromActionIdAsync(int actionId)
        {
            var projectName = await (from p in _context.QuerySet<Project>() 
                join tag in _context.QuerySet<Tag>() on p.Id equals EF.Property<int>(tag, "ProjectId")
                join action in _context.QuerySet<Action>() on tag.Id equals EF.Property<int>(action, "TagId")
                where action.Id == actionId
                select p.Name).FirstOrDefaultAsync();
            
            return projectName;
        }

        public async Task<string> GetProjectNameFromRequirementIdAsync(int requirementId)
        {
            var projectName = await (from p in _context.QuerySet<Project>() 
                join tag in _context.QuerySet<Tag>() on p.Id equals EF.Property<int>(tag, "ProjectId")
                join req in _context.QuerySet<Requirement>() on tag.Id equals EF.Property<int>(req, "TagId")
                where req.Id == requirementId
                select p.Name).FirstOrDefaultAsync();
            
            return projectName;
        }
    }
}

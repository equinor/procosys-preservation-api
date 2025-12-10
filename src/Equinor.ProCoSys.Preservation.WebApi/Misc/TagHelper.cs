using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.Preservation.WebApi.Misc
{
    public class TagHelper : ITagHelper
    {
        private readonly IReadOnlyContext _context;

        public TagHelper(IReadOnlyContext context) => _context = context;

        public async Task<string> GetProjectNameAsync(int tagId)
        {
            var projectName = await (from p in _context.QuerySet<Project>()
                                     join tag in _context.QuerySet<Tag>() on p.Id equals EF.Property<int>(tag, "ProjectId")
                                     where tag.Id == tagId
                                     select p.Name).SingleOrDefaultAsync();

            return projectName;
        }

        public async Task<string> GetResponsibleCodeAsync(int tagId)
        {
            var responsibleCode = await (from tag in _context.QuerySet<Tag>()
                                         join step in _context.QuerySet<Step>() on tag.StepId equals step.Id
                                         join resp in _context.QuerySet<Responsible>() on step.ResponsibleId equals resp.Id
                                         where tag.Id == tagId
                                         select resp.Code).SingleOrDefaultAsync();

            return responsibleCode;
        }
    }
}

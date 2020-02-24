using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Microsoft.EntityFrameworkCore;
using DomProject = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Project;

namespace Equinor.Procosys.Preservation.Command.Validators.Project
{
    public class ProjectValidator : IProjectValidator
    {
        private readonly IReadOnlyContext _context;

        public ProjectValidator(IReadOnlyContext context) => _context = context;

        public async Task<bool> ExistsAsync(string projectName, CancellationToken cancellationToken)
        {
            var count = await (from p in _context.QuerySet<DomProject>()
                where p.Name == projectName
                select p).CountAsync(cancellationToken);
            return count > 0;
        }

        public async Task<bool> IsExistingAndClosedAsync(string projectName, CancellationToken cancellationToken)
        {
            var project = await (from p in _context.QuerySet<DomProject>()
                where p.Name == projectName
                select p).FirstOrDefaultAsync(cancellationToken);

            return project != null && project.IsClosed;
        }

        public async Task<bool> IsClosedForTagAsync(int tagId, CancellationToken cancellationToken)
        {
            var project = await (from tag in _context.QuerySet<Domain.AggregateModels.ProjectAggregate.Tag>()
                join p in _context.QuerySet<DomProject>() on EF.Property<int>(tag, "ProjectId") equals p.Id
                where tag.Id == tagId
                select p).FirstOrDefaultAsync(cancellationToken);

            return project != null && project.IsClosed;
        }
    }
}

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetProjectByName
{
    public class GetProjectByNameQueryHandler : IRequestHandler<GetProjectByNameQuery, Result<ProjectDetailsDto>>
    {
        private readonly IReadOnlyContext _context;

        public GetProjectByNameQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<ProjectDetailsDto>> Handle(GetProjectByNameQuery request, CancellationToken cancellationToken)
        {
            var project = await (from p in _context.QuerySet<Project>()
                where p.Name == request.ProjectName
                select p).SingleOrDefaultAsync(cancellationToken);
            
            if (project == null)
            {
                return new NotFoundResult<ProjectDetailsDto>(Strings.EntityNotFound(nameof(Project), request.ProjectName));
            }

            var projectDto = new ProjectDetailsDto(
                project.Id,
                project.Name,
                project.Description,
                project.IsClosed);

            return new SuccessResult<ProjectDetailsDto>(projectDto);
        }
    }
}

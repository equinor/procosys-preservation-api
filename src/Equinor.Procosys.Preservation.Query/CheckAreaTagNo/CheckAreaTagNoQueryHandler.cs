using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.CheckAreaTagNo
{
    public class CheckAreaTagNoQueryHandler : IRequestHandler<CheckAreaTagNoQuery, Result<AreaTagDto>>
    {
        private readonly IReadOnlyContext _context;

        public CheckAreaTagNoQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<AreaTagDto>> Handle(CheckAreaTagNoQuery request, CancellationToken cancellationToken)
        {
            var tagNo = request.GetTagNo();
            var areaTagDto = new AreaTagDto(tagNo)
            {
                Exists = await (from tag in _context.QuerySet<Tag>()
                    join p in _context.QuerySet<Project>() on EF.Property<int>(tag, "ProjectId") equals p.Id
                    where tag.TagNo == tagNo && p.Name == request.ProjectName
                    select p).AnyAsync(cancellationToken)
            };
            return new SuccessResult<AreaTagDto>(areaTagDto);
        }
    }
}

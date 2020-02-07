using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagDetails
{
    public class GetTagDetailsQueryHandler : IRequestHandler<GetTagDetailsQuery, Result<TagDetailsDto>>
    {
        private readonly IReadOnlyContext _readOnlyContext;
        private readonly PreservationContext _context;

        public GetTagDetailsQueryHandler(IReadOnlyContext readOnlyContext)
        {
            _readOnlyContext = readOnlyContext;
        }

        public async Task<Result<TagDetailsDto>> Handle(GetTagDetailsQuery request, CancellationToken cancellationToken)
        {
            var tagDetails = await _context.Set<TagDetailsDto>().ToListAsync();

            return new SuccessResult<TagDetailsDto>(tagDetails.FirstOrDefault());
        }
    }
}

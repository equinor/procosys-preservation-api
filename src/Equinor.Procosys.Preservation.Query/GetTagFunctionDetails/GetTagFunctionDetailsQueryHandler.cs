using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagFunctionDetails
{
    public class GetTagFunctionDetailsQueryHandler : IRequestHandler<GetTagFunctionDetailsQuery, Result<TagFunctionDetailsDto>>
    {
        private readonly IReadOnlyContext _context;

        public GetTagFunctionDetailsQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<TagFunctionDetailsDto>> Handle(GetTagFunctionDetailsQuery request, CancellationToken cancellationToken)
        {
            var tagFunctionDto = await (from tagFunction in _context.QuerySet<TagFunction>().Include(tf => tf.Requirements)
                    where tagFunction.Code == request.TagFunctionCode &&
                          tagFunction.RegisterCode == request.RegisterCode
                    select new TagFunctionDetailsDto(
                        tagFunction.Id,
                        tagFunction.Code,
                        tagFunction.Description,
                        tagFunction.RegisterCode,
                        tagFunction.IsVoided,
                        tagFunction.Requirements.Select(s => new RequirementDto(s.Id, s.RequirementDefinitionId)),
                        tagFunction.RowVersion.ConvertToString()))
                .SingleOrDefaultAsync(cancellationToken);
            
            if (tagFunctionDto == null)
            {
                return new NotFoundResult<TagFunctionDetailsDto>($"{nameof(TagFunction)} with Code {request.TagFunctionCode} not found in register {request.RegisterCode}");
            }

            return new SuccessResult<TagFunctionDetailsDto>(tagFunctionDto);
        }
    }
}

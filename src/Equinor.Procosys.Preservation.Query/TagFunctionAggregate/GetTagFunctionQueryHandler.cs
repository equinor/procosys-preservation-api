using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.TagFunctionAggregate
{
    public class GetTagFunctionQueryHandler : IRequestHandler<GetTagFunctionQuery, Result<TagFunctionDto>>
    {
        private readonly IReadOnlyContext _context;

        public GetTagFunctionQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<TagFunctionDto>> Handle(GetTagFunctionQuery request, CancellationToken cancellationToken)
        {
            var tagFunctionDto = await (from tagFunction in _context.QuerySet<TagFunction>().Include(tf => tf.Requirements)
                    where tagFunction.Code == request.TagFunctionCode &&
                          tagFunction.RegisterCode == request.RegisterCode
                    select new TagFunctionDto(
                        tagFunction.Id,
                        tagFunction.Code,
                        tagFunction.Description,
                        tagFunction.RegisterCode,
                        tagFunction.IsVoided,
                        tagFunction.Requirements.Select(s => new RequirementDto(s.Id, s.RequirementDefinitionId))))
                .SingleOrDefaultAsync(cancellationToken);
            
            if (tagFunctionDto == null)
            {
                return new NotFoundResult<TagFunctionDto>($"{nameof(TagFunction)} with Code {request.TagFunctionCode} not found in register {request.RegisterCode}");
            }

            return new SuccessResult<TagFunctionDto>(tagFunctionDto);
        }
    }
}

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.GetRequirementTypeById
{
    public class GetRequirementTypeByIdQueryHandler : IRequestHandler<GetRequirementTypeByIdQuery, Result<RequirementTypeDetailsDto>>
    {
        private readonly IReadOnlyContext _context;

        public GetRequirementTypeByIdQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<RequirementTypeDetailsDto>> Handle(GetRequirementTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var reqType = await (from rt in _context.QuerySet<RequirementType>()
                    .Include(rt => rt.RequirementDefinitions)
                    .ThenInclude(rd => rd.Fields)
                                 where rt.Id == request.Id
                                 select rt).SingleOrDefaultAsync(cancellationToken);

            if (reqType == null)
            {
                return new NotFoundResult<RequirementTypeDetailsDto>(Strings.EntityNotFound(nameof(RequirementType), request.Id));
            }

            var requirementDefinitionIds = reqType.RequirementDefinitions.Select(r => r.Id);

            var tagRequirements = await (from tr in _context.QuerySet<TagRequirement>()
                                         where requirementDefinitionIds.Contains(tr.RequirementDefinitionId)
                                         select tr).ToListAsync(cancellationToken);

            var tagFunctionRequirements = await (from tfr in _context.QuerySet<TagFunctionRequirement>()
                                                 where requirementDefinitionIds.Contains(tfr.RequirementDefinitionId)
                                                 select tfr).ToListAsync(cancellationToken);

            var fieldIds = reqType.RequirementDefinitions.SelectMany(r => r.Fields).Select(f => f.Id);
            var fieldValues = await (from fv in _context.QuerySet<FieldValue>()
                                     where fieldIds.Contains(fv.Id)
                                     select fv).ToListAsync(cancellationToken);

            var dto = new RequirementTypeDetailsDto(
                reqType.Id,
                reqType.Code,
                reqType.Title,
                reqType.Icon,
                reqType.RequirementDefinitions.Any(),
                reqType.IsVoided,
                reqType.SortKey,
                reqType.RequirementDefinitions.Select(rd =>
                {
                    var definitionIsInUse = rd.Fields.Any()
                                || tagRequirements.Any(tr => tr.RequirementDefinitionId == rd.Id)
                                || tagFunctionRequirements.Any(tr => tr.RequirementDefinitionId == rd.Id);
                    return new RequirementDefinitionDetailDto(rd.Id,
                        rd.Title,
                        definitionIsInUse,
                        rd.IsVoided,
                        rd.DefaultIntervalWeeks,
                        rd.Usage,
                        rd.SortKey,
                        rd.NeedsUserInput,
                        rd.OrderedFields(true)
                            .Select(f =>
                            {
                                var fieldIsInUse = fieldValues.Any(fv => fv.FieldId == f.Id);
                                return new FieldDetailsDto(
                                    f.Id,
                                    f.Label,
                                    fieldIsInUse,
                                    f.IsVoided,
                                    f.FieldType,
                                    f.SortKey,
                                    f.Unit,
                                    f.ShowPrevious,
                                    f.RowVersion.ConvertToString());
                            }),
                        rd.RowVersion.ConvertToString());
                }),
                reqType.RowVersion.ConvertToString());

            return new SuccessResult<RequirementTypeDetailsDto>(dto);
        }
    }
}

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetPreservationRecord
{
    public class GetPreservationRecordQueryHandler : IRequestHandler<GetPreservationRecordQuery,
            Result<PreservationRecordDto>>
    {
        private readonly IReadOnlyContext _context;

        public GetPreservationRecordQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<PreservationRecordDto>> Handle(
            GetPreservationRecordQuery request,
            CancellationToken cancellationToken)
        {
            // Get tag with all requirements, all periods, all preservation records, all field values
            var tag = await
                (from t in _context.QuerySet<Tag>()
                        .Include(t => t.Requirements)
                        .ThenInclude(r => r.PreservationPeriods)
                        .ThenInclude(p => p.PreservationRecord)
                        .Include(t => t.Requirements)
                        .ThenInclude(r => r.PreservationPeriods)
                        .ThenInclude(p => p.FieldValues)
                        .ThenInclude(fv => fv.FieldValueAttachment)
                    where t.Id == request.TagId
                    select t).SingleOrDefaultAsync(cancellationToken);

            if (tag == null)
            {
                return new NotFoundResult<PreservationRecordDto>(Strings.EntityNotFound(nameof(Tag), request.TagId));
            }

            var tagRequirement = tag.Requirements.SingleOrDefault(r => r.Id == request.TagRequirementId);
            if (tagRequirement == null)
            {
                return new NotFoundResult<PreservationRecordDto>(Strings.EntityNotFound(nameof(TagRequirement), request.TagRequirementId));
            }

            var requirementDefinitionId = tagRequirement.RequirementDefinitionId;

            // get needed information about requirementType/Definition for all requirement on tag
            var requirementDto = await
                (from requirementDefinition in _context.QuerySet<RequirementDefinition>().Include(rd => rd.Fields)
                    join requirementType in _context.QuerySet<RequirementType>()
                        on EF.Property<int>(requirementDefinition, "RequirementTypeId") equals requirementType.Id
                    where requirementDefinitionId == requirementDefinition.Id
                    select new
                    {
                        RequirementType = requirementType,
                        RequirementDefinition = requirementDefinition,
                    }
                ).SingleOrDefaultAsync(cancellationToken);


            var orderedPreservationPeriods = tagRequirement.PreservationPeriods
                .Where(pp => pp.PreservationRecord != null)
                .OrderBy(pp => pp.PreservationRecord.PreservedAtUtc)
                .Select(pp => pp)
                .ToList();

            var preservationPeriod = orderedPreservationPeriods
                .Where(pp => pp.PreservationRecord.ObjectGuid == request.PreservationRecordGuid)
                .Select(pp => pp)
                .SingleOrDefault();

            if (preservationPeriod == null)
            {
                return new NotFoundResult<PreservationRecordDto>($"{nameof(PreservationPeriod)} not found");
            }

            var indexOfPreservationPeriod = orderedPreservationPeriods
                .ToList()
                .IndexOf(preservationPeriod);

            var previousPreservedPeriod = indexOfPreservationPeriod > 0
                ? tagRequirement.PreservationPeriods.ToList()[indexOfPreservationPeriod - 1]
                : null;

            var fieldList = requirementDto.RequirementDefinition.Fields.ToList();
            var fields = fieldList.Select(f =>
            {
                var currentValue = preservationPeriod.GetFieldValue(f.Id);
                var previousValue = previousPreservedPeriod?.GetFieldValue(f.Id);
                return new FieldDetailsDto(f, currentValue, previousValue);
            }).ToList();

            var preservationRecordDto = new PreservationRecordDto(
                preservationPeriod.PreservationRecord.Id,
                preservationPeriod.PreservationRecord.BulkPreserved,
                new RequirementTypeDetailsDto(
                    requirementDto.RequirementType.Id,
                    requirementDto.RequirementType.Code,
                    requirementDto.RequirementType.Icon,
                    requirementDto.RequirementType.Title), 
                new RequirementDefinitionDetailDto(
                    requirementDto.RequirementDefinition.Id,
                    requirementDto.RequirementDefinition.Title), 
                tagRequirement.IntervalWeeks,
                preservationPeriod.Comment, 
                fields);

            return new SuccessResult<PreservationRecordDto>(preservationRecordDto);
        }
    }
}

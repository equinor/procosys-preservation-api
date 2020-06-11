using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Query.GetTags;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetPreservationRecord
{
    public class GetPreservationRecordQueryHandler : IRequestHandler<GetPreservationRecordQuery, Result<List<PreservationRecordDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetPreservationRecordQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<PreservationRecordDto>>> Handle(GetPreservationRecordQuery request, CancellationToken cancellationToken)
        {
            // Get tag with all requirements, all periods, all preservation records
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
                return new NotFoundResult<List<PreservationRecordDto>>($"{nameof(Tag)} with ID {request.TagId} not found");
            }

            var requirement = tag.Requirements.SingleOrDefault(r => r.Id == request.RequirementId);
            if (requirement == null)
            {
                return new NotFoundResult<List<PreservationRecordDto>>(Strings.EntityNotFound(nameof(TagRequirement), request.RequirementId));
            }

            var records = requirement.PreservationPeriods.Select(pp => pp.PreservationRecord);
            var preservationRecord = records.SingleOrDefault(pr => pr.Id == request.PreservationRecordId);
            if (preservationRecord == null)
            {
                return new NotFoundResult<List<PreservationRecordDto>>(Strings.EntityNotFound(nameof(PreservationPeriod), request.PreservationRecordId));
            }

            // get needed information about preservation record
            var preservationRecordDto =
                (from pr in _context.QuerySet<PreservationRecord>()
                 where pr.Id == preservationRecord.Id
                 select new 
                 {
                     pr.Id,
                     pr.BulkPreserved,
                     pr.RowVersion
                 }
                );

            return new SuccessResult<PreservationRecordDto>(preservationRecordDto);
        }
    }

}

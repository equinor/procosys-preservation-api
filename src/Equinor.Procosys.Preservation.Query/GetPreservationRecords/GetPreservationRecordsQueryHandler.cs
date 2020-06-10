using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetPreservationRecords
{
    public class GetPreservationRecordsQueryHandler : IRequestHandler<GetPreservationRecordsQuery, Result<List<PreservationRecordDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetPreservationRecordsQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<PreservationRecordDto>>> Handle(GetPreservationRecordsQuery request, CancellationToken cancellationToken)
        {
            // Get tag with all requirements, all periods, all preservation records
            var tag = await
                (from t in _context.QuerySet<Tag>()
                        .Include(t => t.Requirements)
                            .ThenInclude(r => r.PreservationPeriods)
                            .ThenInclude(p => p.PreservationRecord)
                    where t.Id == request.TagId
                 select t).SingleOrDefaultAsync(cancellationToken);

            var tagReq = await
                (from t in _context.QuerySet<TagRequirement>()
                        .Include(p => p.PreservationPeriods)
                            .ThenInclude(r => r.PreservationRecord)
                    where t.Id == request.TagId
                    select t).SingleOrDefaultAsync(cancellationToken);

            if (tag == null)
            {
                return new NotFoundResult<List<PreservationRecordDto>>($"{nameof(Tag)} with ID {request.TagId} not found");
            }

            var preservationRecord = tagReq.PreservationPeriods.Select(r => r.PreservationRecord).ToList();

            // get needed information about preservation record for all periods on tag
            var preservationRecordDtos = await
                (from preservRecord in _context.QuerySet<PreservationRecord>()
                    join preservPeriod in _context.QuerySet<PreservationPeriod>()
                        on EF.Property<int>(preservationRecord, "Id") equals preservPeriod.Id
                 where preservationRecord.Contains(preservRecord)
                    select new Dto
                    {
                        PresRecordBulkReserved = preservRecord.BulkPreserved,
                        PreservationRecord = preservRecord
                    }
                ).ToListAsync(cancellationToken);

            var preservationRecords = tagReq
                .PreservationPeriods
                .Select(prsvPeriods =>
                {
                    var PresevRecordDto =
                        preservationRecordDtos.Single(rd => rd.PreservationRecord == prsvPeriods.PreservationRecord);

                    return new PreservationRecordDto(
                        prsvPeriods.Id,
                        PresevRecordDto.PresRecordBulkReserved,
                        prsvPeriods.RowVersion.ConvertToString());
                }).ToList();
    
            return new SuccessResult<List<PreservationRecordDto>>(preservationRecords);
        }
        
        private class Dto
        {
            public bool PresRecordBulkReserved { get; set; }
            public PreservationRecord PreservationRecord { get; set; }
        }
    }

}

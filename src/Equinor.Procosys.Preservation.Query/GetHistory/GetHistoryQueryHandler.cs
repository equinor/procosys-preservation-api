using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetHistory
{
    public class GetHistoryQueryHandler : IRequestHandler<GetHistoryQuery, Result<List<HistoryDto>>>
    {
        private readonly IReadOnlyContext _context;

        public GetHistoryQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<List<HistoryDto>>> Handle(GetHistoryQuery request, CancellationToken cancellationToken)
        {
            var tagHistory = await (from h in _context.QuerySet<History>()
                join tag in _context.QuerySet<Tag>() on h.ObjectGuid equals tag.ObjectGuid
                join createdBy in _context.QuerySet<Person>() on h.CreatedById equals createdBy.Id
                //join preservationRecord in _context.QuerySet<PreservationRecord>() on h.PreservationRecordGuid equals preservationRecord.ObjectGuid
                //join preservationPeriod in _context.QuerySet<PreservationPeriod>() on preservationRecord.Id equals preservationPeriod.PreservationRecord.Id
                from preservationRecord in _context.QuerySet<PreservationRecord>()
                    .Where(pr => pr.ObjectGuid == EF.Property<Guid>(h, "PreservationRecordGuid")).DefaultIfEmpty() //left join
                from preservationPeriod in _context.QuerySet<PreservationPeriod>()
                    .Where(pr => pr.PreservationRecord.Id == EF.Property<int>(preservationRecord, "Id")).DefaultIfEmpty() // left join
                where tag.ObjectGuid == h.ObjectGuid
                select new HistoryDto(
                    h.Id,
                    h.Description,
                    h.CreatedAtUtc,
                    new PersonDto(createdBy.Id, createdBy.FirstName, createdBy.LastName),
                    h.EventType,
                    h.DueInWeeks,
                    preservationPeriod.TagRequirementId,
                    h.PreservationRecordGuid))
                //.OrderByDescending(dto => dto.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return new SuccessResult<List<HistoryDto>>(tagHistory);
        }
    }
}

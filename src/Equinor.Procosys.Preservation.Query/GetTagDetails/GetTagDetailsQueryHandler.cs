using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagDetails
{
    public class GetTagDetailsQueryHandler : IRequestHandler<GetTagDetailsQuery, Result<TagDetailsDto>>
    {
        private readonly IReadOnlyContext _context;

        public GetTagDetailsQueryHandler(IReadOnlyContext context) => _context = context;

        public async Task<Result<TagDetailsDto>> Handle(GetTagDetailsQuery request, CancellationToken cancellationToken)
        {
            // Requirements and it's PreservationPeriods needs to be included so tag.IsReadyToBePreserved calculates as it should
            var tagDetails = await (from tag in _context.QuerySet<Tag>().Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods)
                                    join step in _context.QuerySet<Step>() on tag.StepId equals step.Id
                                    join journey in _context.QuerySet<Journey>() on EF.Property<int>(step, "JourneyId") equals journey.Id
                                    join mode in _context.QuerySet<Mode>() on step.ModeId equals mode.Id
                                    join responsible in _context.QuerySet<Responsible>() on step.ResponsibleId equals responsible.Id
                                    where tag.Id == request.TagId
                                    select new TagDetailsDto
                                    {
                                        AreaCode = tag.AreaCode,
                                        CommPkgNo = tag.CommPkgNo,
                                        Description = tag.Description,
                                        Id = tag.Id,
                                        JourneyTitle = journey.Title,
                                        McPkgNo = tag.McPkgNo,
                                        Mode = mode.Title,
                                        PurchaseOrderNo = tag.PurchaseOrderNo,
                                        Remark = tag.Remark,
                                        ResponsibleName = responsible.Code,
                                        Status = tag.Status,
                                        StorageArea = tag.StorageArea,
                                        TagNo = tag.TagNo,
                                        TagType = tag.TagType,
                                        ReadyToBePreserved = tag.IsReadyToBePreserved(),
                                        RowVersion = tag.RowVersion.ToULong()
                                    }).SingleOrDefaultAsync(cancellationToken);

            if (tagDetails == null)
            {
                return new NotFoundResult<TagDetailsDto>($"Entity with ID {request.TagId} not found");
            }

            return new SuccessResult<TagDetailsDto>(tagDetails);
        }
    }
}

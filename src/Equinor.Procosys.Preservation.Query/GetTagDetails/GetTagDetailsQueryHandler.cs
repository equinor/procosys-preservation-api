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
            var tagDetails = await (from tag in _context.QuerySet<Tag>()
                                                .Include(t => t.Actions)
                                                .Include(t => t.Attachments)
                                                .Include(t => t.Requirements)
                                                    .ThenInclude(r => r.PreservationPeriods)
                                    join step in _context.QuerySet<Step>() on tag.StepId equals step.Id
                                    join journey in _context.QuerySet<Journey>() on EF.Property<int>(step, "JourneyId") equals journey.Id
                                    join mode in _context.QuerySet<Mode>() on step.ModeId equals mode.Id
                                    join responsible in _context.QuerySet<Responsible>() on step.ResponsibleId equals responsible.Id
                                    where tag.Id == request.TagId
                                    select new TagDetailsDto(
                                        tag.Id,
                                        tag.TagNo,
                                        tag.Status != PreservationStatus.NotStarted || tag.Actions.Any() || tag.Attachments.Any(),
                                        tag.IsVoided,
                                        tag.Description,
                                        tag.Status.GetDisplayValue(),
                                        new JourneyDetailsDto(journey.Id, journey.Title),
                                        new StepDetailsDto(step.Id, step.Title), 
                                        new ModeDetailsDto(mode.Id, mode.Title),
                                        new ResponsibleDetailsDto(responsible.Id, responsible.Code, responsible.Description),
                                        tag.CommPkgNo,
                                        tag.McPkgNo,
                                        tag.Calloff,
                                        tag.PurchaseOrderNo,
                                        tag.AreaCode, 
                                        tag.TagType,
                                        tag.IsReadyToBePreserved(),
                                        tag.Remark,
                                        tag.StorageArea,
                                        tag.RowVersion.ConvertToString())).SingleOrDefaultAsync(cancellationToken);

            if (tagDetails == null)
            {
                return new NotFoundResult<TagDetailsDto>($"Entity with ID {request.TagId} not found");
            }

            return new SuccessResult<TagDetailsDto>(tagDetails);
        }
    }
}

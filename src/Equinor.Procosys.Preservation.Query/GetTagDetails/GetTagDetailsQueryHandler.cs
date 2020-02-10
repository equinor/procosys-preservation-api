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
        private readonly PreservationContext _context;

        public GetTagDetailsQueryHandler(PreservationContext context)
        {
            _context = context;
        }

        public async Task<Result<TagDetailsDto>> Handle(GetTagDetailsQuery request, CancellationToken cancellationToken)
        {
            var tagDetails =    await (from tag in _context.Tags.Include(tag => tag.Requirements)
                                join step in _context.Step on tag.StepId equals step.Id
                                join journey in _context.Journeys on EF.Property<int>(step, "JourneyId") equals journey.Id
                                join mode in _context.Modes on step.ModeId equals mode.Id
                                join responsible in _context.Responsibles on step.ResponsibleId equals responsible.Id
                                where tag.Id == request.Id
                                select new TagDetailsDto
                                {
                                    Area = tag.AreaCode,
                                    CommPkgNo = tag.CommPkgNo,
                                    Description = tag.Description,
                                    Id = tag.Id,
                                    JourneyName = journey.Title,
                                    McPkgNo = tag.McPkgNo,
                                    NextDueDate = tag.NextDueTimeUtc,
                                    PoNo = tag.PurchaseOrderNo,
                                    Status = tag.Status,
                                    TagNo = tag.TagNo
                                }).FirstOrDefaultAsync();

            if (tagDetails == null)
            {
                return new NotFoundResult<TagDetailsDto>($"Entity with ID {request.Id} not found");
            }

            return new SuccessResult<TagDetailsDto>(tagDetails);
        }
    }
}

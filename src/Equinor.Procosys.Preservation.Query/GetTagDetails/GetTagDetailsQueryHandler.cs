using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.GetTagDetails
{
    public class GetTagDetailsQueryHandler : IRequestHandler<GetTagDetailsQuery, Result<TagDetailsDto>>
    {
        private readonly IReadOnlyContext _readOnlyContext;

        public GetTagDetailsQueryHandler(IReadOnlyContext readOnlyContext)
        {
            _readOnlyContext = readOnlyContext;
        }

        public async Task<Result<TagDetailsDto>> Handle(GetTagDetailsQuery request, CancellationToken cancellationToken)
        {
            //DbSet<Tag> tags = null;
            //DbSet<Journey> journeys;
            //DbSet<Step> steps;

            //TagDetailsDto tagDetails;

            //tags.FromSqlInterpolated($"SELECT Tag.Id, Tag.TagNo FROM Tags INNER JOIN Step ON Tag.StepId = Step.Id WHERE Tag.Id = {request.Id}");



            var tagDetails = await _readOnlyContext
                .QuerySet<Tag>()
                .Where(tag => tag.Id == request.Id)
                .Join(
                    _readOnlyContext.QuerySet<Step>(),
                    tag => tag.StepId,
                    step => step.Id,
                    (tag, step) => new { tag, step.JourneyId })
                .Join(
                    _readOnlyContext.QuerySet<Journey>(),
                    x => x.StepId,
                    journey => journey.Id,
                    (x, journey) => new { x.
                .Select(x => new (tag, step. ))
                .FirstOrDefaultAsync();

            return new SuccessResult<TagDetailsDto>(tagDetails);
        }
    }
}

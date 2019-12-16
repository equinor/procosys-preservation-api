using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.Exceptions;
using MediatR;

namespace Equinor.Procosys.Preservation.Query.JourneyAggregate
{
    public class GetJourneyByIdQueryHandler : IRequestHandler<GetJourneyByIdQuery, JourneyDto>
    {
        private readonly IJourneyRepository _journeyRepository;

        public GetJourneyByIdQueryHandler(IJourneyRepository journeyRepository)
        {
            _journeyRepository = journeyRepository;
        }

        public async Task<JourneyDto> Handle(GetJourneyByIdQuery request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.Id);
            if (journey == null)
                throw new ProcosysEntityNotFoundException($"{nameof(Journey)} with ID {request.Id} not found");
            return new JourneyDto(journey.Id, journey.Title, journey.Steps.Select(step => new StepDto(step.Id, step.ModeId, step.ResponsibleId, step.Schema)));
        }
    }
}

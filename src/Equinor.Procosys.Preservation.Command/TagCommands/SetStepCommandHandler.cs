using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Equinor.Procosys.Preservation.Domain.Exceptions;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.TagCommands
{
    public class SetStepCommandHandler : IRequestHandler<SetStepCommand, Unit>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IJourneyRepository _journeyRepository;

        public SetStepCommandHandler(ITagRepository tagRepository, IJourneyRepository journeyRepository)
        {
            _tagRepository = tagRepository;
            _journeyRepository = journeyRepository;
        }

        public async Task<Unit> Handle(SetStepCommand request, CancellationToken cancellationToken)
        {
            Tag tag = await _tagRepository.GetByIdAsync(request.TagId) ?? throw new ProcosysEntityNotFoundException($"{nameof(Tag)} with ID {request.TagId} not found");
            Journey journey = await _journeyRepository.GetByIdAsync(request.JourneyId) ?? throw new ProcosysEntityNotFoundException($"{nameof(Journey)} with ID {request.JourneyId} not found");
            tag.SetStep(journey.Steps.FirstOrDefault(step => step.Id == request.StepId));
            await _tagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}

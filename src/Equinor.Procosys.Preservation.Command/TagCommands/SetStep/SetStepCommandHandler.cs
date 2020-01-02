using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.TagCommands.SetStep
{
    public class SetStepCommandHandler : IRequestHandler<SetStepCommand, Unit>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SetStepCommandHandler(ITagRepository tagRepository, IJourneyRepository journeyRepository, IUnitOfWork unitOfWork)
        {
            _tagRepository = tagRepository;
            _journeyRepository = journeyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(SetStepCommand request, CancellationToken cancellationToken)
        {
            var tag = await _tagRepository.GetByIdAsync(request.TagId);
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);
            tag.SetStep(journey.Steps.FirstOrDefault(step => step.Id == request.StepId));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}

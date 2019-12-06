using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using MediatR;

namespace Equinor.Procosys.Preservation.Command.TagCommands
{
    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, int>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IPlantProvider _plantProvider;

        public CreateTagCommandHandler(ITagRepository tagRepository, IJourneyRepository journeyRepository, IPlantProvider plantProvider)
        {
            _tagRepository = tagRepository;
            _journeyRepository = journeyRepository;
            _plantProvider = plantProvider;
        }

        public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            Journey journey = await _journeyRepository.GetByIdAsync(request.JourneyId);

            var tagToAdd = new Tag(_plantProvider.Plant, request.TagNo, request.ProjectNo, journey.Steps.FirstOrDefault(step => step.Id == request.StepId));
            _tagRepository.Add(tagToAdd);
            await _tagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return tagToAdd.Id;
        }
    }
}

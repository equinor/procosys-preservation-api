using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Equinor.Procosys.Preservation.MainApi;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.CreateTag
{
    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, Result<int>>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly ITagApiService _tagApiService;

        public CreateTagCommandHandler(
            ITagRepository tagRepository,
            IJourneyRepository journeyRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            ITagApiService tagApiService)
        {
            _tagRepository = tagRepository;
            _journeyRepository = journeyRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _tagApiService = tagApiService;
        }

        public async Task<Result<int>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            var journey = await _journeyRepository.GetByIdAsync(request.JourneyId);
            if (journey == null)
            {
                return new NotFoundResult<int>(Strings.EntityNotFound(nameof(Journey), request.JourneyId));
            }

            var tagDetails = await _tagApiService.GetTagDetails(_plantProvider.Plant, request.ProjectNo, request.TagNo);

            var tagToAdd = new Tag(
                _plantProvider.Plant,
                request.TagNo,
                request.ProjectNo,
                tagDetails.Description,
                tagDetails.AreaCode,
                tagDetails.CallOffNo,
                tagDetails.DisciplineCode,
                tagDetails.McPkgNo,
                tagDetails.PurchaseOrderNo,
                tagDetails.TagFunctionCode,
                journey.Steps.FirstOrDefault(step => step.Id == request.StepId));
            _tagRepository.Add(tagToAdd);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<int>(tagToAdd.Id);
        }
    }
}

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.StartPreservation
{
    public class StartPreservationCommandHandler : IRequestHandler<StartPreservationCommand, Result<Unit>>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IRequirementTypeRepository _requirementTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITimeService _timeService;

        public StartPreservationCommandHandler(ITagRepository tagRepository, IRequirementTypeRepository requirementTypeRepository, ITimeService timeService, IUnitOfWork unitOfWork)
        {
            _tagRepository = tagRepository;
            _requirementTypeRepository = requirementTypeRepository;
            _timeService = timeService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(StartPreservationCommand request, CancellationToken cancellationToken)
        {
            var tags = await _tagRepository.GetByIdsAsync(request.TagIds);
            foreach (var tag in tags)
            {
                var reqDefs = await _requirementTypeRepository
                    .GetRequirementDefinitionsByIdsAsync(tag.Requirements.Select(r => r.RequirementDefinitionId)
                        .ToList());
                foreach (var requirement in tag.Requirements)
                {
                    var reqDef = reqDefs.Single(r => r.Id == requirement.RequirementDefinitionId);

                    requirement.StartPreservation(_timeService.GetCurrentTimeUtc(), reqDef.NeedsUserInput);
                }

                tag.StartPreservation();
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}

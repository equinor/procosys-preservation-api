using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.Transfer
{
    public class TransferCommandHandler : IRequestHandler<TransferCommand, Result<IEnumerable<IdAndRowVersion>>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TransferCommandHandler(
            IProjectRepository projectRepository,
            IJourneyRepository journeyRepository,
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _journeyRepository = journeyRepository;
        }

        public async Task<Result<IEnumerable<IdAndRowVersion>>> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            var tags = await _projectRepository.GetTagsByTagIdsAsync(request.Tags.Select(x => x.Id));

            var stepIds = tags.Select(t => t.StepId).Distinct();
            var journeys = await _journeyRepository.GetJourneysByStepIdsAsync(stepIds);

            var tagsWithUpdatedRowVersion = new List<IdAndRowVersion>();

            foreach (var tag in tags)
            {
                var journey = journeys.Single(j => j.Steps.Any(s => s.Id == tag.StepId));
                tag.SetRowVersion(request.Tags.Single(x => x.Id == tag.Id).RowVersion);
                tag.Transfer(journey);
                tagsWithUpdatedRowVersion.Add(new IdAndRowVersion(tag.Id, tag.RowVersion.ConvertToString()));
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return new SuccessResult<IEnumerable<IdAndRowVersion>>(tagsWithUpdatedRowVersion);
        }
    }
}

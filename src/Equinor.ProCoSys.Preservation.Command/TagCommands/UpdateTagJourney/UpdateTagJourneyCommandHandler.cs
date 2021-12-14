using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTagJourney
{
    public class UpdateTagJourneyCommandHandler : IRequestHandler<UpdateTagJourneyCommand, Result<IEnumerable<IdAndRowVersion>>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IJourneyRepository _journeyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTagJourneyCommandHandler(
            IProjectRepository projectRepository, 
            IJourneyRepository journeyRepository, 
            IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _journeyRepository = journeyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<IdAndRowVersion>>> Handle(UpdateTagJourneyCommand request, CancellationToken cancellationToken)
        {
            var tags = await _projectRepository.GetTagsOnlyByTagIdsAsync(request.Tags.Select(x => x.Id));
            var step = await _journeyRepository.GetStepByStepIdAsync(request.StepId);

            foreach (var tag in tags)
            {
                tag.SetRowVersion(request.Tags.Single(x => x.Id == tag.Id).RowVersion);
                tag.UpdateStep(step);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<IEnumerable<IdAndRowVersion>>(tags.CreateIdAndRowVersionList());
        }
    }
}

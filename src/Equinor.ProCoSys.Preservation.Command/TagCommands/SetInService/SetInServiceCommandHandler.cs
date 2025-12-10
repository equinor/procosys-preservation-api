using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.SetInService
{
    public class SetInServiceCommandHandler : IRequestHandler<SetInServiceCommand, Result<IEnumerable<IdAndRowVersion>>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SetInServiceCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<IdAndRowVersion>>> Handle(SetInServiceCommand request, CancellationToken cancellationToken)
        {
            var tags = await _projectRepository.GetTagsWithPreservationHistoryByTagIdsAsync(request.Tags.Select(x => x.Id));

            foreach (var tag in tags)
            {
                tag.SetRowVersion(request.Tags.Single(x => x.Id == tag.Id).RowVersion);
                tag.SetInService();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<IEnumerable<IdAndRowVersion>>(tags.CreateIdAndRowVersionList());
        }
    }
}

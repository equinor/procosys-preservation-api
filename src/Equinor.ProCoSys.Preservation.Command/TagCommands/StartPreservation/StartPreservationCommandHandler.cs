using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.StartPreservation
{
    public class StartPreservationCommandHandler : IRequestHandler<StartPreservationCommand, Result<Unit>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StartPreservationCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(StartPreservationCommand request, CancellationToken cancellationToken)
        {
            var tags = await _projectRepository.GetTagsWithPreservationHistoryByTagIdsAsync(request.TagIds);
            
            foreach (var tag in tags)
            {
                tag.StartPreservation();
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.UndoStartPreservation
{
    public class UndoStartPreservationCommandHandler : IRequestHandler<UndoStartPreservationCommand, Result<Unit>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UndoStartPreservationCommandHandler(IProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(UndoStartPreservationCommand request, CancellationToken cancellationToken)
        {
            var tags = await _projectRepository.GetTagsWithPreservationHistoryByTagIdsAsync(request.TagIds);
            
            foreach (var tag in tags)
            {
                tag.UndoStartPreservation();
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}

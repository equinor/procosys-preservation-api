using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.StartPreservation
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
            var tags = await _projectRepository.GetTagsByTagIdsAsync(request.TagIds);
            foreach (var tag in tags)
            {
                tag.StartPreservation();
            }
            
            await _unitOfWork.SaveChangesAsync(request.CurrentUserOid, cancellationToken);
            
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}

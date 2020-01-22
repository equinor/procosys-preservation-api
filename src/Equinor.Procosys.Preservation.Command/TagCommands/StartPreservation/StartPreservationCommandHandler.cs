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
        private readonly ITimeService _timeService;

        public StartPreservationCommandHandler(IProjectRepository projectRepository, ITimeService timeService, IUnitOfWork unitOfWork)
        {
            _projectRepository = projectRepository;
            _timeService = timeService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(StartPreservationCommand request, CancellationToken cancellationToken)
        {
            var tags = await _projectRepository.GetTagsByTagIdsAsync(request.TagIds);
            foreach (var tag in tags)
            {
                tag.StartPreservation(_timeService.GetCurrentTimeUtc());
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}

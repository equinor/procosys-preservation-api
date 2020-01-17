using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.TagCommands.StartPreservation
{
    public class StartPreservationCommandHandler : IRequestHandler<StartPreservationCommand, Result<Unit>>
    {
        private readonly ITagRepository _tagRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITimeService _timeService;

        public StartPreservationCommandHandler(ITagRepository tagRepository, ITimeService timeService, IUnitOfWork unitOfWork)
        {
            _tagRepository = tagRepository;
            _timeService = timeService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(StartPreservationCommand request, CancellationToken cancellationToken)
        {
            var tags = await _tagRepository.GetByIdsAsync(request.TagIds);
            foreach (var tag in tags)
            {
                tag.Status = PreservationStatus.Active;
                foreach (var requirement in tag.Requirements)
                {
                    requirement.SetNextDueTimeUtc(_timeService.GetCurrentTimeUtc());
                }
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}

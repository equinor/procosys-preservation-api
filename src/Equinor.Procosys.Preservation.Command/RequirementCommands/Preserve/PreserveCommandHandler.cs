using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.RequirementCommands.Preserve
{
    public class PreserveCommandHandler : IRequestHandler<PreserveCommand, Result<Unit>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly ITimeService _timeService;

        public PreserveCommandHandler(
            IProjectRepository projectRepository,
            IPersonRepository personRepository,
            ITimeService timeService,
            IUnitOfWork unitOfWork,
            ICurrentUserProvider currentUserProvider)
        {
            _projectRepository = projectRepository;
            _personRepository = personRepository;
            _timeService = timeService;
            _unitOfWork = unitOfWork;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<Result<Unit>> Handle(PreserveCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagByTagIdAsync(request.TagId);
            var requirement = tag.Requirements.Single(r => r.Id == request.RequirementId);
            var currentUser = await _personRepository.GetByOidAsync(_currentUserProvider.GetCurrentUser());

            requirement.Preserve(_timeService.GetCurrentTimeUtc(), currentUser, false);
            tag.UpdateNextDueTimeUtc();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}

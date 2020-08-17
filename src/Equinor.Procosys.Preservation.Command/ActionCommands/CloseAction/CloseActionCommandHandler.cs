using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.Time;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ActionCommands.CloseAction
{
    public class CloseActionCommandHandler : IRequestHandler<CloseActionCommand, Result<Unit>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPersonRepository _personRepository;
        private readonly ICurrentUserProvider _currentUserProvider;

        public CloseActionCommandHandler(
            IProjectRepository projectRepository, 
            IUnitOfWork unitOfWork,
            IPersonRepository personRepository,
            ICurrentUserProvider currentUserProvider
            )
        {
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _personRepository = personRepository;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<Result<Unit>> Handle(CloseActionCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagByTagIdAsync(request.TagId);
            var currentUser = await _personRepository.GetByOidAsync(_currentUserProvider.GetCurrentUserOid());

            tag.CloseAction(request.ActionId, currentUser, TimeService.UtcNow, request.RowVersion);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}

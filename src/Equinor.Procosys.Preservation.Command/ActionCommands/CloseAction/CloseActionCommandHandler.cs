using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ActionCommands.CloseAction
{
    public class CloseActionCommandHandler : IRequestHandler<CloseActionCommand, Result<string>>
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

        public async Task<Result<string>> Handle(CloseActionCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagByTagIdAsync(request.TagId);
            var action = tag.Actions.Single(a => a.Id == request.ActionId);
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var currentUser = await _personRepository.GetByOidAsync(currentUserOid);
            action.Close(TimeService.UtcNow, currentUser);
            action.SetRowVersion(request.RowVersion);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(action.RowVersion.ToString());
        }
    }
}

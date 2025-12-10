using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.ActionCommands.CloseAction
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
            var tag = await _projectRepository.GetTagWithActionsByTagIdAsync(request.TagId);
            var currentUser = await _personRepository.GetByOidAsync(_currentUserProvider.GetCurrentUserOid());

            var action = tag.CloseAction(request.ActionId, currentUser, TimeService.UtcNow, request.RowVersion);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(action.RowVersion.ConvertToString());
        }
    }
}

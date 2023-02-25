using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Misc;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.Preserve
{
    public class PreserveCommandHandler : IRequestHandler<PreserveCommand, Result<Unit>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUserProvider;

        public PreserveCommandHandler(
            IProjectRepository projectRepository,
            IPersonRepository personRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserProvider currentUserProvider)
        {
            _projectRepository = projectRepository;
            _personRepository = personRepository;
            _unitOfWork = unitOfWork;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<Result<Unit>> Handle(PreserveCommand request, CancellationToken cancellationToken)
        {
            var tag = await _projectRepository.GetTagWithPreservationHistoryByTagIdAsync(request.TagId);
            var currentUser = await _personRepository.GetByOidAsync(_currentUserProvider.GetCurrentUserOid());

            tag.Preserve(currentUser);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}

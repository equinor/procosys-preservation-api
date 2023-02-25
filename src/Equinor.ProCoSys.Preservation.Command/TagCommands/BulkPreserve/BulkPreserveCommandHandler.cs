using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Misc;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.TagCommands.BulkPreserve
{
    public class BulkPreserveCommandHandler : IRequestHandler<BulkPreserveCommand, Result<Unit>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUserProvider;

        public BulkPreserveCommandHandler(
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

        public async Task<Result<Unit>> Handle(BulkPreserveCommand request, CancellationToken cancellationToken)
        {
            var tags = await _projectRepository.GetTagsWithPreservationHistoryByTagIdsAsync(request.TagIds);
            var currentUser = await _personRepository.GetByOidAsync(_currentUserProvider.GetCurrentUserOid());

            foreach (var tag in tags)
            {
                tag.BulkPreserve(currentUser);
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}

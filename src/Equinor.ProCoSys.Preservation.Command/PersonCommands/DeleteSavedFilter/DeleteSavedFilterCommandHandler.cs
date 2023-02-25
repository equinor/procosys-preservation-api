using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Misc;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.PersonCommands.DeleteSavedFilter
{
    public class DeleteSavedFilterCommandHandler : IRequestHandler<DeleteSavedFilterCommand, Result<Unit>>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUserProvider;

        public DeleteSavedFilterCommandHandler(
            IPersonRepository personRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserProvider currentUserProvider)
        {
            _personRepository = personRepository;
            _unitOfWork = unitOfWork;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<Result<Unit>> Handle(DeleteSavedFilterCommand request, CancellationToken cancellationToken)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var person = await _personRepository.GetWithSavedFiltersByOidAsync(currentUserOid);
            var savedFilter = person.SavedFilters.Single(sf => sf.Id == request.SavedFilterId);

            savedFilter.SetRowVersion(request.RowVersion);
            person.RemoveSavedFilter(savedFilter);
            _personRepository.RemoveSavedFilter(savedFilter);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}

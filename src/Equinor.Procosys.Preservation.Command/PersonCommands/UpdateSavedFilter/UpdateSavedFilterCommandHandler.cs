using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.PersonCommands.UpdateSavedFilter
{
    public class UpdateSavedFilterCommandHandler : IRequestHandler<UpdateSavedFilterCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IPersonRepository _personRepository;

        public UpdateSavedFilterCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserProvider currentUserProvider,
            IPersonRepository personRepository)
        {
            _unitOfWork = unitOfWork;
            _currentUserProvider = currentUserProvider;
            _personRepository = personRepository;
        }

        public async Task<Result<string>> Handle(UpdateSavedFilterCommand request, CancellationToken cancellationToken)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var person = await _personRepository.GetWithSavedFiltersByOidAsync(currentUserOid);
            var savedFilter = person.SavedFilters.Single(sf => sf.Id == request.SavedFilterId);

            if (request.DefaultFilter == true)
            {
                var currentDefaultFiler = person.GetDefaultFilter(savedFilter.ProjectId);
                if (currentDefaultFiler != null)
                {
                    currentDefaultFiler.DefaultFilter = false;
                }
            }

            savedFilter.Title = request.Title;
            savedFilter.Criteria = request.Criteria;
            if (request.DefaultFilter.HasValue)
            {
                savedFilter.DefaultFilter = request.DefaultFilter.Value;
            }
            savedFilter.SetRowVersion(request.RowVersion);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SuccessResult<string>(savedFilter.RowVersion.ConvertToString());
        }
    }
}

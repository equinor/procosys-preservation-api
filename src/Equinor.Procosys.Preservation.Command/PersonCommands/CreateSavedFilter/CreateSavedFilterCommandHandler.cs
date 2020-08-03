using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.PersonCommands.CreateSavedFilter;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.ActionCommands.CreateAction
{
    public class CreateSavedFilterCommandHandler : IRequestHandler<CreateSavedFilterCommand, Result<int>>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlantProvider _plantProvider;
        private readonly ICurrentUserProvider _currentUserProvider;

        public CreateSavedFilterCommandHandler(
            IPersonRepository personRepository,
            IUnitOfWork unitOfWork,
            IPlantProvider plantProvider,
            ICurrentUserProvider currentUserProvider)
        {
            _personRepository = personRepository;
            _unitOfWork = unitOfWork;
            _plantProvider = plantProvider;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<Result<int>> Handle(CreateSavedFilterCommand request, CancellationToken cancellationToken)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var person = await _personRepository.GetByOidAsync(currentUserOid);

            var savedFilter = new SavedFilter(_plantProvider.Plant, request.Title, request.Criteria);
            person.AddSavedFilter(savedFilter);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new SuccessResult<int>(savedFilter.Id);
        }
    }
}

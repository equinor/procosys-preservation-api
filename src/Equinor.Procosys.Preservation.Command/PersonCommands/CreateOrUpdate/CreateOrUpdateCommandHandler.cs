using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.PersonCommands.CreateOrUpdate
{
    public class CreateOrUpdateCommandHandler : IRequestHandler<CreateOrUpdatePersonCommand, Result<Unit>>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrUpdateCommandHandler(IPersonRepository personRepository, IUnitOfWork unitOfWork)
        {
            _personRepository = personRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(CreateOrUpdatePersonCommand request, CancellationToken cancellationToken)
        {
            var save = false;
            var person = await _personRepository.GetByOidAsync(request.Oid);

            if (person == null)
            {
                person = new Person(request.Oid, request.FirstName, request.LastName);
                _personRepository.Add(person);
                save = true;
            }
            else if (person.FirstName != request.FirstName || person.LastName != request.LastName)
            {
                person.FirstName = request.FirstName;
                person.LastName = request.LastName;
                save = true;
            }

            if (save)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}

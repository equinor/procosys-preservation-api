using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.PersonCommands.CreatePerson
{
    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Result<Unit>>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePersonCommandHandler(IPersonRepository personRepository, IUnitOfWork unitOfWork)
        {
            _personRepository = personRepository;
            _unitOfWork = unitOfWork;
        }

        // todo write unit test after review ok
        public async Task<Result<Unit>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var person = await _personRepository.GetByOidAsync(request.Oid);

            if (person == null)
            {
                person = new Person(request.Oid, request.FirstName, request.LastName);
                _personRepository.Add(person);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}

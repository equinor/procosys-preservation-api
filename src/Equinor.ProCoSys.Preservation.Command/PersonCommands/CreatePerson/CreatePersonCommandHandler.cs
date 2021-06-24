using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Person;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.PersonCommands.CreatePerson
{
    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Result<Unit>>
    {
        private readonly IPlantProvider _plantProvider;
        private readonly IPersonApiService _personApiService;
        private readonly IPersonRepository _personRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePersonCommandHandler(
            IPlantProvider plantProvider, 
            IPersonApiService personApiService,
            IPersonRepository personRepository,
            IUnitOfWork unitOfWork)
        {
            _plantProvider = plantProvider;
            _personApiService = personApiService;
            _personRepository = personRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var person = await _personRepository.GetByOidAsync(request.Oid);

            if (person == null)
            {
                var pcsPersons = await _personApiService.GetPersonsByOidsAsync(_plantProvider.Plant,
                    new List<string> {request.Oid.ToString("D")});
                if (pcsPersons == null || pcsPersons.Count != 1)
                {
                    return new NotFoundResult<Unit>($"Details for user with oid {request.Oid:D} not found in ProCoSys");
                }
                person = new Person(request.Oid, pcsPersons.Single().FirstName, pcsPersons.Single().LastName);
                _personRepository.Add(person);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            
            return new SuccessResult<Unit>(Unit.Value);
        }
    }
}

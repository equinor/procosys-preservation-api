using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Equinor.ProCoSys.Preservation.MainApi.Person;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Command.PersonCommands.CreatePerson
{
    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, Result<Unit>>
    {
        private readonly IAuthenticator _authenticator;
        private readonly IPersonApiService _personApiService;
        private readonly IPersonRepository _personRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePersonCommandHandler(
            IAuthenticator authenticator,
            IPersonApiService personApiService,
            IPersonRepository personRepository,
            IUnitOfWork unitOfWork
            )
        {
            _authenticator = authenticator;
            _personApiService = personApiService;
            _personRepository = personRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Unit>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var person = await _personRepository.GetByOidAsync(request.Oid);

            if (person == null)
            {
                var pcsPerson = await TryGetPersonDetailsAsync(request.Oid.ToString("D"));
                if (pcsPerson == null)
                {
                    throw new Exception($"Details for user with oid {request.Oid:D} not found in ProCoSys");
                }
                person = new Person(request.Oid, pcsPerson.FirstName, pcsPerson.LastName);
                _personRepository.Add(person);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            
            return new SuccessResult<Unit>(Unit.Value);
        }

        private async Task<PCSPerson> TryGetPersonDetailsAsync(string azureOid)
        {
            var oldAuthType = _authenticator.AuthenticationType;
            _authenticator.AuthenticationType = AuthenticationType.AsApplication;
            try
            {
                return await _personApiService.TryGetPersonByOidAsync(azureOid);
            }
            finally
            {
                _authenticator.AuthenticationType = oldAuthType;
            }
        }
    }
}

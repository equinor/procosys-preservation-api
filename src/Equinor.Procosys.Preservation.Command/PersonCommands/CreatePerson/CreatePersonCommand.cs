using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.PersonCommands.CreatePerson
{
    public class CreatePersonCommand : IRequest<Result<Unit>>
    {
        public CreatePersonCommand(Guid oid, string firstName, string lastName)
        {
            Oid = oid;
            FirstName = firstName;
            LastName = lastName;
        }

        public Guid Oid { get; }
        public string FirstName { get; }
        public string LastName { get; }
    }
}

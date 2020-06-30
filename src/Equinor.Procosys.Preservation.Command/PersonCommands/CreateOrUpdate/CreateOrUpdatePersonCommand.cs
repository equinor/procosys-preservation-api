using System;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Command.PersonCommands.CreateOrUpdate
{
    public class CreateOrUpdatePersonCommand : IRequest<Result<Unit>>
    {
        public CreateOrUpdatePersonCommand(Guid oid, string firstName, string lastName)
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

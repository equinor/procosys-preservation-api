using System;

namespace Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate
{
    public class Person : EntityBase, IAggregateRoot
    {
        public const int FirstNameLengthMax = 64;
        public const int LastNameLengthMax = 64;

        protected Person() : base()
        {
        }

        public Person(Guid oid, string firstName, string lastName) : base()
        {
            Oid = oid;
            FirstName = firstName;
            LastName = lastName;
        }

        public Guid Oid { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
    }
}

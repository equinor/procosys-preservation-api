using System;
using System.Collections.Generic;
using System.Text;

namespace Equinor.Procosys.Preservation.Query
{
    public class PersonDto
    {
        public PersonDto(int id, string firstName, string lastName)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }

        public int Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
    }
}

using System;

namespace Equinor.Procosys.Preservation.Domain.Exceptions
{
    public class NotFoundException : ProcosysNotFoundException
    {
        public NotFoundException()
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

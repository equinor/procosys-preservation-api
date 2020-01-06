using System;

namespace Equinor.Procosys.Preservation.Domain.Exceptions
{
    public class ProcosysNotFoundException : Exception
    {
        public ProcosysNotFoundException()
        {
        }

        public ProcosysNotFoundException(string message)
            : base(message)
        {
        }

        public ProcosysNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

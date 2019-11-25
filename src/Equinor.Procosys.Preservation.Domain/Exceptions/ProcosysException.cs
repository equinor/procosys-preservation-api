using System;

namespace Equinor.Procosys.Preservation.Domain.Exceptions
{
    public class ProcosysException : Exception
    {
        public ProcosysException()
        {
        }

        public ProcosysException(string message)
            : base(message)
        {
        }

        public ProcosysException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

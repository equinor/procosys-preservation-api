using System;

namespace Equinor.Procosys.Preservation.Domain.Exceptions
{
    public class ProcosysEntityValidationException : ProcosysException
    {
        public ProcosysEntityValidationException()
        {
        }

        public ProcosysEntityValidationException(string message) : base(message)
        {
        }

        public ProcosysEntityValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

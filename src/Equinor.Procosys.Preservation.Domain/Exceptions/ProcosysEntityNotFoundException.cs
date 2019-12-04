using System;

namespace Equinor.Procosys.Preservation.Domain.Exceptions
{
    public class ProcosysEntityNotFoundException : ProcosysException
    {
        public ProcosysEntityNotFoundException()
        {
        }

        public ProcosysEntityNotFoundException(string message) : base(message)
        {
        }

        public ProcosysEntityNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

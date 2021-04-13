using System;

namespace Equinor.ProCoSys.Preservation.MainApi.Exceptions
{
    public class InvalidResultException : Exception
    {
        public InvalidResultException(string message) : base(message)
        {
        }
    }
}

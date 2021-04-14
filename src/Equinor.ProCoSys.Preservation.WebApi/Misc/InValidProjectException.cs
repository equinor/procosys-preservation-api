using System;

namespace Equinor.ProCoSys.Preservation.WebApi.Misc
{
    public class InValidProjectException : Exception
    {
        public InValidProjectException(string error) : base(error)
        {
        }
    }
}

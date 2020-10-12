using System;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public class InValidProjectException : Exception
    {
        public InValidProjectException(string error) : base(error)
        {
        }
    }
}

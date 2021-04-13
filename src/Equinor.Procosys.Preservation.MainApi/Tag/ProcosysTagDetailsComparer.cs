using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.MainApi.Tag
{
    public class ProcosysTagDetailsComparer : IEqualityComparer<ProcosysTagDetails>
    {
        public bool Equals(ProcosysTagDetails d1, ProcosysTagDetails d2)
        {
            if (d2 == null && d1 == null)
            {
                return true;
            }

            if (d1 == null || d2 == null)
            {
                return false;
            }

            if (d1.RegisterCode == d2.RegisterCode && d1.TagFunctionCode == d2.TagFunctionCode)
            {
                return true;
            }
                
            return false;
        }

        public int GetHashCode(ProcosysTagDetails d)
        {
            var hCode = d.RegisterCode.GetHashCode() ^ d.TagFunctionCode.GetHashCode();
            return hCode.GetHashCode();
        }
    }
}

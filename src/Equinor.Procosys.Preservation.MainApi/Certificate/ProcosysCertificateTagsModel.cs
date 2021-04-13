using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.MainApi.Certificate
{
    public class ProcosysCertificateTagsModel
    {
        public bool CertificateIsAccepted { get; set; }
        public IEnumerable<ProcosysCertificateTag> Tags { get; set; }
    }
}

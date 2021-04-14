using System.Collections.Generic;

namespace Equinor.ProCoSys.Preservation.MainApi.Certificate
{
    public class PCSCertificateTagsModel
    {
        public bool CertificateIsAccepted { get; set; }
        public IEnumerable<PCSCertificateTag> Tags { get; set; }
    }
}

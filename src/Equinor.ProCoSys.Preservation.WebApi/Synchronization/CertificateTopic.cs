using Equinor.ProCoSys.PcsServiceBus.Enums;

namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    public class CertificateTopic
    {
        public string TopicName { get; set; } = "certificate";

        public string Plant { get; set; }

        public string ProjectName { get; set; }

        public string CertificateNo { get; set; }

        public string CertificateType { get; set; }

        public CertificateStatus CertificateStatus { get; set; }
    }
}

using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    public interface ICertificateEventProcessorService
    {
        Task ProcessCertificateEvent(string messageJson);
    }
}

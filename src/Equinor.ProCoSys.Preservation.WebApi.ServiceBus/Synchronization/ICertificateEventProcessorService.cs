using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.WebApi.ServiceBus.Synchronization
{
    public interface ICertificateEventProcessorService
    {
        Task ProcessCertificateEventAsync(string messageJson);
    }
}

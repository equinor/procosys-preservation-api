using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.WebApi.Synchronization
{
    public interface ICertificateEventProcessorService
    {
        Task ProcessCertificateEventAsync(string messageJson);
    }
}

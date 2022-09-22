using System;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.MainApi.Certificate
{
    public interface ICertificateApiService
    {
        Task<PCSCertificateTagsModel> TryGetCertificateTagsAsync(string plant, Guid proCoSysGuid);
    }
}

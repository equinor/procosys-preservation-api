﻿using System.Threading.Tasks;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Synchronization
{
    public interface ICertificateEventProcessorService
    {
        Task ProcessCertificateEventAsync(string messageJson);
    }
}

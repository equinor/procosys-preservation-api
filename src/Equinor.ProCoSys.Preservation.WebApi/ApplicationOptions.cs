using System;

namespace Equinor.ProCoSys.Preservation.WebApi
{
    /// <summary>
    /// Options for the application. Read from application configuration via IOptionsMonitor.
    /// "Mapped" to the generic IAuthenticatorOptions
    /// </summary>
    public class ApplicationOptions
    {
        public Guid ObjectId { get; set; }
    }
}

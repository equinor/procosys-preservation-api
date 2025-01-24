using System;

namespace Equinor.ProCoSys.Preservation.WebApi.Authentication
{
    /// <summary>
    /// Options for Authentication. Read from application configuration via IOptionsMonitor.
    /// "Mapped" to the generic IAuthenticatorOptions
    /// </summary>
    public class PreservationAuthenticatorOptions
    {
        // TODO 1064: Refactor this. This can probably be consolidated into another config section.
        public Guid PreservationApiObjectId { get; set; }
    }
}

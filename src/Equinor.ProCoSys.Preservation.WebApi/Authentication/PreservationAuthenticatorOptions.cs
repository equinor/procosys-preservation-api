using System;

namespace Equinor.ProCoSys.Preservation.WebApi.Authentication
{
    /// <summary>
    /// Options for Authentication. Read from application configuration via IOptionsMonitor.
    /// "Mapped" to the generic IAuthenticatorOptions
    /// </summary>
    public class PreservationAuthenticatorOptions
    {
        public string Instance { get; set; }

        public string PreservationApiClientId { get; set; }
        public Guid PreservationApiObjectId { get; set; }
        public string PreservationApiSecret { get; set; }

        public bool DisableProjectUserDataClaims { get; set; }
        public bool DisableRestrictionRoleUserDataClaims { get; set; }

        public string MainApiScope { get; set; }
    }
}

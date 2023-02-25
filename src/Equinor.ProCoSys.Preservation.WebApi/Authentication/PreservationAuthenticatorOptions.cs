using System;

namespace Equinor.ProCoSys.Preservation.WebApi.Authentication
{
    public class PreservationAuthenticatorOptions
    {
        public string Instance { get; set; }

        public string PreservationApiClientId { get; set; }
        public Guid PreservationApiObjectId { get; set; }
        public string PreservationApiSecret { get; set; }

        public string MainApiScope { get; set; }
    }
}

namespace Equinor.ProCoSys.Preservation.WebApi.Authentication
{
    public class AuthenticatorOptions
    {
        public string Instance { get; set; }

        public string PreservationApiClientId { get; set; }
        public string PreservationApiSecret { get; set; }

        public string MainApiClientId { get; set; }
        public string MainApiSecret { get; set; }
        public string MainApiScope { get; set; }
    }
}

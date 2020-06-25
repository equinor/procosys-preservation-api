namespace Equinor.Procosys.Preservation.WebApi.Authentication
{
    public class AuthenticatorOptions
    {
        public string Instance { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string MainApiScope { get; set; }
        public string PreservationApiScope { get; set; }
    }
}

namespace Equinor.Procosys.Preservation.WebApi.Synchronization
{
    public class AuthenticatorOptions
    {
        public string Instance { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string[] Scopes { get; set; }
    }
}

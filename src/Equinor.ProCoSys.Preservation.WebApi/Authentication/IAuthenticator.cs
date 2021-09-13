namespace Equinor.ProCoSys.Preservation.WebApi.Authentication
{
    public interface IAuthenticator
    {
        AuthenticationType AuthenticationType { get; set; }
    }
}

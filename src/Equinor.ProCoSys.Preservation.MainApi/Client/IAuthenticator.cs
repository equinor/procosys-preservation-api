namespace Equinor.ProCoSys.Preservation.MainApi.Client
{
    public interface IAuthenticator
    {
        AuthenticationType AuthenticationType { get; set; }
    }
}

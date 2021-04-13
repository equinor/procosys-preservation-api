namespace Equinor.ProCoSys.Preservation.WebApi.Misc
{
    public interface IBearerTokenSetter
    {
        void SetBearerToken(string token, bool isUserToken = true);
    }
}

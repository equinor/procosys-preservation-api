namespace Equinor.ProCoSys.Preservation.WebApi.Authorizations
{
    public interface ICrossPlantAccessChecker
    {
        bool HasCurrentUserAccessToCrossPlant();
    }
}

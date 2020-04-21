namespace Equinor.Procosys.Preservation.WebApi.Authorizations
{
    public interface IPlantAccessChecker
    {
        bool HasCurrentUserAccessToPlant(string plantId);
    }
}

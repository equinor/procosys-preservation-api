namespace Equinor.Procosys.Preservation.Domain
{
    public interface IPlantAccessChecker
    {
        bool HasCurrentUserAccessToPlant(string plantId);
    }
}

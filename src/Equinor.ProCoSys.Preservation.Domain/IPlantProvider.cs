namespace Equinor.ProCoSys.Preservation.Domain
{
    public interface IPlantProvider
    {
        string Plant { get; }
        bool IsCrossPlantQuery { get; }
    }
}

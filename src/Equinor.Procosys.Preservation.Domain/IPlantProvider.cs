namespace Equinor.Procosys.Preservation.Domain
{
    public interface IPlantProvider
    {
        string Plant { get; }
        void SetTemporaryPlant(string plant);
        void ReleaseTemporaryPlant();
    }
}

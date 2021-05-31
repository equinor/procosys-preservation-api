namespace Equinor.ProCoSys.Preservation.Domain
{
    public interface IPlantSetter
    {
        void SetPlant(string plant);
        void SetCrossPlantQuery();
        void ClearCrossPlantQuery();
    }
}

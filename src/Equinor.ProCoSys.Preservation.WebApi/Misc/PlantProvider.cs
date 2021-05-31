using Equinor.ProCoSys.Preservation.Domain;

namespace Equinor.ProCoSys.Preservation.WebApi.Misc
{
    public class PlantProvider : IPlantProvider, IPlantSetter
    {
        public string Plant { get; private set; }
        public bool IsCrossPlantQuery { get; private set; }

        public void SetPlant(string plant) => Plant = plant;
        public void SetCrossPlantQuery() => IsCrossPlantQuery = true;
        public void ClearCrossPlantQuery() => IsCrossPlantQuery = false;
    }
}

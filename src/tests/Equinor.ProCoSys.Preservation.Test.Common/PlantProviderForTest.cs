using Equinor.ProCoSys.Common.Misc;

namespace Equinor.ProCoSys.Preservation.Test.Common
{
    public class PlantProviderForTest : IPlantProvider, IPlantSetter
    {
        public PlantProviderForTest(string plant) => Plant = plant;

        public string Plant { get; private set; }
        public bool IsCrossPlantQuery { get; private set; }

        public void SetPlant(string plant) => Plant = plant;
        public void SetCrossPlantQuery() => IsCrossPlantQuery = true;
        public void ClearCrossPlantQuery() => IsCrossPlantQuery = false;
    }
}

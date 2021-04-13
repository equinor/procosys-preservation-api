using Equinor.ProCoSys.Preservation.Domain;

namespace Equinor.ProCoSys.Preservation.WebApi.Misc
{
    public class PlantProvider : IPlantProvider, IPlantSetter
    {
        public string Plant { get; private set; }

        public void SetPlant(string plant) => Plant = plant;
    }
}

using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.WebApi.Misc
{
    public class PlantProvider : IPlantProvider, IPlantSetter
    {
        public string Plant { get; private set; }

        public void SetPlant(string plant) => Plant = plant;
    }
}

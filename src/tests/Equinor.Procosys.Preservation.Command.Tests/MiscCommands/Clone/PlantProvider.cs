using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Command.Tests.MiscCommands.Clone
{
    public class PlantProvider : IPlantProvider, IPlantSetter
    {
        public PlantProvider(string plant) => Plant = plant;

        public string Plant { get; private set; }

        public void SetPlant(string plant) => Plant = plant;
    }
}

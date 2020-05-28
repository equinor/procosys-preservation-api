using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Command.Tests.MiscCommands.Clone
{
    public class PlantProvider : IPlantProvider
    {
        private string _temporaryPlant;
        private string _plant;

        public PlantProvider(string plant) => _plant = plant;

        public string Plant
        {
            get
            {
                if (_temporaryPlant != null)
                {
                    return _temporaryPlant;
                }
                return _plant;
            }
        }

        public void SetTemporaryPlant(string plant) => _temporaryPlant = plant;

        public void ReleaseTemporaryPlant() => _temporaryPlant = null;
    }
}

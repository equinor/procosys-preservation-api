using System;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Command.Tests.MiscCommands.Clone
{
    public class PlantProvider : IPlantProvider, IPlantSetter
    {
        public const string PlantHeader = "x-plant";
        private string _plant;

        public PlantProvider(string plant)
        {
            _plant = plant;
        }

        public string Plant => _plant ?? throw new Exception("Could not determine current plant");

        public void SetPlant(string plant)
        {
            _plant = plant;
        }
    }
}

using System;
using Equinor.ProCoSys.Preservation.Domain;

namespace Equinor.ProCoSys.Preservation.Command.Tests.MiscCommands.Clone
{
    public class PlantProvider : IPlantProvider, IPlantSetter
    {
        public PlantProvider(string plant) => Plant = plant;

        public string Plant { get; private set; }
        public bool IsCrossPlantQuery => throw new NotImplementedException();

        public void SetPlant(string plant) => Plant = plant;
        public void SetCrossPlantQuery() => throw new NotImplementedException();
        public void ClearCrossPlantQuery() => throw new NotImplementedException();
    }
}

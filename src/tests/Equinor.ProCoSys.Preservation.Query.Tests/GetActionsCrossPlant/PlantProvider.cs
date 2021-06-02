using System;
using Equinor.ProCoSys.Preservation.Domain;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetActionsCrossPlant
{
    public class PlantProvider : IPlantProvider, IPlantSetter
    {
        public string Plant => null;
        public bool IsCrossPlantQuery { get; private set; }

        public void SetPlant(string plant) => throw new NotImplementedException();
        public void SetCrossPlantQuery() => IsCrossPlantQuery = true;
        public void ClearCrossPlantQuery() => IsCrossPlantQuery = false;
    }
}

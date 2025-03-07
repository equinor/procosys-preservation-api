using Equinor.ProCoSys.Common.Misc;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Seeding
{
    public class SeedingPlantProvider : IPlantProvider
    {
        public SeedingPlantProvider(string plant) => Plant = plant;

        public string Plant { get; }
        public bool IsCrossPlantQuery => false;
    }
}

using Equinor.ProCoSys.Auth.Misc;

namespace Equinor.ProCoSys.Preservation.WebApi.Seeding
{
    public class SeedingPlantProvider : IPlantProvider
    {
        public SeedingPlantProvider(string plant) => Plant = plant;

        public string Plant { get; }
        public bool IsCrossPlantQuery => false;
    }
}

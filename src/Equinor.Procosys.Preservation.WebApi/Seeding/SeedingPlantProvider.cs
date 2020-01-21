using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.WebApi.Seeding
{
    public class SeedingPlantProvider : IPlantProvider
    {
        public SeedingPlantProvider(string plant)
        {
            Plant = plant;
        }

        public string Plant { get; }
    }
}

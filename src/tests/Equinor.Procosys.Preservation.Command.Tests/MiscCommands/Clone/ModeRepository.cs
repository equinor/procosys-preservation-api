using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;

namespace Equinor.Procosys.Preservation.Command.Tests.MiscCommands.Clone
{
    internal class ModeRepository : TestRepository<Mode>, IModeRepository
    {
        public ModeRepository(PlantProvider plantProvider, List<Mode> sourceModes)
            :base(plantProvider, sourceModes)
        {
        }
    }
}

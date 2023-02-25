using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Test.Common;

namespace Equinor.ProCoSys.Preservation.Command.Tests.MiscCommands.Clone
{
    internal class ModeRepository : TestRepository<Mode>, IModeRepository
    {
        public ModeRepository(PlantProviderForTest plantProvider, List<Mode> sourceModes)
            :base(plantProvider, sourceModes)
        {
        }
    }
}

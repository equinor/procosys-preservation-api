﻿using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;

namespace Equinor.ProCoSys.Preservation.Command.Tests.MiscCommands.Clone
{
    internal class ModeRepository : TestRepository<Mode>, IModeRepository
    {
        public ModeRepository(PlantProvider plantProvider, List<Mode> sourceModes)
            :base(plantProvider, sourceModes)
        {
        }
    }
}

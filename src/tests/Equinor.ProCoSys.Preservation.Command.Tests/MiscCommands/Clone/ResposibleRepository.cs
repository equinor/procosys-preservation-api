using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Test.Common;

namespace Equinor.ProCoSys.Preservation.Command.Tests.MiscCommands.Clone
{
    internal class ResponsibleRepository : TestRepository<Responsible>, IResponsibleRepository
    {

        public ResponsibleRepository(PlantProviderForTest plantProvider, List<Responsible> sourceResponsibles)
            : base(plantProvider, sourceResponsibles)
        {
        }

        public Task<Responsible> GetByCodeAsync(string responsibleCode) => throw new NotImplementedException();
    }
}

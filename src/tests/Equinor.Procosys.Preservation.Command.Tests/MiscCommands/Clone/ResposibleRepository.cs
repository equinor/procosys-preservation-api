using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.Procosys.Preservation.Command.Tests.MiscCommands.Clone
{
    internal class ResponsibleRepository : TestRepository<Responsible>, IResponsibleRepository
    {

        public ResponsibleRepository(PlantProvider plantProvider, List<Responsible> sourceResponsibles)
            :base(plantProvider, sourceResponsibles)
        {
        }

        public Task<Responsible> GetByCodeAsync(string responsibleCode) => throw new NotImplementedException();
    }
}

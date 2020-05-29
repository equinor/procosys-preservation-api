using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;

namespace Equinor.Procosys.Preservation.Command.Tests.MiscCommands.Clone
{
    internal class TagFunctionRepository : TestRepository<TagFunction>, ITagFunctionRepository
    {
        public TagFunctionRepository(PlantProvider plantProvider, List<TagFunction> sourceTagFunctions)
            :base(plantProvider, sourceTagFunctions)
        {
        }

        public Task<TagFunction> GetByCodesAsync(string tagFunctionCode, string registerCode) => throw new NotImplementedException();
        public Task<List<TagFunction>> GetAllNonVoidedWithRequirementsAsync() => throw new NotImplementedException();
    }
}

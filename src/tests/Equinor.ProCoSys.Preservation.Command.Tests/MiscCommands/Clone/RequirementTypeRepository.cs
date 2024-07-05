using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Test.Common;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;

namespace Equinor.ProCoSys.Preservation.Command.Tests.MiscCommands.Clone
{
    internal class RequirementTypeRepository : TestRepository<RequirementType>, IRequirementTypeRepository
    {
        public RequirementTypeRepository(PlantProviderForTest plantProvider, List<RequirementType> sourceRequirementTypes)
            :base(plantProvider, sourceRequirementTypes)
        {
        }
        
        public void Save()
        {
            var requirementDefinitions = _targetItems
                .SelectMany(rt => rt.RequirementDefinitions)
                .ToList();
            var nextId = requirementDefinitions.Count;
            foreach (var rd in requirementDefinitions.Where(rd => rd.Id == 0))
            {
                rd.SetProtectedIdForTesting(++nextId);
            }
        }

        public Task<RequirementDefinition> GetRequirementDefinitionByFieldGuidAsync(Guid fieldGuid) => throw new NotImplementedException();
        public Task<RequirementDefinition> GetRequirementDefinitionByIdAsync(int requirementDefinitionId) => throw new NotImplementedException();
        public Task<List<RequirementDefinition>> GetRequirementDefinitionsByIdsAsync(IList<int> requirementDefinitionIds) => throw new NotImplementedException();
        public Task<RequirementType> GetRequirementTypeByRequirementDefinitionGuidAsync(Guid requirementDefinitionGuid) => throw new NotImplementedException();
        public void RemoveRequirementDefinition(RequirementDefinition requirementDefinition) => throw new NotImplementedException();
        public void RemoveField(Field field) => throw new NotImplementedException();
    }
}

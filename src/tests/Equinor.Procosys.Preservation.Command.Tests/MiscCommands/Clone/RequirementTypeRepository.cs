using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;

namespace Equinor.Procosys.Preservation.Command.Tests.MiscCommands.Clone
{
    internal class RequirementTypeRepository : TestRepository<RequirementType>, IRequirementTypeRepository
    {
        public RequirementTypeRepository(PlantProvider plantProvider, List<RequirementType> sourceRequirementTypes)
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

        public Task<RequirementDefinition> GetRequirementDefinitionByIdAsync(int requirementDefinitionId) => throw new NotImplementedException();
        public Task<List<RequirementDefinition>> GetRequirementDefinitionsByIdsAsync(IList<int> requirementDefinitionIds) => throw new NotImplementedException();
        public void RemoveRequirementDefinition(RequirementDefinition requirementDefinition) => throw new NotImplementedException();
    }
}

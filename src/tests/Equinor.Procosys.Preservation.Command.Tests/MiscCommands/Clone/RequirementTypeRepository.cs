using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;

namespace Equinor.Procosys.Preservation.Command.Tests.MiscCommands.Clone
{
    internal class RequirementTypeRepository : IRequirementTypeRepository
    {
        private readonly PlantProvider _plantProvider;
        private readonly List<RequirementType> _sourceRequirementTypes;
        private readonly List<RequirementType> _targetRequirementTypes = new List<RequirementType>();
        private readonly string _targetPlant;

        public RequirementTypeRepository(PlantProvider plantProvider, List<RequirementType> sourceRequirementTypes)
        {
            _plantProvider = plantProvider;
            _sourceRequirementTypes = sourceRequirementTypes;
            _targetPlant = plantProvider.Plant;
        }

        public void Add(RequirementType item)
        {
            if (_plantProvider.Plant == _targetPlant)
            {
                _targetRequirementTypes.Add(item);
            }
            else
            {
                _sourceRequirementTypes.Add(item);
            }
        }

        public Task<List<RequirementType>> GetAllAsync()
        {
            if (_plantProvider.Plant == _targetPlant)
            {
                return Task.FromResult(_targetRequirementTypes);
            }
            return Task.FromResult(_sourceRequirementTypes);
        }

        public Task<bool> Exists(int id) => throw new NotImplementedException();

        public Task<RequirementType> GetByIdAsync(int id) => throw new NotImplementedException();

        public Task<List<RequirementType>> GetByIdsAsync(IEnumerable<int> id) => throw new NotImplementedException();

        public void Remove(RequirementType entity) => throw new NotImplementedException();

        public Task<RequirementType> GetByCodeAsync(string RequirementTypeCode) => throw new NotImplementedException();
        public Task<RequirementDefinition> GetRequirementDefinitionByIdAsync(int requirementDefinitionId) => throw new NotImplementedException();
        public Task<List<RequirementDefinition>> GetRequirementDefinitionsByIdsAsync(IList<int> requirementDefinitionIds) => throw new NotImplementedException();
    }
}

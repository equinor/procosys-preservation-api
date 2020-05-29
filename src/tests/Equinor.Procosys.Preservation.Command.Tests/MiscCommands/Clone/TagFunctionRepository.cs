using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;

namespace Equinor.Procosys.Preservation.Command.Tests.MiscCommands.Clone
{
    internal class TagFunctionRepository : ITagFunctionRepository
    {
        private readonly PlantProvider _plantProvider;
        private readonly List<TagFunction> _sourceTagFunctions;
        private readonly List<TagFunction> _targetTagFunctions = new List<TagFunction>();
        private readonly string _targetPlant;

        public TagFunctionRepository(PlantProvider plantProvider, List<TagFunction> sourceTagFunctions)
        {
            _plantProvider = plantProvider;
            _sourceTagFunctions = sourceTagFunctions;
            _targetPlant = plantProvider.Plant;
        }

        public void Add(TagFunction item)
        {
            if (_plantProvider.Plant == _targetPlant)
            {
                _targetTagFunctions.Add(item);
            }
            else
            {
                _sourceTagFunctions.Add(item);
            }
        }

        public Task<List<TagFunction>> GetAllAsync()
        {
            if (_plantProvider.Plant == _targetPlant)
            {
                return Task.FromResult(_targetTagFunctions);
            }
            return Task.FromResult(_sourceTagFunctions);
        }

        public Task<bool> Exists(int id) => throw new NotImplementedException();
        public Task<TagFunction> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<List<TagFunction>> GetByIdsAsync(IEnumerable<int> id) => throw new NotImplementedException();
        public void Remove(TagFunction entity) => throw new NotImplementedException();
        public Task<TagFunction> GetByCodesAsync(string tagFunctionCode, string registerCode) => throw new NotImplementedException();
        public Task<List<TagFunction>> GetAllNonVoidedWithRequirementsAsync() => throw new NotImplementedException();
    }
}

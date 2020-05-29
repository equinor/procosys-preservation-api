using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Command.Tests.MiscCommands.Clone
{
    internal abstract class TestRepository<T> where T : EntityBase
    {
        private readonly PlantProvider _plantProvider;
        private readonly List<T> _sourceItems;
        protected readonly List<T> _targetItems = new List<T>();
        private readonly string _targetPlant;

        protected TestRepository(PlantProvider plantProvider, List<T> sourceModes)
        {
            _plantProvider = plantProvider;
            _sourceItems = sourceModes;
            _targetPlant = plantProvider.Plant;
        }

        public void Add(T item)
        {
            if (_plantProvider.Plant == _targetPlant)
            {
                _targetItems.Add(item);
            }
            else
            {
                _sourceItems.Add(item);
            }
        }

        public Task<List<T>> GetAllAsync()
        {
            if (_plantProvider.Plant == _targetPlant)
            {
                return Task.FromResult(_targetItems);
            }
            return Task.FromResult(_sourceItems);
        }

        public Task<bool> Exists(int id) => throw new NotImplementedException();
        public Task<T> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<List<T>> GetByIdsAsync(IEnumerable<int> id) => throw new NotImplementedException();
        public void Remove(T entity) => throw new NotImplementedException();
    }
}

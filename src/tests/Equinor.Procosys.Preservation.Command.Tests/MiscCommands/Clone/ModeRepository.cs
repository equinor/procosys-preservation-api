using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;

namespace Equinor.Procosys.Preservation.Command.Tests.MiscCommands.Clone
{
    internal class ModeRepository : IModeRepository
    {
        private readonly PlantProvider _plantProvider;
        private readonly List<Mode> _sourceModes;
        private readonly List<Mode> _targetModes = new List<Mode>();
        private readonly string _targetPlant;

        public ModeRepository(PlantProvider plantProvider, List<Mode> sourceModes)
        {
            _plantProvider = plantProvider;
            _sourceModes = sourceModes;
            _targetPlant = plantProvider.Plant;
        }

        public void Add(Mode item)
        {
            if (_plantProvider.Plant == _targetPlant)
            {
                _targetModes.Add(item);
            }
            else
            {
                _sourceModes.Add(item);
            }
        }

        public Task<List<Mode>> GetAllAsync()
        {
            if (_plantProvider.Plant == _targetPlant)
            {
                return Task.FromResult(_targetModes);
            }
            return Task.FromResult(_sourceModes);
        }

        public Task<bool> Exists(int id) => throw new NotImplementedException();

        public Task<Mode> GetByIdAsync(int id) => throw new NotImplementedException();

        public Task<List<Mode>> GetByIdsAsync(IEnumerable<int> id) => throw new NotImplementedException();

        public void Remove(Mode entity) => throw new NotImplementedException();
    }
}

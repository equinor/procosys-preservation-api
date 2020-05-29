using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.Procosys.Preservation.Command.Tests.MiscCommands.Clone
{
    internal class ResponsibleRepository : IResponsibleRepository
    {
        private readonly PlantProvider _plantProvider;
        private readonly List<Responsible> _sourceResponsibles;
        private readonly List<Responsible> _targetResponsibles = new List<Responsible>();
        private readonly string _targetPlant;

        public ResponsibleRepository(PlantProvider plantProvider, List<Responsible> sourceResponsibles)
        {
            _plantProvider = plantProvider;
            _sourceResponsibles = sourceResponsibles;
            _targetPlant = plantProvider.Plant;
        }

        public void Add(Responsible item)
        {
            if (_plantProvider.Plant == _targetPlant)
            {
                _targetResponsibles.Add(item);
            }
            else
            {
                _sourceResponsibles.Add(item);
            }
        }

        public Task<bool> Exists(int id) => throw new NotImplementedException();

        public Task<Responsible> GetByIdAsync(int id) => throw new NotImplementedException();

        public Task<List<Responsible>> GetByIdsAsync(IEnumerable<int> id) => throw new NotImplementedException();

        public void Remove(Responsible entity) => throw new NotImplementedException();

        public Task<List<Responsible>> GetAllAsync()
        {
            if (_plantProvider.Plant == _targetPlant)
            {
                return Task.FromResult(_targetResponsibles);
            }
            return Task.FromResult(_sourceResponsibles);
        }

        public Task<Responsible> GetByCodeAsync(string responsibleCode) => throw new NotImplementedException();
    }
}

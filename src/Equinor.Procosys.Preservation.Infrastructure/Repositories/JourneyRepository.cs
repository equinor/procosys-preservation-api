using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public class JourneyRepository : IJourneyRepository
    {
        public IUnitOfWork UnitOfWork => throw new NotImplementedException();

        public void Add(Journey item)
        {
            throw new NotImplementedException();
        }

        public ValueTask<Journey> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public void Remove(Journey entity)
        {
            throw new NotImplementedException();
        }
    }
}

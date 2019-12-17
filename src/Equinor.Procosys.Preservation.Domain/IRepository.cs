using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain
{
    public interface IRepository<TEntity> where TEntity : EntityBase, IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        void Add(TEntity item);
        Task<bool> Exists(int id);
        Task<TEntity> GetByIdAsync(int id);
        void Remove(TEntity entity);
        Task<List<TEntity>> GetAllAsync();
    }
}

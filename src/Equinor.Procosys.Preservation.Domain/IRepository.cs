using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Domain
{
    public interface IRepository<TEntity> where TEntity : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        void Add(TEntity item);
        Task<TEntity> GetByIdAsync(int id);
        void Remove(TEntity entity);
    }
}

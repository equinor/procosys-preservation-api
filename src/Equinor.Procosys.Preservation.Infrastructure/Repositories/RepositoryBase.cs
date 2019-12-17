using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : EntityBase, IAggregateRoot
    {
        protected readonly DbSet<TEntity> Set;

        protected RepositoryBase(DbSet<TEntity> set, IUnitOfWork unitOfWork)
        {
            Set = set;
            UnitOfWork = unitOfWork;
        }

        public IUnitOfWork UnitOfWork { get; }

        public virtual void Add(TEntity entity) => Set.Add(entity);

        public Task<bool> Exists(int id) => Set.AnyAsync(x => x.Id == id);

        public Task<List<TEntity>> GetAllAsync() => Set.ToListAsync();

        public virtual Task<TEntity> GetByIdAsync(int id) => Set
                .FindAsync(id)
                .AsTask();

        public virtual void Remove(TEntity entity) => Set.Remove(entity);
    }
}

using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;

namespace Equinor.Procosys.Preservation.Infrastructure.Repositories
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class, IAggregateRoot
    {
        protected readonly PreservationContext context;

        protected RepositoryBase(PreservationContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => context;

        public void Add(TEntity entity)
        {
            context.Add(entity);
        }

        public ValueTask<TEntity> GetByIdAsync(int id)
        {
            return context
                .Set<TEntity>()
                .FindAsync(id);
        }

        public void Remove(TEntity entity)
        {
            context.Remove(entity);
        }
    }
}

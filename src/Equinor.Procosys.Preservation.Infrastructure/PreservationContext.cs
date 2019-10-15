using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.Events;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Equinor.Procosys.Preservation.Infrastructure
{
    public class PreservationContext : DbContext, IReadOnlyContext, IUnitOfWork
    {
        private readonly IEventDispatcher eventDispatcher;

        public PreservationContext(
            DbContextOptions<PreservationContext> options,
            IEventDispatcher eventDispatcher)
            : base(options)
        {
            this.eventDispatcher = eventDispatcher;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchEvents();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private async Task DispatchEvents()
        {
            var entities = ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .Select(x => x.Entity);
            await eventDispatcher.DispatchAsync(entities);
        }

        public IQueryable<TQuery> ReadOnlySet<TQuery>() where TQuery : class
        {
            return Set<TQuery>()
                .AsNoTracking();
        }
    }
}

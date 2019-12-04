using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure
{
    public class PreservationContext : DbContext, IReadOnlyContext, IUnitOfWork
    {
        private readonly IEventDispatcher eventDispatcher;
        private readonly IPlantProvider plantProvider;

        public PreservationContext(
            DbContextOptions<PreservationContext> options,
            IEventDispatcher eventDispatcher,
            IPlantProvider plantProvider)
            : base(options)
        {
            this.eventDispatcher = eventDispatcher;
            this.plantProvider = plantProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder
                .Entity<Entity>()
                .HasQueryFilter(e =>
                    EF.Property<string>(e, "Plant") == plantProvider.Plant //TODO: Should there be an "EntityWithPlant" that will always have the "Plant" property? Or perhaps a marker interface to allow shadow properties?
                );
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

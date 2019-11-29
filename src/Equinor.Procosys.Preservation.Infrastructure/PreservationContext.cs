using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
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
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Global query filter
            modelBuilder
                .Entity<SchemaEntity>()
                .HasQueryFilter(e =>
                    EF.Property<string>(e, nameof(SchemaEntity.Schema)) == plantProvider.Plant
                );
        }

        public virtual DbSet<Journey> Journeys { get; set; }
        public virtual DbSet<JourneyStep> JourneyStep { get; set; }
        public virtual DbSet<JourneyMode> JourneyModes { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }

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

using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure
{
    public class PreservationContext : DbContext, IReadOnlyContext, IUnitOfWork
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IPlantProvider _plantProvider;

        public PreservationContext(
            DbContextOptions<PreservationContext> options,
            IEventDispatcher eventDispatcher,
            IPlantProvider plantProvider)
            : base(options)
        {
            _eventDispatcher = eventDispatcher;
            _plantProvider = plantProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            NewMethod(modelBuilder);
        }

        private void NewMethod(ModelBuilder modelBuilder)
        {
            // Set global query filter on entities inheriting from SchemaEntityBase
            // https://gunnarpeipman.com/ef-core-global-query-filters/
            foreach (var type in TypeProvider.GetEntityTypes(typeof(IDomainMarker).GetTypeInfo().Assembly, typeof(SchemaEntityBase)))
            {
                typeof(PreservationContext)
                .GetMethod(nameof(PreservationContext.SetGlobalQuery))
                ?.MakeGenericMethod(type)
                .Invoke(this, new object[] { modelBuilder });
            }
        }

        public DbSet<Journey> Journeys { get; set; }
        public DbSet<Step> Step { get; set; }
        public DbSet<Mode> Modes { get; set; }
        public DbSet<Responsible> Responsibles { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<RequirementType> RequirementTypes { get; set; }
        public DbSet<RequirementDefinition> RequirementDefinitions { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchEvents(cancellationToken);
            return await base.SaveChangesAsync(cancellationToken);
        }

        private async Task DispatchEvents(CancellationToken cancellationToken = default)
        {
            var entities = ChangeTracker
                .Entries<EntityBase>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .Select(x => x.Entity);
            await _eventDispatcher.DispatchAsync(entities, cancellationToken);
        }

        public IQueryable<TQuery> ReadOnlySet<TQuery>() where TQuery : class =>
                Set<TQuery>()
                .AsNoTracking();

        public void SetGlobalQuery<T>(ModelBuilder builder) where T : SchemaEntityBase =>
            builder
            .Entity<T>()
            .HasQueryFilter(e => e.Schema == _plantProvider.Plant);
    }
}

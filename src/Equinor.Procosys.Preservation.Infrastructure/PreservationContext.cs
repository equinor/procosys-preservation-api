using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Equinor.Procosys.Preservation.Infrastructure
{
    public class PreservationContext : DbContext, IUnitOfWork, IReadOnlyContext
    {
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IPlantProvider _plantProvider;
        private readonly ITimeService _timeService;
        private readonly ICurrentUserProvider _currentUserProvider;

        public PreservationContext(
            DbContextOptions<PreservationContext> options,
            IEventDispatcher eventDispatcher,
            IPlantProvider plantProvider,
            ITimeService timeService,
            ICurrentUserProvider currentUserProvider)
            : base(options)
        {
            _eventDispatcher = eventDispatcher;
            _plantProvider = plantProvider;
            _timeService = timeService;
            _currentUserProvider = currentUserProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            SetGlobalPlantFilter(modelBuilder);
        }

        private void SetGlobalPlantFilter(ModelBuilder modelBuilder)
        {
            // Set global query filter on entities inheriting from SchemaEntityBase
            // https://gunnarpeipman.com/ef-core-global-query-filters/
            foreach (var type in TypeProvider.GetEntityTypes(typeof(IDomainMarker).GetTypeInfo().Assembly, typeof(SchemaEntityBase)))
            {
                typeof(PreservationContext)
                .GetMethod(nameof(PreservationContext.SetGlobalQueryFilter))
                ?.MakeGenericMethod(type)
                .Invoke(this, new object[] { modelBuilder });
            }
        }

        public static DateTimeKindConverter DateTimeKindConverter { get; } = new DateTimeKindConverter();
        public static NullableDateTimeKindConverter NullableDateTimeKindConverter { get; } = new NullableDateTimeKindConverter();

        public virtual DbSet<Journey> Journeys { get; set; }
        public virtual DbSet<Step> Step { get; set; }
        public virtual DbSet<Mode> Modes { get; set; }
        public virtual DbSet<Responsible> Responsibles { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<RequirementType> RequirementTypes { get; set; }
        public virtual DbSet<RequirementDefinition> RequirementDefinitions { get; set; }
        public virtual DbSet<Field> Fields { get; set; }
        public virtual DbSet<Requirement> Requirements { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<PreservationRecord> PreservationRecords { get; set; }
        public virtual DbSet<PreservationPeriod> PreservationPeriods { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<FieldValue> FieldValues { get; set; }
        public virtual DbSet<Action> Actions { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchEvents(cancellationToken);
            await SetAuditData();
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

        public void SetGlobalQueryFilter<T>(ModelBuilder builder) where T : SchemaEntityBase =>
            builder
            .Entity<T>()
            .HasQueryFilter(e => e.Schema == _plantProvider.Plant);

        public IQueryable<TEntity> QuerySet<TEntity>() where TEntity : class => Set<TEntity>().AsNoTracking();

        private async Task SetAuditData()
        {
            var auditables = ChangeTracker.Entries<IAuditable>().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
            if (auditables.Any())
            {
                var now = _timeService.GetCurrentTimeUtc();
                var currentUser = await _currentUserProvider.GetCurrentUserAsync();

                foreach (var entry in auditables)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Entity.SetCreated(now, currentUser.Id);
                            break;
                        case EntityState.Modified:
                            entry.Entity.SetModified(now, currentUser.Id);
                            break;
                    }
                }
            }
        }
    }
}

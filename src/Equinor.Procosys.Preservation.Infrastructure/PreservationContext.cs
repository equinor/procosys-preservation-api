using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.Procosys.Preservation.Domain.Audit;
using Equinor.Procosys.Preservation.Domain.Events;
using Equinor.Procosys.Preservation.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure
{
    public class PreservationContext : DbContext, IUnitOfWork, IReadOnlyContext
    {
        private readonly IPlantProvider _plantProvider;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly ICurrentUserProvider _currentUserProvider;

        public PreservationContext(
            DbContextOptions<PreservationContext> options,
            IPlantProvider plantProvider,
            IEventDispatcher eventDispatcher,
            ICurrentUserProvider currentUserProvider)
            : base(options)
        {
            _plantProvider = plantProvider;
            _eventDispatcher = eventDispatcher;
            _currentUserProvider = currentUserProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            SetGlobalPlantFilter(modelBuilder);
        }

        public static DateTimeKindConverter DateTimeKindConverter { get; } = new DateTimeKindConverter();
        public static NullableDateTimeKindConverter NullableDateTimeKindConverter { get; } = new NullableDateTimeKindConverter();

        public virtual DbSet<Journey> Journeys { get; set; }
        public virtual DbSet<Step> Steps { get; set; }
        public virtual DbSet<Mode> Modes { get; set; }
        public virtual DbSet<Responsible> Responsibles { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<RequirementType> RequirementTypes { get; set; }
        public virtual DbSet<RequirementDefinition> RequirementDefinitions { get; set; }
        public virtual DbSet<Field> Fields { get; set; }
        public virtual DbSet<TagRequirement> TagRequirements { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<PreservationRecord> PreservationRecords { get; set; }
        public virtual DbSet<PreservationPeriod> PreservationPeriods { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<FieldValue> FieldValues { get; set; }
        public virtual DbSet<Action> Actions { get; set; }
        public virtual DbSet<TagFunction> TagFunctions { get; set; }
        public virtual DbSet<TagFunctionRequirement> TagFunctionRequirements { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<History> HistoryEntries { get; set; }

        private void SetGlobalPlantFilter(ModelBuilder modelBuilder)
        {
            // Set global query filter on entities inheriting from PlantEntityBase
            // https://gunnarpeipman.com/ef-core-global-query-filters/
            foreach (var type in TypeProvider.GetEntityTypes(typeof(IDomainMarker).GetTypeInfo().Assembly, typeof(PlantEntityBase)))
            {
                typeof(PreservationContext)
                .GetMethod(nameof(PreservationContext.SetGlobalQueryFilter))
                ?.MakeGenericMethod(type)
                .Invoke(this, new object[] { modelBuilder });
            }
        }

        public void SetGlobalQueryFilter<T>(ModelBuilder builder) where T : PlantEntityBase =>
            builder
            .Entity<T>()
            .HasQueryFilter(e => e.Plant == _plantProvider.Plant);

        public IQueryable<TEntity> QuerySet<TEntity>() where TEntity : class => Set<TEntity>().AsNoTracking();

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchEventsAsync(cancellationToken);
            await SetAuditDataAsync();
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException concurrencyException)
            {
                throw new ConcurrencyException("Data store operation failed. Data may have been modified or deleted since entities were loaded.", concurrencyException);
            }
        }

        private async Task DispatchEventsAsync(CancellationToken cancellationToken = default)
        {
            var entities = ChangeTracker
                .Entries<EntityBase>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .Select(x => x.Entity);
            await _eventDispatcher.DispatchAsync(entities, cancellationToken);
        }

        private async Task SetAuditDataAsync()
        {
            var addedEntries = ChangeTracker
                .Entries<ICreationAuditable>()
                .Where(x => x.State == EntityState.Added)
                .ToList();
            var modifiedEntries = ChangeTracker
                .Entries<IModificationAuditable>()
                .Where(x => x.State == EntityState.Modified)
                .ToList();

            if (addedEntries.Any() || modifiedEntries.Any())
            {
                var currentUserOid = _currentUserProvider.GetCurrentUserOid();
                var currentUser = await Persons.SingleOrDefaultAsync(p => p.Oid == currentUserOid);

                foreach (var entry in addedEntries)
                {
                    entry.Entity.SetCreated(currentUser);
                }

                foreach (var entry in modifiedEntries)
                {
                    entry.Entity.SetModified(currentUser);
                }
            }
        }
    }
}

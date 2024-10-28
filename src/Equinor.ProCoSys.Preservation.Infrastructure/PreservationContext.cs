using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.SettingAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.ProCoSys.Preservation.Domain.Audit;
using Microsoft.EntityFrameworkCore;
using IDomainMarker = Equinor.ProCoSys.Preservation.Domain.IDomainMarker;
using MassTransit;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;
using ConcurrencyException = Equinor.ProCoSys.Common.Misc.ConcurrencyException;


namespace Equinor.ProCoSys.Preservation.Infrastructure
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

        private static void ConfigureOutBoxPattern(ModelBuilder modelBuilder)
            => modelBuilder.AddTransactionalOutboxEntities();


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (DebugOptions.DebugEntityFrameworkInDevelopment)
            {
                optionsBuilder.LogTo(System.Console.WriteLine);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            SetGlobalPlantFilter(modelBuilder);
            ConfigureOutBoxPattern(modelBuilder);
        }

        public static DateTimeKindConverter DateTimeKindConverter { get; } = new DateTimeKindConverter();
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
        public virtual DbSet<History> History { get; set; }
        public virtual DbSet<SavedFilter> SavedFilters { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }

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
            .HasQueryFilter(e => e.Plant == _plantProvider.Plant || _plantProvider.IsCrossPlantQuery);

        public IQueryable<TEntity> QuerySet<TEntity>() where TEntity : class => Set<TEntity>().AsNoTracking();

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //Some events are sent in SetCreated and SetModified which comes from SetAuditDataAsync, so queue those and then dispatches
            //Then does SetAuditDataAsync again to update History objects that were created in the DomainEvents
            await SetAuditDataAsync();
            await DispatchDomainEventsAsync(cancellationToken);
            await SetAuditDataAsync();

            UpdateConcurrencyToken();

            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException concurrencyException)
            {
                throw new ConcurrencyException("Data store operation failed. Data may have been modified or deleted since entities were loaded.", concurrencyException);
            }
        }

        private void UpdateConcurrencyToken()
        {
            var modifiedEntries = ChangeTracker
                .Entries<EntityBase>()
                .Where(x => x.State == EntityState.Modified || x.State == EntityState.Deleted);

            foreach (var entry in modifiedEntries)
            {
                var currentRowVersion = entry.CurrentValues.GetValue<byte[]>(nameof(EntityBase.RowVersion));
                var originalRowVersion = entry.OriginalValues.GetValue<byte[]>(nameof(EntityBase.RowVersion));
                for (var i = 0; i < currentRowVersion.Length; i++)
                {
                    originalRowVersion[i] = currentRowVersion[i];
                }
            }
        }

        private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default)
        {
            var entities = ChangeTracker
                .Entries<EntityBase>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .Select(x => x.Entity);
            await _eventDispatcher.DispatchDomainEventsAsync(entities, cancellationToken);
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
                var currentUser = await Persons.SingleOrDefaultAsync(p => p.Guid == currentUserOid);

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

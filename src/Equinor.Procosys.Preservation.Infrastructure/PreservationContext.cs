using System.Linq;
using System.Reflection;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.EntityFrameworkCore;

namespace Equinor.Procosys.Preservation.Infrastructure
{
    public class PreservationContext : DbContext, IReadOnlyContext
    {
        private readonly IPlantProvider _plantProvider;

        public PreservationContext(DbContextOptions<PreservationContext> options, IPlantProvider plantProvider)
            : base(options) => _plantProvider = plantProvider;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            SetGlobalPlantFilter(modelBuilder);
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

        public void SetGlobalQueryFilter<T>(ModelBuilder builder) where T : SchemaEntityBase =>
            builder
            .Entity<T>()
            .HasQueryFilter(e => e.Schema == _plantProvider.Plant);

        public IQueryable<TEntity> QuerySet<TEntity>() where TEntity : class => Set<TEntity>().AsNoTracking();
    }
}

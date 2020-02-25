using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class RequirementConfiguration : IEntityTypeConfiguration<Requirement>
    {
        public void Configure(EntityTypeBuilder<Requirement> builder)
        {
            builder.Property(x => x.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();

            builder.Property(x => x.NextDueTimeUtc)
                .HasConversion(PreservationContext.NullableDateTimeKindConverter);

            builder.HasOne<RequirementDefinition>();

            builder
                .HasMany(x => x.PreservationPeriods)
                .WithOne()
                .IsRequired();

            builder.Property(f => f.InitialPreservationPeriodStatus)
                .HasConversion<string>()
                .HasDefaultValue(PreservationPeriodStatus.NeedsUserInput)
                .IsRequired();

            builder.HasCheckConstraint(
                "constraint_requirement_check_valid_initial_status",
                $"{nameof(Requirement.InitialPreservationPeriodStatus)} in ('{PreservationPeriodStatus.NeedsUserInput}','{PreservationPeriodStatus.ReadyToBePreserved}')");
        }
    }
}

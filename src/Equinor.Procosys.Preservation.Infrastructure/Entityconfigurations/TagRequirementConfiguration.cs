using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class TagRequirementConfiguration : IEntityTypeConfiguration<TagRequirement>
    {
        private readonly string InitialPreservationPeriodStatusPropertyName = "_initialPreservationPeriodStatus";

        public void Configure(EntityTypeBuilder<TagRequirement> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();
            builder.ConfigureConcurrencyToken();

            builder.Property(x => x.NextDueTimeUtc)
                .HasConversion(PreservationContext.DateTimeKindConverter);

            builder.HasOne<RequirementDefinition>()
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(x => x.PreservationPeriods)
                .WithOne()
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(InitialPreservationPeriodStatusPropertyName)
                .HasMaxLength(TagRequirement.InitialPreservationPeriodStatusMax)
                .HasConversion<string>()
                .HasDefaultValue(PreservationPeriodStatus.NeedsUserInput)
                .IsRequired();

            builder.HasCheckConstraint(
                "constraint_requirement_check_valid_initial_status",
                $"{InitialPreservationPeriodStatusPropertyName} in ('{PreservationPeriodStatus.NeedsUserInput}','{PreservationPeriodStatus.ReadyToBePreserved}')");
     
            builder.Property(x => x.Usage)
                .HasConversion<string>()
                .HasDefaultValue(RequirementUsage.ForAll)
                .HasMaxLength(RequirementDefinition.UsageMax)
                .IsRequired();

            builder.HasCheckConstraint("constraint_tagreq_check_valid_usage", $"{nameof(RequirementDefinition.Usage)} in ({GetValidUsages()})");
        }

        private string GetValidUsages()
        {
            var names = Enum.GetNames(typeof(RequirementUsage)).Select(t => $"'{t}'");
            return string.Join(',', names);
        }
    }
}

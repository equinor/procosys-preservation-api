using System;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations
{
    internal class RequirementDefinitionConfiguration : IEntityTypeConfiguration<RequirementDefinition>
    {
        public void Configure(EntityTypeBuilder<RequirementDefinition> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();
            builder.ConfigureConcurrencyToken();

            builder.Property(rt => rt.Title)
                .IsRequired()
                .HasMaxLength(RequirementDefinition.TitleLengthMax);

            builder
                .HasMany(x => x.Fields)
                .WithOne()
                .IsRequired();

            builder
                .HasIndex(x => x.Plant)
                .HasDatabaseName("IX_RequirementDefinitions_Plant_ASC")
                .IncludeProperties(x => new {x.IsVoided, x.CreatedAtUtc, x.ModifiedAtUtc, x.SortKey, x.Title});
     
            builder.Property(x => x.Usage)
                .HasConversion<string>()
                .HasDefaultValue(RequirementUsage.ForAll)
                .HasMaxLength(RequirementDefinition.UsageMax)
                .IsRequired();

            builder.HasCheckConstraint("constraint_reqdef_check_valid_usage", $"{nameof(RequirementDefinition.Usage)} in ({GetValidUsages()})");
        }

        private string GetValidUsages()
        {
            var names = Enum.GetNames(typeof(RequirementUsage)).Select(t => $"'{t}'");
            return string.Join(',', names);
        }
    }
}

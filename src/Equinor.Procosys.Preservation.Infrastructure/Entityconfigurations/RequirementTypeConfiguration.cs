using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class RequirementTypeConfiguration : IEntityTypeConfiguration<RequirementType>
    {
        public void Configure(EntityTypeBuilder<RequirementType> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();
            builder.ConfigureConcurrencyToken();

            builder.Property(rt => rt.Code)
                .IsRequired()
                .HasMaxLength(RequirementType.CodeLengthMax);

            builder.Property(rt => rt.Title)
                .IsRequired()
                .HasMaxLength(RequirementType.TitleLengthMax);

            builder.Property(f => f.Icon)
                .HasConversion<string>()
                .HasDefaultValue(RequirementTypeIcon.Other)
                .IsRequired();

            builder.HasCheckConstraint("constraint_requirement_type_check_icon", $"{nameof(RequirementType.Icon)} in ({GetValidIcons()})");

            builder
                .HasMany(x => x.RequirementDefinitions)
                .WithOne()
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
        private string GetValidIcons()
        {
            var fieldTypes = Enum.GetNames(typeof(RequirementTypeIcon)).Select(t => $"'{t}'");
            return string.Join(',', fieldTypes);
        }
    }
}

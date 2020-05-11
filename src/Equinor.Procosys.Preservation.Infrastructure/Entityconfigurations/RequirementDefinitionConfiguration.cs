using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
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
                .HasName("IX_PreservationPeriods_Plant_ASC")
                .IncludeProperties(x => new {x.IsVoided, x.CreatedAtUtc, x.ModifiedAtUtc, x.SortKey, x.Title});
        }
    }
}

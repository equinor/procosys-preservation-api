using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class RequirementDefinitionConfiguration : IEntityTypeConfiguration<RequirementDefinition>
    {
        public void Configure(EntityTypeBuilder<RequirementDefinition> builder)
        {
            builder.Property(x => x.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();

            builder.Property(rt => rt.Title)
                .IsRequired()
                .HasMaxLength(RequirementDefinition.TitleLengthMax);

            builder
                .HasMany(x => x.Fields)
                .WithOne()
                .IsRequired();
        }
    }
}

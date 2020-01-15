using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
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

            builder.HasOne<RequirementDefinition>();

            builder
                .HasMany(x => x.PreservationRecords)
                .WithOne()
                .IsRequired();
        }
    }
}

using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.Entityconfigurations
{
    internal class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.Property(x => x.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();

            builder.Property(x => x.ProjectNumber)
                .HasMaxLength(Tag.ProjectNumberLengthMax)
                .IsRequired();

            builder.Property(x => x.TagNumber)
                .HasMaxLength(Tag.TagNumberLengthMax)
                .IsRequired();

            builder
                .HasMany(x => x.Requirements)
                .WithOne()
                .IsRequired();
        }
    }
}

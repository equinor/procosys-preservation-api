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
                .HasMaxLength(Tag.SchemaLengthMax)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(Tag.DescriptionLengthMax);

            builder.Property(x => x.ProjectNumber)
                .HasMaxLength(Tag.ProjectNoLengthMax)
                .IsRequired();

            builder.Property(x => x.TagNumber)
                .HasMaxLength(Tag.TagNoLengthMax)
                .IsRequired();
        }
    }
}

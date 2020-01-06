using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.Entityconfigurations
{
    internal class ModeConfiguration : IEntityTypeConfiguration<Mode>
    {
        public void Configure(EntityTypeBuilder<Mode> builder)
        {
            builder.Property(x => x.Schema)
                .HasMaxLength(Mode.SchemaLengthMax)
                .IsRequired();

            builder.Property(x => x.Title)
                .HasMaxLength(Mode.TitleLengthMax)
                .IsRequired();
        }
    }
}

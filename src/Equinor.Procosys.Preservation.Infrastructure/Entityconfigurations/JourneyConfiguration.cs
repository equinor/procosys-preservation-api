using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.Entityconfigurations
{
    internal class JourneyConfiguration : IEntityTypeConfiguration<Journey>
    {
        public void Configure(EntityTypeBuilder<Journey> builder)
        {
            builder.Property(x => x.Schema)
                .HasMaxLength(Journey.SchemaLengthMax)
                .IsRequired();

            builder.Property(x => x.Title)
                .HasMaxLength(Journey.TitleLengthMax)
                .IsRequired();
        }
    }
}

using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class JourneyConfiguration : IEntityTypeConfiguration<Journey>
    {
        public void Configure(EntityTypeBuilder<Journey> builder)
        {
            builder.ConfigureSchema();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();

            builder.Property(x => x.Title)
                .HasMaxLength(Journey.TitleLengthMax)
                .IsRequired();
        }
    }
}

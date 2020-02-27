using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class ModeConfiguration : IEntityTypeConfiguration<Mode>
    {
        public void Configure(EntityTypeBuilder<Mode> builder)
        {
            builder.ConfigureSchema();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();

            builder.Property(x => x.Title)
                .HasMaxLength(Mode.TitleLengthMax)
                .IsRequired();
        }
    }
}

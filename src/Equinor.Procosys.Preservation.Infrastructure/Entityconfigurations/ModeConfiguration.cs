using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class ModeConfiguration : EntityBaseConfiguration<Mode>
    {
        public override void Configure(EntityTypeBuilder<Mode> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();

            builder.Property(x => x.Title)
                .HasMaxLength(Mode.TitleLengthMax)
                .IsRequired();
        }
    }
}

using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class ResponsibleConfiguration : EntityBaseConfiguration<Responsible>
    {
        public override void Configure(EntityTypeBuilder<Responsible> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(Responsible.NameLengthMax)
                .IsRequired();
        }
    }
}

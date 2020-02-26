using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class FieldValueConfiguration : EntityBaseConfiguration<FieldValue>
    {
        public override void Configure(EntityTypeBuilder<FieldValue> builder)
        {
            base.Configure(builder);

            builder.Property(f => f.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();

            builder
                .HasDiscriminator<string>("FieldType")
                .HasValue<CheckBoxChecked>("CheckBoxChecked")
                .HasValue<Attachment>("Attachment")
                .HasValue<NumberValue>("NumberValue");
        }
    }
}

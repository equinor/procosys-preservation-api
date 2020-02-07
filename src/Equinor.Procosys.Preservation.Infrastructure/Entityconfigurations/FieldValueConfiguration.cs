using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class FieldValueConfiguration : IEntityTypeConfiguration<FieldValue>
    {
        public void Configure(EntityTypeBuilder<FieldValue> builder)
        {
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

using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class FieldValueConfiguration : IEntityTypeConfiguration<FieldValue>
    {
        public void Configure(EntityTypeBuilder<FieldValue> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureConcurrencyToken();

            builder
                .HasDiscriminator<string>("FieldType")
                .HasValue<CheckBoxChecked>("CheckBoxChecked")
                .HasValue<AttachmentValue>("AttachmentValue")
                .HasValue<NumberValue>("NumberValue");

            //builder.HasOne(b => b.FieldValueAttachment);
        }
    }
}

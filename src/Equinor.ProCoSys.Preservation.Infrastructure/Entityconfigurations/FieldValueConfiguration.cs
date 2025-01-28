using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations
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
                // When adding a new value be sure to run a migration since the length of the discriminator column might need to be increased.
                .HasValue<CheckBoxChecked>("CheckBoxChecked")
                .HasValue<AttachmentValue>("AttachmentValue")
                .HasValue<NumberValue>("NumberValue");
        }
    }
}

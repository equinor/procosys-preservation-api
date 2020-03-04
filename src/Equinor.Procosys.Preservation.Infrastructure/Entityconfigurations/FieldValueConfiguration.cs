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
            builder.ConfigureSchema();
            builder.ConfigureCreationAudit();

            builder
                .HasDiscriminator<string>("FieldType")
                .HasValue<CheckBoxChecked>("CheckBoxChecked")
                .HasValue<Attachment>("Attachment")
                .HasValue<NumberValue>("NumberValue");
        }
    }
}

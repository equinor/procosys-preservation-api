using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class ActionAttachmentConfiguration : IEntityTypeConfiguration<ActionAttachment>
    {
        public void Configure(EntityTypeBuilder<ActionAttachment> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureConcurrencyToken();

            builder.Property(x => x.Title)
                .HasMaxLength(AttachmentEntityBase.TitleLengthMax)
                .IsRequired();
        }
    }
}

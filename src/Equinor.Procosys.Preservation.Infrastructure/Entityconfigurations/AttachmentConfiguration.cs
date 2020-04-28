using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureConcurrencyToken();

            builder.Property(x => x.Title)
                .HasMaxLength(Attachment.TitleLengthMax)
                .IsRequired();

            builder.Property(x => x.FileName)
                .HasMaxLength(Attachment.FileNameLengthMax)
                .IsRequired();

            builder
                .HasDiscriminator<string>("AttachmentType")
                .HasValue<TagAttachment>("TagAttachment")
                .HasValue<ActionAttachment>("ActionAttachment");
        }
    }
}

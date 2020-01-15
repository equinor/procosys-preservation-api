using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.Entityconfigurations
{
    internal class PreservationRecordConfiguration : IEntityTypeConfiguration<PreservationRecord>
    {
        public void Configure(EntityTypeBuilder<PreservationRecord> builder)
        {
            builder.Property(x => x.Comment)
                .HasMaxLength(PreservationRecord.CommentLengthMax);

            builder.Property(x => x.Preserved)
                .IsRequired(false);

            builder.Property(x => x.PreservedBy)
                .IsRequired(false);
        }
    }
}

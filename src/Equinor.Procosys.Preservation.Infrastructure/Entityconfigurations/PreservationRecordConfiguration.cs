using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.Entityconfigurations
{
    internal class PreservationRecordConfiguration : IEntityTypeConfiguration<PreservationRecord>
    {
        public void Configure(EntityTypeBuilder<PreservationRecord> builder)
        {
            builder.Property(f => f.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();
            
            builder.Property(x => x.PreservedAtUtc)
                .HasConversion(PreservationContext.DateTimeKindConverter);
        }
    }
}

using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class PreservationRecordConfiguration : EntityBaseConfiguration<PreservationRecord>
    {
        public override void Configure(EntityTypeBuilder<PreservationRecord> builder)
        {
            base.Configure(builder);

            builder.Property(f => f.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();
            
            builder.Property(x => x.PreservedAtUtc)
                .HasConversion(PreservationContext.DateTimeKindConverter);
        }
    }
}

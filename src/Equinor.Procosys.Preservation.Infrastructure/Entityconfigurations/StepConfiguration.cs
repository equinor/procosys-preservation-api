using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.Entityconfigurations
{
    internal class StepConfiguration : IEntityTypeConfiguration<Step>
    {
        public void Configure(EntityTypeBuilder<Step> builder)
        {
            builder.Property(x => x.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();

            builder.HasOne<Mode>();
            builder.HasMany<Tag>();
        }
    }
}

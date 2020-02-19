using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.Entityconfigurations
{
    internal class ActionConfiguration : IEntityTypeConfiguration<Action>
    {
        public void Configure(EntityTypeBuilder<Action> builder)
        {
            builder.Property(f => f.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();
            
            builder.Property(x => x.Description)
                .HasMaxLength(Action.DescriptionLengthMax)
                .IsRequired();

            builder.Property(x => x.DueTimeUtc)
                .HasConversion(PreservationContext.NullableDateTimeKindConverter);
            
            builder.Property(x => x.ClosedAtUtc)
                .HasConversion(PreservationContext.NullableDateTimeKindConverter);

            builder
                .HasMany(x => x.ActionComments)
                .WithOne()
                .IsRequired();
        }
    }
}

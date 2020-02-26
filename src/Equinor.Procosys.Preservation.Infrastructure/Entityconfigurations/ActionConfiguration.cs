using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class ActionConfiguration : EntityBaseConfiguration<Action>
    {
        public override void Configure(EntityTypeBuilder<Action> builder)
        {
            base.Configure(builder);

            builder.Property(f => f.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();
            
            builder.Property(x => x.Title)
                .HasMaxLength(Action.TitleLengthMax)
                .IsRequired();
            
            builder.Property(x => x.Description)
                .HasMaxLength(Action.DescriptionLengthMax)
                .IsRequired();

            builder.Property(x => x.DueTimeUtc)
                .HasConversion(PreservationContext.NullableDateTimeKindConverter);
            
            builder.Property(x => x.ClosedAtUtc)
                .HasConversion(PreservationContext.NullableDateTimeKindConverter);
        }
    }
}

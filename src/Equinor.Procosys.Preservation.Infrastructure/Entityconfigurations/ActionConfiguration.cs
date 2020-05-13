using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class ActionConfiguration : IEntityTypeConfiguration<Action>
    {
        public void Configure(EntityTypeBuilder<Action> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();
            builder.ConfigureConcurrencyToken();

            builder.Property(x => x.Title)
                .HasMaxLength(Action.TitleLengthMax)
                .IsRequired();
            
            builder.Property(x => x.Description)
                .HasMaxLength(Action.DescriptionLengthMax)
                .IsRequired();

            builder.HasOne<Person>()
                .WithMany()
                .HasForeignKey(x => x.ClosedById)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(x => x.DueTimeUtc)
                .HasConversion(PreservationContext.NullableDateTimeKindConverter);
            
            builder.Property(x => x.ClosedAtUtc)
                .HasConversion(PreservationContext.NullableDateTimeKindConverter);

            builder
                .HasMany(x => x.Attachments)
                .WithOne()
                .IsRequired();
        }
    }
}

using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations
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
                .HasConversion(PreservationContext.DateTimeKindConverter);
            
            builder.Property(x => x.ClosedAtUtc)
                .HasConversion(PreservationContext.DateTimeKindConverter);

            builder
                .HasMany(x => x.Attachments)
                .WithOne()
                .IsRequired();
        }
    }
}

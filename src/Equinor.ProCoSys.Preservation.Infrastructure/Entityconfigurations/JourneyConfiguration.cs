using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations
{
    internal class JourneyConfiguration : IEntityTypeConfiguration<Journey>
    {
        public void Configure(EntityTypeBuilder<Journey> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();
            builder.ConfigureConcurrencyToken();

            builder.Property(x => x.Title)
                .HasMaxLength(Journey.TitleLengthMax)
                .IsRequired();

            builder
                .HasIndex(x => x.Plant)
                .HasDatabaseName("IX_Journeys_Plant_ASC")
                .IncludeProperties(x => new {x.CreatedAtUtc, x.IsVoided, x.ModifiedAtUtc, x.Title});

            builder
                .HasMany(x => x.Steps)
                .WithOne()
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class ModeConfiguration : IEntityTypeConfiguration<Mode>
    {
        public void Configure(EntityTypeBuilder<Mode> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();
            builder.ConfigureConcurrencyToken();

            builder.Property(x => x.Title)
                .HasMaxLength(Mode.TitleLengthMax)
                .IsRequired();

            builder
                .HasIndex(x => x.Plant)
                .HasDatabaseName("IX_Modes_Plant_ASC")
                .IncludeProperties(x => new {x.CreatedAtUtc, x.IsVoided, x.ModifiedAtUtc, x.Title});
        }
    }
}

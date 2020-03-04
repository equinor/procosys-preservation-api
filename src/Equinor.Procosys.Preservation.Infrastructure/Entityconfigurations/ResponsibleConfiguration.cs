using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class ResponsibleConfiguration : IEntityTypeConfiguration<Responsible>
    {
        public void Configure(EntityTypeBuilder<Responsible> builder)
        {
            builder.ConfigureSchema();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();

            builder.Property(x => x.Code)
                .HasMaxLength(Responsible.CodeLengthMax)
                .IsRequired();

            builder.Property(x => x.Title)
                .HasMaxLength(Responsible.TitleLengthMax)
                .IsRequired();
        }
    }
}

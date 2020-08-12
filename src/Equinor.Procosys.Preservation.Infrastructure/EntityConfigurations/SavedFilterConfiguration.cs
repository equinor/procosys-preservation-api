using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class SavedFilterConfiguration : IEntityTypeConfiguration<SavedFilter>
    {
        public void Configure(EntityTypeBuilder<SavedFilter> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureConcurrencyToken();

            builder.Property(x => x.Title)
                .HasMaxLength(SavedFilter.TitleLengthMax)
                .IsRequired();

            builder.Property(x => x.Criteria)
                .HasMaxLength(SavedFilter.CriteriaLengthMax)
                .IsRequired();

            builder.HasOne<Project>()
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();

            builder.Property(x => x.Name)
                .HasMaxLength(Project.NameLengthMax)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(Project.DescriptionLengthMax)
                .IsRequired();
            
            builder
                .HasMany(x => x.Tags)
                .WithOne()
                .IsRequired();
        }
    }
}

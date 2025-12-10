using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations
{
    internal class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();
            builder.ConfigureConcurrencyToken();

            builder.Property(x => x.Name)
                .HasMaxLength(Project.NameLengthMax)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(Project.DescriptionLengthMax)
                .IsRequired();

            builder
                .HasMany(x => x.Tags)
                .WithOne()
                .IsRequired()
                .OnDelete(DeleteBehavior.ClientCascade);

            builder
                .HasIndex(p => p.Plant)
                .HasDatabaseName("IX_Projects_Plant_ASC")
                .IncludeProperties(p => new { p.Name, p.IsClosed, p.CreatedAtUtc, p.ModifiedAtUtc });

            builder
               .HasIndex(p => p.Name)
               .HasDatabaseName("IX_Projects_Name_ASC")
               .IncludeProperties(p => new { p.Plant });
        }
    }
}

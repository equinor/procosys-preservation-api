using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class TagFunctionRequirementConfiguration : IEntityTypeConfiguration<TagFunctionRequirement>
    {
        public void Configure(EntityTypeBuilder<TagFunctionRequirement> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();
            builder.ConfigureConcurrencyToken();

            builder.HasOne<RequirementDefinition>()
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasIndex(x => x.Plant)
                .HasDatabaseName("IX_TagFunctionRequirements_Plant_ASC")
                .IncludeProperties(x => new {x.CreatedAtUtc, x.IsVoided, x.ModifiedAtUtc});
        }
    }
}

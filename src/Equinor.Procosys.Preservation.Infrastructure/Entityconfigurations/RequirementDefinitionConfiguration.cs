using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class RequirementDefinitionConfiguration : IEntityTypeConfiguration<RequirementDefinition>
    {
        public void Configure(EntityTypeBuilder<RequirementDefinition> builder)
        {
            builder.Property(rt => rt.Title).IsRequired().HasMaxLength(RequirementDefinition.TitleLengthMax);
        }
    }
}

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
            builder.ConfigureSchema();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();

            builder.HasOne<RequirementDefinition>();
        }
    }
}

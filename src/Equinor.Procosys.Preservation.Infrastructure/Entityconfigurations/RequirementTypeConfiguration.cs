using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class RequirementTypeConfiguration : IEntityTypeConfiguration<RequirementType>
    {
        public void Configure(EntityTypeBuilder<RequirementType> builder)
        {
            builder.Property(rt => rt.Code).IsRequired().HasMaxLength(RequirementType.CodeMax);
            builder.Property(rt => rt.Title).IsRequired().HasMaxLength(RequirementType.TitleMax);
        }
    }
}

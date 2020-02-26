using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class RequirementTypeConfiguration : EntityBaseConfiguration<RequirementType>
    {
        public override void Configure(EntityTypeBuilder<RequirementType> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();

            builder.Property(rt => rt.Code)
                .IsRequired()
                .HasMaxLength(RequirementType.CodeLengthMax);

            builder.Property(rt => rt.Title)
                .IsRequired()
                .HasMaxLength(RequirementType.TitleLengthMax);

            builder
                .HasMany(x => x.RequirementDefinitions)
                .WithOne()
                .IsRequired();
        }
    }
}

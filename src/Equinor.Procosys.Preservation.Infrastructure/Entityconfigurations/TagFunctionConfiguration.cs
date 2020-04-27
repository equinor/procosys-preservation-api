using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class TagFunctionConfiguration : IEntityTypeConfiguration<TagFunction>
    {
        public void Configure(EntityTypeBuilder<TagFunction> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();
            builder.ConfigureConcurrencyToken();

            builder.Property(x => x.Code)
                .HasMaxLength(TagFunction.CodeLengthMax)
                .IsRequired();

            builder.Property(x => x.RegisterCode)
                .HasMaxLength(TagFunction.RegisterCodeLengthMax)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(TagFunction.DescriptionLengthMax);

            builder
                .HasMany(x => x.Requirements)
                .WithOne()
                .IsRequired();
        }
    }
}

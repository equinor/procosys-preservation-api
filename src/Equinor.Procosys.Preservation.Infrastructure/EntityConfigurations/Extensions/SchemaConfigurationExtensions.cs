using Equinor.Procosys.Preservation.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions
{
    public static class SchemaConfigurationExtensions
    {
        public static void ConfigureSchema<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : SchemaEntityBase
        {
            builder.Property(x => x.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();
        }
    }
}

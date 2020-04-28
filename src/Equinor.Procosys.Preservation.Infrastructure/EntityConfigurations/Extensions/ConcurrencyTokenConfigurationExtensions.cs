using Equinor.Procosys.Preservation.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions
{
    public static class ConcurrencyTokenConfigurationExtensions
    {
        public static void ConfigureConcurrencyToken<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : EntityBase
        {
            builder
                .Property("RowVersion")
                .IsRowVersion()
                .UsePropertyAccessMode(PropertyAccessMode.Field);

        }
    }
}

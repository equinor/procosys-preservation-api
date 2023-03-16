using Equinor.ProCoSys.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations.Extensions
{
    public static class ConcurrencyTokenConfigurationExtensions
    {
        public static void ConfigureConcurrencyToken<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : EntityBase =>
            builder
                .Property(nameof(EntityBase.RowVersion))
                .IsRowVersion()
                .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

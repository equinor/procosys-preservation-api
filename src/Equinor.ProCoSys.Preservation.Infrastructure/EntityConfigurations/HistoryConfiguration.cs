using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Microsoft.EntityFrameworkCore;
using Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using System;
using System.Linq;

namespace Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations
{
    internal class HistoryConfiguration : IEntityTypeConfiguration<History>
    {
        public void Configure(EntityTypeBuilder<History> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureConcurrencyToken();

            builder.Property(x => x.Description)
                .HasMaxLength(History.DescriptionLengthMax)
                .IsRequired();

            builder.Property(f => f.EventType)
                .HasConversion<string>()
                .IsRequired();

            builder
                .HasIndex(p => p.ObjectGuid)
                .HasDatabaseName("IX_History_ObjectGuid_ASC");

            builder.HasCheckConstraint("constraint_history_check_valid_event_type", $"{nameof(History.EventType)} in ({GetValidEventTypes()})");
        }

        private string GetValidEventTypes()
        {
            var names = Enum.GetNames(typeof(EventType)).Select(t => $"'{t}'");
            return string.Join(',', names);
        }
    }
}

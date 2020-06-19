using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Microsoft.EntityFrameworkCore;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
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

            builder.HasOne<PreservationRecord>()
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasIndex(p => p.ObjectGuid)
                .HasName("IX_History_ObjectGuid_ASC");

            builder.HasCheckConstraint("constraint_history_check_valid_event_type", $"{nameof(History.EventType)} in ({GetValidEventTypes()})");
        }

        private string GetValidEventTypes()
        {
            var names = Enum.GetNames(typeof(EventType)).Select(t => $"'{t}'");
            return string.Join(',', names);
        }
    }
}

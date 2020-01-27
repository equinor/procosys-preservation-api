using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.Entityconfigurations
{
    internal class PreservationPeriodConfiguration : IEntityTypeConfiguration<PreservationPeriod>
    {
        public void Configure(EntityTypeBuilder<PreservationPeriod> builder)
        {
            builder.Property(f => f.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();

            builder.Property(x => x.Comment)
                .HasMaxLength(PreservationPeriod.CommentLengthMax);

            builder
                .HasOne(p => p.PreservationRecord);

            builder.Property(f => f.Status)
                .HasConversion<string>()
                .HasDefaultValue(PreservationPeriodStatus.NeedUserInput)
                .IsRequired();

            // todo datetime converter for DueTimeUtc

            builder.HasCheckConstraint("constraint_period_check_valid_status", $"{nameof(Tag.Status)} in ({GetValidStatuses()})");
        }

        private string GetValidStatuses()
        {
            var fieldTypes = Enum.GetNames(typeof(PreservationPeriodStatus)).Select(t => $"'{t}'");
            return string.Join(',', fieldTypes);
        }
    }
}

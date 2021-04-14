using System;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations
{
    internal class PreservationPeriodConfiguration : IEntityTypeConfiguration<PreservationPeriod>
    {
        public void Configure(EntityTypeBuilder<PreservationPeriod> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();
            builder.ConfigureConcurrencyToken();

            builder.Property(x => x.Comment)
                .HasMaxLength(PreservationPeriod.CommentLengthMax);

            builder
                .HasOne(p => p.PreservationRecord);

            builder.Property(f => f.Status)
                .HasConversion<string>()
                .HasDefaultValue(PreservationPeriodStatus.NeedsUserInput)
                .IsRequired();
                        
            builder
                .HasMany(x => x.FieldValues)
                .WithOne()
                .IsRequired();

            builder.Property(x => x.DueTimeUtc)
                .HasConversion(PreservationContext.DateTimeKindConverter);

            builder.HasCheckConstraint("constraint_period_check_valid_status", $"{nameof(PreservationPeriod.Status)} in ({GetValidStatuses()})");

            builder
                .HasIndex(x => x.Plant)
                .HasDatabaseName("IX_PreservationPeriods_Plant_ASC")
                .IncludeProperties(x => new {x.Comment, x.CreatedAtUtc, x.DueTimeUtc, x.ModifiedAtUtc, x.Status});
        }

        private string GetValidStatuses()
        {
            var fieldTypes = Enum.GetNames(typeof(PreservationPeriodStatus)).Select(t => $"'{t}'");
            return string.Join(',', fieldTypes);
        }
    }
}

using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.Entityconfigurations
{
    internal class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.Property(x => x.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();

            builder.Property(x => x.ProjectName)
                .HasMaxLength(Tag.ProjectNameLengthMax)
                .IsRequired();

            builder.Property(x => x.TagNo)
                .HasMaxLength(Tag.TagNoLengthMax)
                .IsRequired();

            builder
                .HasMany(x => x.Requirements)
                .WithOne()
                .IsRequired();

            builder.Property(f => f.Status)
                .HasConversion<string>()
                .HasDefaultValue(PreservationStatus.NotStarted)
                .IsRequired();

            builder.HasCheckConstraint("constraint_tag_check_valid_status", $"{nameof(Tag.Status)} in ({GetValidStatuses()})");
        }

        private string GetValidStatuses()
        {
            var fieldTypes = Enum.GetNames(typeof(PreservationStatus)).Select(t => $"'{t}'");
            return string.Join(',', fieldTypes);
        }
    }
}

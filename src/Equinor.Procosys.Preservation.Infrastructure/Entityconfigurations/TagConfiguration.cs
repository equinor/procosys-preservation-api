using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
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

            builder.Property(x => x.Description)
                .HasMaxLength(Tag.DescriptionLengthMax)
                .IsRequired();

            builder.Property(x => x.Remark)
                .HasMaxLength(Tag.RemarkLengthMax);

            builder.Property(x => x.TagNo)
                .HasMaxLength(Tag.TagNoLengthMax)
                .IsRequired();

            builder.Property(x => x.AreaCode)
                .HasMaxLength(Tag.AreaCodeLengthMax);

            builder.Property(x => x.DisciplineCode)
                .HasMaxLength(Tag.DisciplineCodeLengthMax);

            builder
                .HasMany(x => x.Requirements)
                .WithOne()
                .IsRequired();

            builder.Property(f => f.Status)
                .HasConversion<string>()
                .HasDefaultValue(PreservationStatus.NotStarted)
                .IsRequired();

            builder.Property(f => f.TagType)
                .HasConversion<string>()
                .HasDefaultValue(TagType.Standard)
                .IsRequired();

            builder.HasCheckConstraint("constraint_tag_check_valid_status", $"{nameof(Tag.Status)} in ({GetValidStatuses()})");
            
            builder.HasCheckConstraint("constraint_tag_check_valid_tag_type", $"{nameof(Tag.TagType)} in ({GetValidTagTypes()})");
        }

        private string GetValidStatuses()
        {
            var fieldTypes = Enum.GetNames(typeof(PreservationStatus)).Select(t => $"'{t}'");
            return string.Join(',', fieldTypes);
        }

        private string GetValidTagTypes()
        {
            var fieldTypes = Enum.GetNames(typeof(TagType)).Select(t => $"'{t}'");
            return string.Join(',', fieldTypes);
        }
    }
}

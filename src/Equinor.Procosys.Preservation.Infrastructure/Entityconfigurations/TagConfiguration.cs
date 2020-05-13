using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();
            builder.ConfigureConcurrencyToken();

            builder.Property(x => x.Description)
                .HasMaxLength(Tag.DescriptionLengthMax)
                .IsRequired();

            builder.Property(x => x.Remark)
                .HasMaxLength(Tag.RemarkLengthMax);

            builder.Property(x => x.StorageArea)
                .HasMaxLength(Tag.StorageAreaLengthMax);

            builder.Property(x => x.TagNo)
                .HasMaxLength(Tag.TagNoLengthMax)
                .IsRequired();

            builder.Property(x => x.TagFunctionCode)
                .HasMaxLength(Tag.TagFunctionCodeLengthMax);

            builder.Property(x => x.AreaCode)
                .HasMaxLength(Tag.AreaCodeLengthMax);

            builder.Property(x => x.AreaDescription)
                .HasMaxLength(Tag.AreaDescriptionLengthMax);

            builder.Property(x => x.DisciplineCode)
                .HasMaxLength(Tag.DisciplineCodeLengthMax);

            builder.Property(x => x.DisciplineDescription)
                .HasMaxLength(Tag.DisciplineDescriptionLengthMax);

            builder.Property(x => x.PurchaseOrderNo)
                .HasMaxLength(Tag.PurchaseOrderNoLengthMax);

            builder.Property(x => x.McPkgNo)
                .HasMaxLength(Tag.McPkgNoLengthMax);

            builder.Property(x => x.CommPkgNo)
                .HasMaxLength(Tag.CommPkgNoLengthMax);

            builder.Property(x => x.Calloff)
                .HasMaxLength(Tag.CalloffLengthMax);

            builder.HasOne<Step>()
                .WithMany()
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(x => x.Requirements)
                .WithOne()
                .IsRequired();

            builder
                .HasMany(x => x.Actions)
                .WithOne()
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasMany(x => x.Attachments)
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
            
            builder.Property(x => x.NextDueTimeUtc)
                .HasConversion(PreservationContext.NullableDateTimeKindConverter);

            builder.HasCheckConstraint("constraint_tag_check_valid_status", $"{nameof(Tag.Status)} in ({GetValidStatuses()})");

            builder.HasCheckConstraint("constraint_tag_check_valid_tag_type", $"{nameof(Tag.TagType)} in ({GetValidTagTypes()})");

            builder
                .HasIndex(x => x.Plant)
                .HasName("IX_Tags_Plant_ASC")
                .IncludeProperties(x => x.TagNo);

            builder
                .HasIndex(x => x.TagNo)
                .HasName("IX_Tags_TagNo_ASC")
                .IncludeProperties(x => new
                {
                    x.AreaCode,
                    x.Calloff,
                    x.CommPkgNo,
                    x.Description,
                    x.CreatedAtUtc,
                    x.DisciplineCode,
                    x.IsVoided,
                    x.McPkgNo,
                    x.NextDueTimeUtc,
                    x.PurchaseOrderNo,
                    x.Status,
                    x.StorageArea,
                    x.TagFunctionCode,
                    x.TagType
                }); 
            
            builder
                .HasIndex(x => x.CommPkgNo)
                .HasName("IX_Tags_CommPkgNo_ASC")
                .IncludeProperties(x => new
                {
                    x.AreaCode,
                    x.Calloff,
                    x.Description,
                    x.CreatedAtUtc,
                    x.DisciplineCode,
                    x.IsVoided,
                    x.McPkgNo,
                    x.NextDueTimeUtc,
                    x.PurchaseOrderNo,
                    x.Status,
                    x.StorageArea,
                    x.TagFunctionCode,
                    x.TagNo,
                    x.TagType
                });

            builder
                .HasIndex(x => x.McPkgNo)
                .HasName("IX_Tags_McPkgNo_ASC")
                .IncludeProperties(x => new
                {
                    x.AreaCode,
                    x.Calloff,
                    x.Description,
                    x.CommPkgNo,
                    x.CreatedAtUtc,
                    x.DisciplineCode,
                    x.IsVoided,
                    x.NextDueTimeUtc,
                    x.PurchaseOrderNo,
                    x.Status,
                    x.StorageArea,
                    x.TagFunctionCode,
                    x.TagNo,
                    x.TagType
                });

            builder
                .HasIndex(x => x.Calloff)
                .HasName("IX_Tags_Calloff_ASC")
                .IncludeProperties(x => new
                {
                    x.AreaCode,
                    x.CommPkgNo,
                    x.CreatedAtUtc,
                    x.Description,
                    x.DisciplineCode,
                    x.IsVoided,
                    x.McPkgNo,
                    x.NextDueTimeUtc,
                    x.PurchaseOrderNo,
                    x.Status,
                    x.StorageArea,
                    x.TagFunctionCode,
                    x.TagNo,
                    x.TagType
                });

            builder
                .HasIndex(x => x.PurchaseOrderNo)
                .HasName("IX_Tags_PurchaseOrderNo_ASC")
                .IncludeProperties(x => new
                {
                    x.AreaCode,
                    x.Calloff,
                    x.CommPkgNo,
                    x.CreatedAtUtc,
                    x.Description,
                    x.DisciplineCode,
                    x.IsVoided,
                    x.McPkgNo,
                    x.NextDueTimeUtc,
                    x.Status,
                    x.StorageArea,
                    x.TagFunctionCode,
                    x.TagNo,
                    x.TagType
                });

            builder
                .HasIndex(x => x.StorageArea)
                .HasName("IX_Tags_StorageArea_ASC")
                .IncludeProperties(x => new
                {
                    x.AreaCode,
                    x.Calloff,
                    x.CommPkgNo,
                    x.CreatedAtUtc,
                    x.Description,
                    x.DisciplineCode,
                    x.IsVoided,
                    x.McPkgNo,
                    x.NextDueTimeUtc,
                    x.PurchaseOrderNo,
                    x.Status,
                    x.TagFunctionCode,
                    x.TagNo,
                    x.TagType
                });

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

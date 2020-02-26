using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class FieldConfiguration : EntityBaseConfiguration<Field>
    {
        public override void Configure(EntityTypeBuilder<Field> builder)
        {
            base.Configure(builder);

            builder.Property(f => f.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();

            builder.Property(f => f.Label)
                .HasMaxLength(Field.LabelLengthMax)
                .IsRequired();

            builder.Property(f => f.Unit)
                .HasMaxLength(Field.UnitLengthMax);

            builder.Property(f => f.FieldType)
                .HasConversion<string>()
                .HasDefaultValue(FieldType.Info)
                .IsRequired();

            builder.HasMany<FieldValue>()
                .WithOne()
                .HasForeignKey(pr => pr.FieldId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            builder.HasCheckConstraint("constraint_field_check_valid_fieldtype", $"{nameof(Field.FieldType)} in ({GetValidFieldTypes()})");
        }

        private string GetValidFieldTypes()
        {
            var fieldTypes = Enum.GetNames(typeof(FieldType)).Select(t => $"'{t}'");
            return string.Join(',', fieldTypes);
        }
    }
}

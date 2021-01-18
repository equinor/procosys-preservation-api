using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class StepConfiguration : IEntityTypeConfiguration<Step>
    {
        public void Configure(EntityTypeBuilder<Step> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();
            builder.ConfigureConcurrencyToken();

            builder.HasOne<Mode>()
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.HasOne<Responsible>()
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(x => x.Title)
                .HasMaxLength(Step.TitleLengthMax)
                .IsRequired();

            builder
                .HasIndex(x => x.Plant)
                .HasDatabaseName("IX_Steps_Plant_ASC")
                .IncludeProperties(x => new {x.CreatedAtUtc, x.IsVoided, x.ModifiedAtUtc, x.SortKey, x.Title});

            builder.Property(f => f.AutoTransferMethod)
                .HasConversion<string>()
                .HasDefaultValue(AutoTransferMethod.None)
                .IsRequired();

            builder.HasCheckConstraint("constraint_step_check_valid_auto_transfer", $"{nameof(Step.AutoTransferMethod)} in ({GetValidAutoTransferMethods()})");
        }

        private string GetValidAutoTransferMethods()
        {
            var names = Enum.GetNames(typeof(AutoTransferMethod)).Select(t => $"'{t}'");
            return string.Join(',', names);
        }
    }
}

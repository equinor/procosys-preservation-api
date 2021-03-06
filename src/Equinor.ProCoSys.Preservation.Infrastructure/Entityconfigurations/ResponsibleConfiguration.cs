﻿using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.ProCoSys.Preservation.Infrastructure.EntityConfigurations
{
    internal class ResponsibleConfiguration : IEntityTypeConfiguration<Responsible>
    {
        public void Configure(EntityTypeBuilder<Responsible> builder)
        {
            builder.ConfigurePlant();
            builder.ConfigureCreationAudit();
            builder.ConfigureModificationAudit();
            builder.ConfigureConcurrencyToken();

            builder.Property(x => x.Code)
                .HasMaxLength(Responsible.CodeLengthMax)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(Responsible.DescriptionLengthMax)
                .IsRequired();

            builder
                .HasIndex(x => x.Plant)
                .HasDatabaseName("IX_Responsibles_Plant_ASC")
                .IncludeProperties(x => new {x.CreatedAtUtc, x.IsVoided, x.ModifiedAtUtc, Title = x.Description});
        }
    }
}

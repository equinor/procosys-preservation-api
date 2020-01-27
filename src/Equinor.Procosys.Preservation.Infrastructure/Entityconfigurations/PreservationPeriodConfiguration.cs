﻿using Equinor.Procosys.Preservation.Domain;
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

            // todo datetime converter for DueDateUtc
        }
    }
}

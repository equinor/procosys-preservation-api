﻿using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.Entityconfigurations
{
    internal class ResponsibleConfiguration : IEntityTypeConfiguration<Responsible>
    {
        public void Configure(EntityTypeBuilder<Responsible> builder)
        {
            builder.Property(x => x.Schema)
                .HasMaxLength(Responsible.SchemaLengthMax)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(Responsible.NameLengthMax)
                .IsRequired();
        }
    }
}

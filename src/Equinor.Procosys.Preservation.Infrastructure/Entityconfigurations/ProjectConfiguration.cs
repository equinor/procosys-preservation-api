﻿using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.Entityconfigurations
{
    internal class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.Property(x => x.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(Project.NameLengthMax)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(Project.DescriptionLengthMax)
                .IsRequired();
            
            builder
                .HasMany(x => x.Tags)
                .WithOne()
                .IsRequired();
        }
    }
}

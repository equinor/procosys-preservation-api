using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.Entityconfigurations
{
    internal class ActionCommentConfiguration : IEntityTypeConfiguration<ActionComment>
    {
        public void Configure(EntityTypeBuilder<ActionComment> builder)
        {
            builder.Property(f => f.Schema)
                .HasMaxLength(SchemaEntityBase.SchemaLengthMax)
                .IsRequired();
            
            builder.Property(x => x.Comment)
                .HasMaxLength(ActionComment.CommentLengthMax)
                .IsRequired();

            builder.Property(x => x.CommentedAtUtc)
                .HasConversion(PreservationContext.DateTimeKindConverter);
        }
    }
}

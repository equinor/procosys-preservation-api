using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.Entityconfigurations
{
    internal class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.Property(x => x.Oid)
                .IsRequired();

            builder.Property(x => x.FirstName)
                .HasMaxLength(Person.FirstNameLengthMax)
                .IsRequired();

            builder.Property(x => x.LastName)
                .HasMaxLength(Person.LastNameLengthMax)
                .IsRequired();

            builder.HasMany<PreservationRecord>()
                .WithOne()
                .HasForeignKey(pr => pr.PreservedByPersonId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            builder.HasMany<Action>()
                .WithOne()
                .HasForeignKey(a => a.ClosedById)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            builder.HasMany<ActionComment>()
                .WithOne()
                .HasForeignKey(a => a.CommentedById)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        }
    }
}

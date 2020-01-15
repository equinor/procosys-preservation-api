using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
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
                .HasForeignKey(pr => pr.PreservedBy)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);        }
    }
}

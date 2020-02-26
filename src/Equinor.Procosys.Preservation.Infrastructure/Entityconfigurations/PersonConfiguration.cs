using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.Procosys.Preservation.Infrastructure.EntityConfigurations
{
    internal class PersonConfiguration : EntityBaseConfiguration<Person>
    {
        public override void Configure(EntityTypeBuilder<Person> builder)
        {
            base.Configure(builder);

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
                .HasForeignKey(pr => pr.PreservedById)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            builder.HasMany<Action>()
                .WithOne()
                .HasForeignKey(a => a.CreatedById)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

            builder.HasMany<Action>()
                .WithOne()
                .HasForeignKey(a => a.ClosedById)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);
        }
    }
}

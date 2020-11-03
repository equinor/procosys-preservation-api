using System;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Infrastructure;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests
{
    public static class PreservationContextExtension
    {
        public static void Seed(
            this PreservationContext dbContext,
            ICurrentUserProvider userProvider,
            IPlantProvider plantProvider
            )
        {
            /* 
             * Add the initial seeder user. Don't do this through the UnitOfWork as this expects/requires the current user to exist in the database.
             * This is the first user that is added to the database and will not get "Created" and "CreatedBy" data.
             */
            dbContext.Persons.Add(new Person(userProvider.GetCurrentUserOid(), "Siri", "Seed"));
            dbContext.SaveChangesAsync().Wait();

            dbContext.Modes.Add(new Mode(plantProvider.Plant, Guid.NewGuid().ToString(), false));
            dbContext.SaveChangesAsync().Wait();
        }
    }
}

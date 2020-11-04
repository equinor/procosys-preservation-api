using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Equinor.Procosys.Preservation.WebApi.Seeding
{
    public class Seeder : IHostedService
    {
        private static readonly Person _seederUser = new Person(new Guid("12345678-1234-1234-1234-123456789123"), "Angus", "MacGyver");
        private readonly IServiceScopeFactory _serviceProvider;

        public Seeder(IServiceScopeFactory serviceProvider) => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var plantProvider = new SeedingPlantProvider("PCS$SEED");

                using (var dbContext = new PreservationContext(
                    scope.ServiceProvider.GetRequiredService<DbContextOptions<PreservationContext>>(),
                    plantProvider,
                    scope.ServiceProvider.GetRequiredService<IEventDispatcher>(),
                    new SeederUserProvider()))
                {
                    // If the seeder user exists in the database, it's already been seeded. Don't seed again.
                    if (await dbContext.Persons.AnyAsync(p => p.Oid == _seederUser.Oid, cancellationToken))
                    {
                        return;
                    }

                    /* 
                     * Add the initial seeder user. Don't do this through the UnitOfWork as this expects/requires the current user to exist in the database.
                     * This is the first user that is added to the database and will not get "Created" and "CreatedBy" data.
                     */
                    dbContext.Persons.Add(_seederUser);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    var personRepository = new PersonRepository(dbContext);
                    var journeyRepository = new JourneyRepository(dbContext);
                    var modeRepository = new ModeRepository(dbContext);
                    var responsibleRepository = new ResponsibleRepository(dbContext);
                    var requirementTypeRepository = new RequirementTypeRepository(dbContext);
                    var projectRepository = new ProjectRepository(dbContext);

                    personRepository.AddUsers(250);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    responsibleRepository.AddResponsibles(250, plantProvider.Plant);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    modeRepository.AddModes(250, plantProvider.Plant);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    var modes = await modeRepository.GetAllAsync();
                    var responsibles = await responsibleRepository.GetAllAsync();
                    journeyRepository.AddJourneys(250, plantProvider.Plant, modes, responsibles);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    requirementTypeRepository.AddRequirementTypes(250, plantProvider.Plant);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    projectRepository.AddProjects(50, plantProvider.Plant);
                    await dbContext.SaveChangesAsync(cancellationToken);
                    var projects = await projectRepository.GetAllAsync();

                    var steps = (await journeyRepository.GetAllAsync()).SelectMany(x => x.Steps).ToList();
                    var requirementDefinitions = (await requirementTypeRepository.GetAllAsync()).SelectMany(x => x.RequirementDefinitions).ToList();

                    projects.AddTags(100, plantProvider.Plant, steps, requirementDefinitions);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }
            }


        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private class SeederUserProvider : ICurrentUserProvider
        {
            public Guid GetCurrentUserOid() => _seederUser.Oid;
        }
    }
}

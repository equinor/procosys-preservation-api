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
        private readonly IServiceScopeFactory _serviceProvider;

        public Seeder(IServiceScopeFactory serviceProvider) => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var plantProvider = new SeedingPlantProvider("PCS$SEED");

                using (var dbContext = new PreservationContext(
                    scope.ServiceProvider.GetRequiredService<DbContextOptions<PreservationContext>>(),
                    scope.ServiceProvider.GetRequiredService<IEventDispatcher>(),
                    plantProvider,
                    scope.ServiceProvider.GetRequiredService<ITimeService>(),
                    scope.ServiceProvider.GetRequiredService<ICurrentUserProvider>()))
                {
                    var unitOfWork = dbContext;
                    var personRepository = new PersonRepository(dbContext);
                    var journeyRepository = new JourneyRepository(dbContext);
                    var modeRepository = new ModeRepository(dbContext);
                    var responsibleRepository = new ResponsibleRepository(dbContext);
                    var requirementTypeRepository = new RequirementTypeRepository(dbContext);
                    var projectRepository = new ProjectRepository(dbContext);

                    personRepository.AddUsers(250);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    responsibleRepository.AddResponsibles(250, plantProvider.Plant);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    modeRepository.AddModes(250, plantProvider.Plant);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    var modes = await modeRepository.GetAllAsync();
                    var responsibles = await responsibleRepository.GetAllAsync();
                    journeyRepository.AddJourneys(250, plantProvider.Plant, modes, responsibles);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    requirementTypeRepository.AddRequirementTypes(250, plantProvider.Plant);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    projectRepository.AddProjects(50, plantProvider.Plant);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                    var projects = await projectRepository.GetAllAsync();

                    var steps = (await journeyRepository.GetAllAsync()).SelectMany(x => x.Steps).ToList();
                    var requirementDefinitions = (await requirementTypeRepository.GetAllAsync()).SelectMany(x => x.RequirementDefinitions).ToList();
                    
                    projects.AddTags(100, plantProvider.Plant, steps, requirementDefinitions);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private class SeederUserProvider : ICurrentUserProvider
        {
            public Task<Person> GetCurrentUserAsync() => Task.FromResult(new Person(Guid.NewGuid(), "Seeder", "Seederson"));
        }
    }
}

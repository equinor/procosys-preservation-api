using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Infrastructure;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests
{
    public static class PreservationContextExtension
    {
        public static void Seed(
            this PreservationContext dbContext,
            ICurrentUserProvider userProvider,
            IPlantProvider plantProvider,
            string projectName
            )
        {
            /* 
             * Add the initial seeder user. Don't do this through the UnitOfWork as this expects/requires the current user to exist in the database.
             * This is the first user that is added to the database and will not get "Created" and "CreatedBy" data.
             */
            dbContext.Persons.Add(new Person(userProvider.GetCurrentUserOid(), "Siri", "Seed"));
            dbContext.SaveChangesAsync().Wait();

            var plant = plantProvider.Plant;

            // todo refactor guids to be known data from TestFactory
            var mode = new Mode(plant, Guid.NewGuid().ToString(), false);
            dbContext.Modes.Add(mode);
            dbContext.SaveChangesAsync().Wait();

            var responsible = new Responsible(plant, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            dbContext.Responsibles.Add(responsible);
            dbContext.SaveChangesAsync().Wait();

            var requirementType = new RequirementType(
                plant,
                "IR",
                Guid.NewGuid().ToString(),
                RequirementTypeIcon.Other, 
                10);
            var requirementDef = new RequirementDefinition(plant, Guid.NewGuid().ToString(), 4, RequirementUsage.ForAll, 10);
            requirementType.AddRequirementDefinition(requirementDef);
            dbContext.RequirementTypes.Add(requirementType);
            dbContext.SaveChangesAsync().Wait();

            var journey = new Journey(plant, Guid.NewGuid().ToString());
            var step = new Step(plant, Guid.NewGuid().ToString(), mode, responsible);
            journey.AddStep(step);
            dbContext.Journeys.Add(journey);
            dbContext.SaveChangesAsync().Wait();

            var project = new Project(plant, projectName, Guid.NewGuid().ToString());
            var siteTag = new Tag(
                plant, 
                TagType.SiteArea, 
                Guid.NewGuid().ToString(), 
                Guid.NewGuid().ToString(), 
                step,
                new List<TagRequirement>
                {
                    new TagRequirement(plant, 4, requirementDef)
                });
            project.AddTag(siteTag);
            dbContext.Projects.Add(project);
            dbContext.SaveChangesAsync().Wait();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Infrastructure.Repositories;
using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests
{
    public static class PreservationContextExtension
    {
        private static string _seederOid = "00000000-0000-0000-0000-999999999999";

        public static void CreateNewDatabaseWithCorrectSchema(this PreservationContext dbContext)
        {
            var migrations = dbContext.Database.GetPendingMigrations();
            if (migrations.Any())
            {
                dbContext.Database.Migrate();
            }
        }

        public static void Seed(this PreservationContext dbContext, IServiceProvider serviceProvider)
        {
            var userProvider = serviceProvider.GetRequiredService<CurrentUserProvider>();
            var plantProvider = serviceProvider.GetRequiredService<PlantProvider>();
            userProvider.SetCurrentUserOid(new Guid(_seederOid));
            plantProvider.SetPlant(SeedingData.Plant);
            
            /* 
             * Add the initial seeder user. Don't do this through the UnitOfWork as this expects/requires the current user to exist in the database.
             * This is the first user that is added to the database and will not get "Created" and "CreatedBy" data.
             */
            SeedCurrentUserAsPerson(dbContext, userProvider);

            var plant = plantProvider.Plant;

            var mode = SeedModes(dbContext, plant);

            var responsible = SeedResponsibles(dbContext, plant);

            var requirementDef = SeedRequirements(dbContext, plant);

            var step = SeedJourneys(dbContext, plant, mode, responsible);

            SeedTags(dbContext, plant, step, requirementDef);
        }

        private static void SeedCurrentUserAsPerson(PreservationContext dbContext, ICurrentUserProvider userProvider)
        {
            var personRepository = new PersonRepository(dbContext);
            personRepository.Add(new Person(userProvider.GetCurrentUserOid(), "Siri", "Seed"));
            dbContext.SaveChangesAsync().Wait();
        }

        private static void SeedTags(PreservationContext dbContext, string plant, Step step,
            RequirementDefinition requirementDef)
        {
            var projectRepository = new ProjectRepository(dbContext);
            var project = new Project(plant, SeedingData.ProjectCode, SeedingData.ProjectDescription);
            var siteTag = new Tag(
                plant,
                TagType.SiteArea,
                SeedingData.SiteTagNo,
                SeedingData.SiteTagDescription,
                step,
                new List<TagRequirement>
                {
                    new TagRequirement(plant, 4, requirementDef)
                });
            siteTag.SetArea("A","A-D");
            siteTag.SetDiscipline("D","D-D");
            project.AddTag(siteTag);
            projectRepository.Add(project);
            dbContext.SaveChangesAsync().Wait();
        }

        private static Step SeedJourneys(PreservationContext dbContext, string plant, Mode mode, Responsible responsible)
        {
            var journeyRepository = new JourneyRepository(dbContext);
            var journey = new Journey(plant, SeedingData.Journey);
            var step = new Step(plant, SeedingData.Step, mode, responsible);
            journey.AddStep(step);
            journeyRepository.Add(journey);
            dbContext.SaveChangesAsync().Wait();
            return step;
        }

        private static RequirementDefinition SeedRequirements(PreservationContext dbContext, string plant)
        {
            var requirementTypeRepository = new RequirementTypeRepository(dbContext);
            var requirementType = new RequirementType(
                plant,
                SeedingData.RequirementTypeCode,
                SeedingData.RequirementTypeDescription,
                RequirementTypeIcon.Other,
                10);
            var requirementDef =
                new RequirementDefinition(plant, SeedingData.RequirementDefinition, 4, RequirementUsage.ForAll, 10);
            requirementType.AddRequirementDefinition(requirementDef);
            requirementTypeRepository.Add(requirementType);
            dbContext.SaveChangesAsync().Wait();
            return requirementDef;
        }

        private static Responsible SeedResponsibles(PreservationContext dbContext, string plant)
        {
            var responsibleRepository = new ResponsibleRepository(dbContext);
            var responsible = new Responsible(plant, SeedingData.ResponsibleCode, SeedingData.ResponsibleDescription);
            responsibleRepository.Add(responsible);
            dbContext.SaveChangesAsync().Wait();
            return responsible;
        }

        private static Mode SeedModes(PreservationContext dbContext, string plant)
        {
            var modeRepository = new ModeRepository(dbContext);
            var mode = new Mode(plant, SeedingData.Mode, false);
            modeRepository.Add(mode);
            dbContext.SaveChangesAsync().Wait();
            return mode;
        }
    }
}

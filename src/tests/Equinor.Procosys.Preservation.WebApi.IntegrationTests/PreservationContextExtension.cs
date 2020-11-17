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
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

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

        public static void Seed(this PreservationContext dbContext, IServiceProvider serviceProvider, KnownTestData knownTestData)
        {
            var userProvider = serviceProvider.GetRequiredService<CurrentUserProvider>();
            var plantProvider = serviceProvider.GetRequiredService<PlantProvider>();
            userProvider.SetCurrentUserOid(new Guid(_seederOid));
            plantProvider.SetPlant(KnownTestData.Plant);
            
            /* 
             * Add the initial seeder user. Don't do this through the UnitOfWork as this expects/requires the current user to exist in the database.
             * This is the first user that is added to the database and will not get "Created" and "CreatedBy" data.
             */
            SeedCurrentUserAsPerson(dbContext, userProvider);

            var plant = plantProvider.Plant;

            var mode = SeedMode(dbContext, plant);

            var responsible = SeedResponsible(dbContext, plant);

            var requirementDef = SeedRequirement(dbContext, plant);

            var journey = SeedJourney(dbContext, plant);
            var step = SeedStep(dbContext, journey, mode, responsible);
            knownTestData.StepIds.Add(step.Id);

            var project = SeedProject(dbContext, plant);
            var standardTag = SeedStandardTag(dbContext, project, step, requirementDef);
            knownTestData.StandardTagIds.Add(standardTag.Id);

            var standardTagAttachment = SeedTagAttachment(dbContext, standardTag);
            knownTestData.StandardTagAttachmentIds.Add(standardTagAttachment.Id);

            var standardTagAction = SeedAction(dbContext, standardTag);
            knownTestData.StandardTagActionIds.Add(standardTagAction.Id);
            var standardTagActionAttachment = SeedActionAttachment(dbContext, standardTagAction);
            knownTestData.StandardTagActionAttachmentIds.Add(standardTagActionAttachment.Id);

            var siteAreaTag = SeedSiteTag(dbContext, project, step, requirementDef);
            knownTestData.SiteAreaTagIds.Add(siteAreaTag.Id);

            var siteAreaTagAttachment = SeedTagAttachment(dbContext, siteAreaTag);
            knownTestData.SiteAreaTagAttachmentIds.Add(siteAreaTagAttachment.Id);

            var areaTagAction = SeedAction(dbContext, siteAreaTag);
            knownTestData.SiteAreaTagActionIds.Add(areaTagAction.Id);
            var areaTagActionAttachment = SeedActionAttachment(dbContext, areaTagAction);
            knownTestData.SiteAreaTagActionAttachmentIds.Add(areaTagActionAttachment.Id);
        }

        private static TagAttachment SeedTagAttachment(PreservationContext dbContext, Tag tag)
        {
            var attachment = new TagAttachment(tag.Plant, KnownTestData.TagAttachmentBlobStorageId, "Fil1.txt");
            tag.AddAttachment(attachment);
            dbContext.SaveChangesAsync().Wait();
            return attachment;
        }

        private static ActionAttachment SeedActionAttachment(PreservationContext dbContext, Action action)
        {
            var attachment = new ActionAttachment(action.Plant, KnownTestData.ActionAttachmentBlobStorageId, "Fil2.txt");
            action.AddAttachment(attachment);
            dbContext.SaveChangesAsync().Wait();
            return attachment;
        }

        private static Action SeedAction(PreservationContext dbContext, Tag tag)
        {
            var action = new Action(tag.Plant, KnownTestData.Action, KnownTestData.ActionDescription, null);
            tag.AddAction(action);
            dbContext.SaveChangesAsync().Wait();
            return action;
        }

        private static void SeedCurrentUserAsPerson(PreservationContext dbContext, ICurrentUserProvider userProvider)
        {
            var personRepository = new PersonRepository(dbContext);
            personRepository.Add(new Person(userProvider.GetCurrentUserOid(), "Siri", "Seed"));
            dbContext.SaveChangesAsync().Wait();
        }

        private static Project SeedProject(PreservationContext dbContext, string plant)
        {
            var projectRepository = new ProjectRepository(dbContext);
            var project = new Project(plant, KnownTestData.ProjectName, KnownTestData.ProjectDescription);
            projectRepository.Add(project);
            dbContext.SaveChangesAsync().Wait();
            return project;
        }

        private static Tag SeedStandardTag(PreservationContext dbContext, Project project, Step step,
            RequirementDefinition requirementDef)
        {
            var tag = new Tag(
                project.Plant,
                TagType.Standard,
                KnownTestData.StandardTagNo,
                KnownTestData.StandardTagDescription,
                step,
                new List<TagRequirement>
                {
                    new TagRequirement(project.Plant, 4, requirementDef)
                });
            tag.SetArea("A","A-D");
            tag.SetDiscipline("D","D-D");
            project.AddTag(tag);
            dbContext.SaveChangesAsync().Wait();
            return tag;
        }

        private static Tag SeedSiteTag(PreservationContext dbContext, Project project, Step step,
            RequirementDefinition requirementDef)
        {
            var tag = new Tag(
                project.Plant,
                TagType.SiteArea,
                KnownTestData.SiteTagNo,
                KnownTestData.SiteTagDescription,
                step,
                new List<TagRequirement>
                {
                    new TagRequirement(project.Plant, 4, requirementDef)
                });
            tag.SetArea("A","A-D");
            tag.SetDiscipline("D","D-D");
            project.AddTag(tag);
            dbContext.SaveChangesAsync().Wait();
            return tag;
        }

        private static Journey SeedJourney(PreservationContext dbContext, string plant)
        {
            var journeyRepository = new JourneyRepository(dbContext);
            var journey = new Journey(plant, KnownTestData.Journey);
            journeyRepository.Add(journey);
            dbContext.SaveChangesAsync().Wait();
            return journey;
        }

        private static Step SeedStep(PreservationContext dbContext, Journey journey, Mode mode, Responsible responsible)
        {
            var step = new Step(journey.Plant, KnownTestData.Step, mode, responsible);
            journey.AddStep(step);
            dbContext.SaveChangesAsync().Wait();
            return step;
        }

        private static RequirementDefinition SeedRequirement(PreservationContext dbContext, string plant)
        {
            var requirementTypeRepository = new RequirementTypeRepository(dbContext);
            var requirementType = new RequirementType(
                plant,
                KnownTestData.RequirementTypeCode,
                KnownTestData.RequirementTypeDescription,
                RequirementTypeIcon.Other,
                10);
            var requirementDef =
                new RequirementDefinition(plant, KnownTestData.RequirementDefinition, 4, RequirementUsage.ForAll, 10);
            requirementType.AddRequirementDefinition(requirementDef);
            requirementTypeRepository.Add(requirementType);
            dbContext.SaveChangesAsync().Wait();
            return requirementDef;
        }

        private static Responsible SeedResponsible(PreservationContext dbContext, string plant)
        {
            var responsibleRepository = new ResponsibleRepository(dbContext);
            var responsible = new Responsible(plant, KnownTestData.ResponsibleCode, KnownTestData.ResponsibleDescription);
            responsibleRepository.Add(responsible);
            dbContext.SaveChangesAsync().Wait();
            return responsible;
        }

        private static Mode SeedMode(PreservationContext dbContext, string plant)
        {
            var modeRepository = new ModeRepository(dbContext);
            var mode = new Mode(plant, KnownTestData.Mode, false);
            modeRepository.Add(mode);
            dbContext.SaveChangesAsync().Wait();
            return mode;
        }
    }
}

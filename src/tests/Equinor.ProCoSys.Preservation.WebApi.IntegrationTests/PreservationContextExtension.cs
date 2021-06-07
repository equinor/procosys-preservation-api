using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Infrastructure.Repositories;
using Equinor.ProCoSys.Preservation.WebApi.Misc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests
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
            plantProvider.SetPlant(knownTestData.Plant);
            
            /* 
             * Add the initial seeder user. Don't do this through the UnitOfWork as this expects/requires the current user to exist in the database.
             * This is the first user that is added to the database and will not get "Created" and "CreatedBy" data.
             */
            var seeder = EnsureCurrentUserIsSeeded(dbContext, userProvider);

            var plant = plantProvider.Plant;

            var supplierMode = SeedMode(dbContext, plant, "SUP", true);
            var otherMode = SeedMode(dbContext, plant, "FAB", false);
            knownTestData.ModeId = otherMode.Id;

            var responsible = SeedResponsible(dbContext, plant);

            var reqTypeA = SeedReqType(dbContext, plant, KnownTestData.ReqTypeA);
            var reqDefANoField = SeedReqDef(dbContext, reqTypeA, KnownTestData.ReqDefInReqTypeANoField);

            var reqDefAWithAttachmentField = SeedReqDef(dbContext, reqTypeA, KnownTestData.ReqDefInReqTypeAWithAttachmentField);
            SeedAttachmentField(dbContext, reqDefAWithAttachmentField);

            var reqDefAWithInfoField = SeedReqDef(dbContext, reqTypeA, KnownTestData.ReqDefInReqTypeAWithInfoField);
            SeedInfoField(dbContext, reqDefAWithInfoField);

            var reqDefAWithCbField = SeedReqDef(dbContext, reqTypeA, KnownTestData.ReqDefInReqTypeAWithCbField);
            SeedCbField(dbContext, reqDefAWithCbField);

            var reqTypeB = SeedReqType(dbContext, plant, KnownTestData.ReqTypeB);
            SeedReqDef(dbContext, reqTypeB, KnownTestData.ReqDefInReqTypeB);
            
            var journeyWithTags = SeedJourney(dbContext, plant, KnownTestData.JourneyWithTags);
            var stepInJourneyWithTags = SeedStep(dbContext, journeyWithTags, KnownTestData.StepAInJourneyWithTags, supplierMode, responsible);
            SeedStep(dbContext, journeyWithTags, KnownTestData.StepBInJourneyWithTags, otherMode, responsible);
            
            var journeyWithoutTags = SeedJourney(dbContext, plant, KnownTestData.JourneyNotInUse);
            SeedStep(dbContext, journeyWithoutTags, KnownTestData.StepInJourneyNotInUse, supplierMode, responsible);

            var project = SeedProject(dbContext, plant);
            var standardTagReadyForBulkPreserveNotStarted = SeedStandardTag(dbContext, project, stepInJourneyWithTags, reqDefANoField);
            knownTestData.TagId_ForStandardTagReadyForBulkPreserve_NotStarted = standardTagReadyForBulkPreserveNotStarted.Id;

            var standardTagWithAttachmentRequirementStarted = SeedStandardTag(dbContext, project, stepInJourneyWithTags, reqDefAWithAttachmentField);
            standardTagWithAttachmentRequirementStarted.StartPreservation();
            dbContext.SaveChangesAsync().Wait();
            knownTestData.TagId_ForStandardTagWithAttachmentRequirement_Started = standardTagWithAttachmentRequirementStarted.Id;

            var standardTagWithInfoRequirementStarted = SeedStandardTag(dbContext, project, stepInJourneyWithTags, reqDefAWithInfoField);
            standardTagWithInfoRequirementStarted.StartPreservation();
            dbContext.SaveChangesAsync().Wait();
            knownTestData.TagId_ForStandardTagWithInfoRequirement_Started = standardTagWithInfoRequirementStarted.Id;

            var standardTagWithCbRequirementStarted = SeedStandardTag(dbContext, project, stepInJourneyWithTags, reqDefAWithCbField);
            standardTagWithCbRequirementStarted.StartPreservation();
            dbContext.SaveChangesAsync().Wait();
            knownTestData.TagId_ForStandardTagWithCbRequirement_Started = standardTagWithCbRequirementStarted.Id;

            var standardTagWithAttachmentsAndActionsStarted = SeedStandardTag(dbContext, project, stepInJourneyWithTags, reqDefANoField);
            standardTagWithAttachmentsAndActionsStarted.StartPreservation();
            knownTestData.TagId_ForStandardTagWithAttachmentsAndActionAttachments_Started = standardTagWithAttachmentsAndActionsStarted.Id;
            SeedTagAttachment(dbContext, standardTagWithAttachmentsAndActionsStarted);
            SeedTagAttachment(dbContext, standardTagWithAttachmentsAndActionsStarted);

            var standardTagAction = SeedAction(dbContext, standardTagWithAttachmentsAndActionsStarted);
            SeedActionAttachment(dbContext, standardTagAction);
            SeedActionAttachment(dbContext, standardTagAction);

            var closedStandardTagAction = SeedAction(dbContext, standardTagWithAttachmentsAndActionsStarted);
            closedStandardTagAction.Close(new DateTime(1971, 1, 1, 0, 0, 0, DateTimeKind.Utc), seeder);
            dbContext.SaveChangesAsync().Wait();

            var siteAreaTag = SeedSiteTag(dbContext, project, stepInJourneyWithTags, reqDefANoField);
            knownTestData.TagId_ForSiteAreaTagReadyForBulkPreserve_NotStarted = siteAreaTag.Id;

            var siteAreaTagWithAttachmentsAndActionAttachments = SeedSiteTag(dbContext, project, stepInJourneyWithTags, reqDefANoField);
            knownTestData.TagId_ForSiteAreaTagWithAttachmentsAndActionAttachments_NotStarted = siteAreaTagWithAttachmentsAndActionAttachments.Id;
            SeedTagAttachment(dbContext, siteAreaTagWithAttachmentsAndActionAttachments);
            SeedTagAttachment(dbContext, siteAreaTagWithAttachmentsAndActionAttachments);

            var closedAreaTagAction = SeedAction(dbContext, siteAreaTagWithAttachmentsAndActionAttachments);
            closedAreaTagAction.Close(new DateTime(1971, 1, 1, 0, 0, 0, DateTimeKind.Utc), seeder);
            dbContext.SaveChangesAsync().Wait();
            
            SeedActionAttachment(dbContext, closedAreaTagAction);
            SeedActionAttachment(dbContext, closedAreaTagAction);
            knownTestData.ActionId_ForActionWithAttachments_Closed = closedAreaTagAction.Id;
        }

        private static void SeedInfoField(PreservationContext dbContext, RequirementDefinition reqDef)
        {
            var infoField = new Field(reqDef.Plant, Guid.NewGuid().ToString(), FieldType.Info, 10);
            reqDef.AddField(infoField);
            dbContext.SaveChangesAsync().Wait();
        }

        private static void SeedCbField(PreservationContext dbContext, RequirementDefinition reqDef)
        {
            var cbField = new Field(reqDef.Plant, Guid.NewGuid().ToString(), FieldType.CheckBox, 10);
            reqDef.AddField(cbField);
            dbContext.SaveChangesAsync().Wait();
        }

        private static void SeedAttachmentField(PreservationContext dbContext, RequirementDefinition reqDef)
        {
            var attachmentField = new Field(reqDef.Plant, Guid.NewGuid().ToString(), FieldType.Attachment, 10);
            reqDef.AddField(attachmentField);
            dbContext.SaveChangesAsync().Wait();
        }

        private static void SeedTagAttachment(PreservationContext dbContext, Tag tag)
        {
            var attachment = new TagAttachment(tag.Plant, KnownTestData.TagAttachmentBlobStorageId, "Fil1.txt");
            tag.AddAttachment(attachment);
            dbContext.SaveChangesAsync().Wait();
        }

        private static void SeedActionAttachment(PreservationContext dbContext, Action action)
        {
            var attachment = new ActionAttachment(action.Plant, KnownTestData.ActionAttachmentBlobStorageId, "Fil2.txt");
            action.AddAttachment(attachment);
            dbContext.SaveChangesAsync().Wait();
        }

        private static Action SeedAction(PreservationContext dbContext, Tag tag)
        {
            var suffix = Guid.NewGuid().ToString().Substring(3,8);
            var title = $"{KnownTestData.Action}-{suffix}";
            var action = new Action(tag.Plant, title, $"{title}-Desc", new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            tag.AddAction(action);
            dbContext.SaveChangesAsync().Wait();
            return action;
        }

        private static Person EnsureCurrentUserIsSeeded(PreservationContext dbContext, ICurrentUserProvider userProvider)
        {
            var personRepository = new PersonRepository(dbContext);
            var person = personRepository.GetByOidAsync(userProvider.GetCurrentUserOid()).Result ??
                         SeedCurrentUserAsPerson(dbContext, userProvider);
            return person;
        }

        private static Person SeedCurrentUserAsPerson(PreservationContext dbContext, ICurrentUserProvider userProvider)
        {
            var personRepository = new PersonRepository(dbContext);
            var person = new Person(userProvider.GetCurrentUserOid(), "Siri", "Seed");
            personRepository.Add(person);
            dbContext.SaveChangesAsync().Wait();
            return person;
        }

        private static Project SeedProject(PreservationContext dbContext, string plant)
        {
            var projectRepository = new ProjectRepository(dbContext);
            var project = new Project(plant, KnownTestData.ProjectName, KnownTestData.ProjectDescription);
            projectRepository.Add(project);
            dbContext.SaveChangesAsync().Wait();
            return project;
        }

        private static Tag SeedStandardTag(
            PreservationContext dbContext,
            Project project,
            Step step,
            RequirementDefinition requirementDef)
        {
            var suffix = Guid.NewGuid().ToString().Substring(3,8);
            var tagNo = $"{KnownTestData.StandardTagNo}-{suffix}";
            var tag = new Tag(
                project.Plant,
                TagType.Standard,
                tagNo,
                $"{tagNo} - Description",
                step,
                new List<TagRequirement>
                {
                    new TagRequirement(project.Plant, 4, requirementDef)
                });
            FillTag(tag, suffix);
            project.AddTag(tag);
            dbContext.SaveChangesAsync().Wait();
            return tag;
        }

        private static Tag SeedSiteTag(PreservationContext dbContext, Project project, Step step,
            RequirementDefinition requirementDef)
        {
            var suffix = Guid.NewGuid().ToString().Substring(3,8);
            var tagNo = $"{KnownTestData.SiteTagNo}-{suffix}";
            var tag = new Tag(
                project.Plant,
                TagType.SiteArea,
                tagNo,
                $"{tagNo} - Description",
                step,
                new List<TagRequirement>
                {
                    new TagRequirement(project.Plant, 4, requirementDef)
                });
            FillTag(tag, suffix);
            project.AddTag(tag);
            dbContext.SaveChangesAsync().Wait();
            return tag;
        }

        private static void FillTag(Tag tag, string suffix)
        {
            tag.SetArea($"A-{suffix}", $"A-D-{suffix}");
            tag.SetDiscipline($"D-{suffix}", $"D-D-{suffix}");
            tag.McPkgNo = $"McP-{suffix}";
            tag.CommPkgNo = $"CoP-{suffix}";
            tag.PurchaseOrderNo = $"PO-{suffix}";
            tag.Calloff = $"CO-{suffix}";
            tag.Remark = $"Rem-{suffix}";
            tag.StorageArea = $"SA-{suffix}";
        }

        private static Journey SeedJourney(PreservationContext dbContext, string plant, string title)
        {
            var journeyRepository = new JourneyRepository(dbContext);
            var journey = new Journey(plant, title);
            journeyRepository.Add(journey);
            dbContext.SaveChangesAsync().Wait();
            return journey;
        }

        private static Step SeedStep(PreservationContext dbContext, Journey journey, string title, Mode mode, Responsible responsible)
        {
            var step = new Step(journey.Plant, title, mode, responsible);
            journey.AddStep(step);
            dbContext.SaveChangesAsync().Wait();
            return step;
        }

        private static RequirementType SeedReqType(PreservationContext dbContext, string plant, string code)
        {
            var reqTypeRepository = new RequirementTypeRepository(dbContext);
            var reqType = new RequirementType(
                plant,
                code,
                $"{code} - Description",
                RequirementTypeIcon.Other,
                10);
            reqTypeRepository.Add(reqType);
            dbContext.SaveChangesAsync().Wait();
            return reqType;
        }

        private static RequirementDefinition SeedReqDef(PreservationContext dbContext, RequirementType reqType, string title)
        {
            var reqDef =
                new RequirementDefinition(reqType.Plant, title, 4, RequirementUsage.ForAll, 10);
            reqType.AddRequirementDefinition(reqDef);
            dbContext.SaveChangesAsync().Wait();
            return reqDef;
        }

        private static Responsible SeedResponsible(PreservationContext dbContext, string plant)
        {
            var responsibleRepository = new ResponsibleRepository(dbContext);
            var responsible = new Responsible(plant, KnownTestData.ResponsibleCode, KnownTestData.ResponsibleDescription);
            responsibleRepository.Add(responsible);
            dbContext.SaveChangesAsync().Wait();
            return responsible;
        }

        private static Mode SeedMode(PreservationContext dbContext, string plant, string title, bool forSupplier)
        {
            var modeRepository = new ModeRepository(dbContext);
            var mode = new Mode(plant, title, forSupplier);
            modeRepository.Add(mode);
            dbContext.SaveChangesAsync().Wait();
            return mode;
        }
    }
}

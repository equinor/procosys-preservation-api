using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using HeboTech.TimeService;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Test.Common
{
    public abstract class ReadOnlyTestsBase
    {
        protected const string TestPlant = "PCS$PlantA";
        protected readonly Guid _currentUserOid = new Guid("12345678-1234-1234-1234-123456789123");
        protected DbContextOptions<PreservationContext> _dbContextOptions;
        protected Mock<IPlantProvider> _plantProviderMock;
        protected IPlantProvider _plantProvider;
        protected ICurrentUserProvider _currentUserProvider;
        protected IEventDispatcher _eventDispatcher;

        [TestInitialize]
        public void SetupBase()
        {
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock.SetupGet(x => x.Plant).Returns(TestPlant);
            _plantProvider = _plantProviderMock.Object;

            var currentUserProviderMock = new Mock<ICurrentUserProvider>();
            currentUserProviderMock.Setup(x => x.GetCurrentUserOid()).Returns(_currentUserOid);
            _currentUserProvider = currentUserProviderMock.Object;

            var eventDispatcher = new Mock<IEventDispatcher>();
            _eventDispatcher = eventDispatcher.Object;

            TimeService.SetConstant(new DateTime(2020, 2, 1, 0, 0, 0, DateTimeKind.Utc));

            _dbContextOptions = new DbContextOptionsBuilder<PreservationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            // ensure current user exists in db
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                if (context.Persons.SingleOrDefault(p => p.Oid == _currentUserOid) == null)
                {
                    AddPerson(context, _currentUserOid, "Ole", "Lukkøye");
                }
            }

            SetupNewDatabase(_dbContextOptions);
        }

        protected abstract void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions);

        protected Responsible AddResponsible(PreservationContext context, string code)
        {
            var responsible = new Responsible(TestPlant, code, $"{code}-Desc");
            context.Responsibles.Add(responsible);
            context.SaveChangesAsync().Wait();
            return responsible;
        }

        protected Mode AddMode(PreservationContext context, string title, bool forSupplier)
        {
            var mode = new Mode(TestPlant, title, forSupplier);
            context.Modes.Add(mode);
            context.SaveChangesAsync().Wait();
            return mode;
        }

        protected Journey AddJourneyWithStep(PreservationContext context, string journeyTitle, string stepTitle, Mode mode, Responsible responsible)
        {
            var journey = new Journey(TestPlant, journeyTitle);
            journey.AddStep(new Step(TestPlant, stepTitle, mode, responsible));
            context.Journeys.Add(journey);
            context.SaveChangesAsync().Wait();
            return journey;
        }

        protected Journey AddJourney(PreservationContext context, string journeyTitle)
        {
            var journey = new Journey(TestPlant, journeyTitle);
            context.Journeys.Add(journey);
            context.SaveChangesAsync().Wait();
            return journey;
        }

        protected RequirementType AddRequirementTypeWith1DefWithoutField(PreservationContext context, string typeCode, string defTitle, RequirementTypeIcon icon, int sortKey = 0)
        {
            var requirementType = new RequirementType(TestPlant, typeCode, $"Title{typeCode}", icon, sortKey);
            context.RequirementTypes.Add(requirementType);
            context.SaveChangesAsync().Wait();

            var requirementDefinition = new RequirementDefinition(TestPlant, defTitle, 2, RequirementUsage.ForAll, 1);
            requirementType.AddRequirementDefinition(requirementDefinition);
            context.SaveChangesAsync().Wait();

            return requirementType;
        }

        protected Person AddPerson(PreservationContext context, Guid oid, string firstName, string lastName)
        {
            var person = new Person(oid, firstName, lastName);
            context.Persons.Add(person);
            context.SaveChangesAsync().Wait();
            return person;
        }

        protected Project AddProject(PreservationContext context, string name, string description, bool isClosed = false)
        {
            var project = new Project(TestPlant, name, description);
            if (isClosed)
            {
                project.Close();
            }
            context.Projects.Add(project);
            context.SaveChangesAsync().Wait();
            return project;
        }

        protected Tag AddTag(PreservationContext context, Project parentProject, TagType tagType, string tagNo, string description, Step step, IEnumerable<TagRequirement> requirements)
        {
            var tag = new Tag(TestPlant, tagType, tagNo, description, step, requirements);
            parentProject.AddTag(tag);
            context.SaveChangesAsync().Wait();
            return tag;
        }

        protected Field AddInfoField(PreservationContext context, RequirementDefinition rd, string label)
        {
            var field = new Field(TestPlant, label, FieldType.Info, 0);
            rd.AddField(field);
            context.SaveChangesAsync().Wait();
            return field;
        }

        protected Field AddNumberField(PreservationContext context, RequirementDefinition rd, string label, string unit, bool showPrevious)
        {
            var field = new Field(TestPlant, label, FieldType.Number, 0, unit, showPrevious);
            rd.AddField(field);
            context.SaveChangesAsync().Wait();
            return field;
        }

        protected Field AddCheckBoxField(PreservationContext context, RequirementDefinition rd, string label)
        {
            var field = new Field(TestPlant, label, FieldType.CheckBox, 0);
            rd.AddField(field);
            context.SaveChangesAsync().Wait();
            return field;
        }

        protected Field AddAttachmentField(PreservationContext context, RequirementDefinition rd, string label)
        {
            var field = new Field(TestPlant, label, FieldType.Attachment, 0);
            rd.AddField(field);
            context.SaveChangesAsync().Wait();
            return field;
        }

        protected TestDataSet AddTestDataSet(PreservationContext context)
        {
            // Test data set:
            //  - 2 journeys:
            //      - first with 2 steps
            //      - second wth 1 step
            //  - 2 requirement types with one definition in each
            //  - First step in both journeys has mode 1/responsible 1
            //  - Second step in first journey has mode 2/responsible 2
            //  - 20 tags in project 1 (P1).
            //      - 10 first tags is standard, all in first step in journey 1, all has requirement of type 1
            //      - 10 last tags is area, all in first step in journey 2, all has requirement of type 2
            //  - 10 tags on project 2 (P2), all tags same as 10 first in P1
            //  - requirement period for all 30 tags is 2 weeks
            //  - all steps in all journeys has responsible 1
            //  - All Tags has 1 history record
            var _projectName1 = "P1";
            var _projectName2 = "P2";
            var _journeyTitle1 = "J1";
            var _journeyTitle2 = "J2";
            var _mode1 = "M1";
            var _mode2 = "M2";
            var _resp1 = "R1";
            var _resp2 = "R2";
            var _reqType1Code = "ROT";
            var _reqType2Code = "AREA";
            var _reqIconOther = RequirementTypeIcon.Other;

            var testDataSet = new TestDataSet
            {
                CurrentUser = context.Persons.Single(p => p.Oid == _currentUserOid),
                Project1 = AddProject(context, _projectName1, "Project 1 description"),
                Project2 = AddProject(context, _projectName2, "Project 2 description"),
                Mode1 = AddMode(context, _mode1, true),
                Responsible1 = AddResponsible(context, _resp1),
                Mode2 = AddMode(context, _mode2, false),
                Responsible2 = AddResponsible(context, _resp2),
                ReqType1 = AddRequirementTypeWith1DefWithoutField(context, _reqType1Code, "D1", _reqIconOther),
                ReqType2 = AddRequirementTypeWith1DefWithoutField(context, _reqType2Code, "D2", _reqIconOther)
            };

            testDataSet.Journey1With2Steps =
                AddJourneyWithStep(context, _journeyTitle1, "Step1", testDataSet.Mode1, testDataSet.Responsible1);
            testDataSet.Journey2With1Step =
                AddJourneyWithStep(context, _journeyTitle2, "Step1", testDataSet.Mode1, testDataSet.Responsible1);

            var step2OnJourney1 = new Step(TestPlant, "Step2", testDataSet.Mode2, testDataSet.Responsible2);
            testDataSet.Journey1With2Steps.AddStep(step2OnJourney1);
            context.SaveChangesAsync().Wait();

            for (var i = 0; i < 10; i++)
            {
                var tag = new Tag(TestPlant,
                    TagType.Standard,
                    $"{testDataSet.StdTagPrefix}-{i}",
                    "Description",
                    testDataSet.Journey1With2Steps.Steps.ElementAt(0),
                    new List<TagRequirement>
                    {
                        new TagRequirement(
                            TestPlant,
                            testDataSet.IntervalWeeks,
                            testDataSet.ReqType1.RequirementDefinitions.ElementAt(0))
                    })
                {
                    Calloff = $"{testDataSet.CallOffPrefix}-{i}",
                    McPkgNo = $"{testDataSet.McPkgPrefix}-{i}",
                    CommPkgNo = $"{testDataSet.CommPkgPrefix}-{i}",
                    PurchaseOrderNo = $"{testDataSet.PoPrefix}-{i}",
                    Remark = "Remark",
                    StorageArea = $"{testDataSet.StorageAreaPrefix}-{i}",
                    TagFunctionCode = $"{testDataSet.TagFunctionPrefix}-{i}"
                };
                tag.SetArea($"{testDataSet.AreaPrefix}-{i}", $"{testDataSet.AreaPrefix}-{i}-Description");
                tag.SetDiscipline($"{testDataSet.DisciplinePrefix}-{i}", $"{testDataSet.DisciplinePrefix}-{i}-Description");

                testDataSet.Project1.AddTag(tag);
            }

            for (var i = 0; i < 10; i++)
            {
                var tag = new Tag(TestPlant,
                    TagType.SiteArea,
                    $"{testDataSet.SiteTagPrefix}-{i}",
                    "Description",
                    testDataSet.Journey2With1Step.Steps.ElementAt(0),
                    new List<TagRequirement>
                    {
                        new TagRequirement(
                            TestPlant,
                            testDataSet.IntervalWeeks,
                            testDataSet.ReqType2.RequirementDefinitions.ElementAt(0))
                    })
                {
                    Calloff = $"{testDataSet.CallOffPrefix}-{i}",
                    McPkgNo = $"{testDataSet.McPkgPrefix}-{i}",
                    CommPkgNo = $"{testDataSet.CommPkgPrefix}-{i}",
                    PurchaseOrderNo = $"{testDataSet.PoPrefix}-{i}",
                    Remark = "Remark",
                    StorageArea = $"{testDataSet.StorageAreaPrefix}-{i}",
                    TagFunctionCode = $"{testDataSet.TagFunctionPrefix}-{i}"
                };
                tag.SetArea($"{testDataSet.AreaPrefix}-{i}", $"{testDataSet.AreaPrefix}-{i}-Description");
                tag.SetDiscipline($"{testDataSet.DisciplinePrefix}-{i}", $"{testDataSet.DisciplinePrefix}-{i}-Description");

                testDataSet.Project1.AddTag(tag);
            }

            for (var i = 0; i < 10; i++)
            {
                var tag = new Tag(TestPlant,
                    TagType.Standard,
                    $"Another-{i}",
                    "Description",
                    testDataSet.Journey1With2Steps.Steps.ElementAt(0),
                    new List<TagRequirement>
                    {
                        new TagRequirement(TestPlant, testDataSet.IntervalWeeks, testDataSet.ReqType1.RequirementDefinitions.ElementAt(0))
                    });

                testDataSet.Project2.AddTag(tag);
            }
            
            context.SaveChangesAsync().Wait();
            
            foreach (var tag in context.Tags)
            {
                context.History.Add(new History(TestPlant, $"Description-{Guid.NewGuid()}", tag.ObjectGuid, ObjectType.Tag, EventType.TagCreated));
            }
            
            context.SaveChangesAsync().Wait();

            return testDataSet;
        }

        protected TagFunction AddTagFunction(PreservationContext context, string tagFunctionCode, string registerCode)
        {
            var tf = new TagFunction(TestPlant, tagFunctionCode, "Description", registerCode);
            context.TagFunctions.Add(tf);
            context.SaveChangesAsync().Wait();

            return tf;
        }
    }
}

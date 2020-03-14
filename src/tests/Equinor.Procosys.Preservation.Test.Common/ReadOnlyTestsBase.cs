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
using Equinor.Procosys.Preservation.Domain.Events;
using Equinor.Procosys.Preservation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Test.Common
{
    public abstract class ReadOnlyTestsBase
    {
        protected const string TestPlant = "PlantA";
        protected readonly Guid _currentUserOid = new Guid("12345678-1234-1234-1234-123456789123");
        protected DbContextOptions<PreservationContext> _dbContextOptions;
        protected Mock<IPlantProvider> _plantProviderMock;
        protected IPlantProvider _plantProvider;
        protected ICurrentUserProvider _currentUserProvider;
        protected IEventDispatcher _eventDispatcher;
        protected ManualTimeProvider _timeProvider;

        [TestInitialize]
        public void SetupBase()
        {
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock.SetupGet(x => x.Plant).Returns(TestPlant);
            _plantProvider = _plantProviderMock.Object;

            var currentUserProviderMock = new Mock<ICurrentUserProvider>();
            currentUserProviderMock.Setup(x => x.GetCurrentUser())
                .Returns(_currentUserOid);
            _currentUserProvider = currentUserProviderMock.Object;

            var eventDispatcher = new Mock<IEventDispatcher>();
            _eventDispatcher = eventDispatcher.Object;

            _timeProvider = new ManualTimeProvider(new DateTime(2020, 2, 1, 0, 0, 0, DateTimeKind.Utc));
            TimeService.SetProvider(_timeProvider);

            _dbContextOptions = new DbContextOptionsBuilder<PreservationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            SetupNewDatabase(_dbContextOptions);
        }

        protected abstract void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions);

        protected Responsible AddResponsible(PreservationContext context, string code)
        {
            var responsible = new Responsible(TestPlant, code, "Title");
            context.Responsibles.Add(responsible);
            context.SaveChangesAsync().Wait();
            return responsible;
        }

        protected Mode AddMode(PreservationContext context, string title)
        {
            var mode = new Mode(TestPlant, title);
            context.Modes.Add(mode);
            context.SaveChangesAsync().Wait();
            return mode;
        }

        protected Journey AddJourneyWithStep(PreservationContext context, string title, Mode mode,
            Responsible responsible)
        {
            var journey = new Journey(TestPlant, title);
            journey.AddStep(new Step(TestPlant, mode, responsible));
            context.Journeys.Add(journey);
            context.SaveChangesAsync().Wait();
            return journey;
        }

        protected RequirementType AddRequirementTypeWith1DefWithoutField(PreservationContext context, string type,
            string def, int sortKey = 0)
        {
            var requirementType = new RequirementType(TestPlant, type, $"Title{type}", sortKey);
            context.RequirementTypes.Add(requirementType);
            context.SaveChangesAsync().Wait();

            var requirementDefinition = new RequirementDefinition(TestPlant, def, 2, 1);
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

        protected Project AddProject(PreservationContext context, string name, string description,
            bool isClosed = false)
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

        protected Tag AddTag(PreservationContext context, Project parentProject, TagType tagType, string tagNo,
            string description, Step step, IEnumerable<Requirement> requirements)
        {
            var tag = new Tag(TestPlant, tagType, tagNo, description, "", "", "", "", "", "", "", "", step,
                requirements);
            parentProject.AddTag(tag);
            context.SaveChangesAsync().Wait();
            return tag;
        }

        protected Field AddInfoField(PreservationContext context, RequirementDefinition rd, string label)
        {
            var field = new Field(TestPlant, label, FieldType.Info, 0);
            rd.AddField(field);
            context.SaveChanges();
            return field;
        }

        protected Field AddNumberField(PreservationContext context, RequirementDefinition rd, string label, string unit,
            bool showPrevious)
        {
            var field = new Field(TestPlant, label, FieldType.Number, 0, unit, showPrevious);
            rd.AddField(field);
            context.SaveChanges();
            return field;
        }

        protected Field AddCheckBoxField(PreservationContext context, RequirementDefinition rd, string label)
        {
            var field = new Field(TestPlant, label, FieldType.CheckBox, 0);
            rd.AddField(field);
            context.SaveChanges();
            return field;
        }

        protected TestDataSet ApplyTestDataSet(PreservationContext context)
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
            var _stdTagPrefix = "StdTagNo";
            var _siteTagPrefix = "SiteTagNo";
            var _callOffPrefix = "CO";
            var _disciplinePrefix = "DI";
            var _mcPkgPrefix = "MC";
            var _commPkgPrefix = "COMM";
            var _poPrefix = "PO";
            var _tagFunctionPrefix = "TF";
            
            var testDataSet = new TestDataSet
            {
                Person = AddPerson(context, _currentUserOid, "Ole", "Lukkøye"),
                Project1 = AddProject(context, _projectName1, "Project 1 description"),
                Project2 = AddProject(context, _projectName2, "Project 2 description"),
                Mode1 = AddMode(context, _mode1),
                Responsible1 = AddResponsible(context, _resp1),
                Mode2 = AddMode(context, _mode2),
                Responsible2 = AddResponsible(context, _resp2),
                ReqType1 = AddRequirementTypeWith1DefWithoutField(context, _reqType1Code, "D1"),
                ReqType2 = AddRequirementTypeWith1DefWithoutField(context, _reqType2Code, "D2")
            };

            testDataSet.Journey1With2Steps =
                AddJourneyWithStep(context, _journeyTitle1, testDataSet.Mode1, testDataSet.Responsible1);
            testDataSet.Journey2With1Steps =
                AddJourneyWithStep(context, _journeyTitle2, testDataSet.Mode1, testDataSet.Responsible1);

            var step2OnJourney1 = new Step(TestPlant, testDataSet.Mode2, testDataSet.Responsible2);
            testDataSet.Journey1With2Steps.AddStep(step2OnJourney1);
            context.SaveChanges();

            for (var i = 0; i < 10; i++)
            {
                var tag = new Tag(TestPlant,
                    TagType.Standard,
                    $"{_stdTagPrefix}-{i}",
                    "Description",
                    "AreaCode",
                    $"{_callOffPrefix}-{i}",
                    $"{_disciplinePrefix}-{i}",
                    $"{_mcPkgPrefix}-{i}",
                    $"{_commPkgPrefix}-{i}",
                    $"{_poPrefix}-{i}",
                    "Remark",
                    $"{_tagFunctionPrefix}-{i}",
                    testDataSet.Journey1With2Steps.Steps.ElementAt(0),
                    new List<Requirement>
                    {
                        new Requirement(TestPlant, testDataSet.IntervalWeeks, testDataSet.ReqType1.RequirementDefinitions.ElementAt(0))
                    });

                testDataSet.Project1.AddTag(tag);
            }

            for (var i = 0; i < 10; i++)
            {
                var tag = new Tag(TestPlant,
                    TagType.SiteArea,
                    $"{_siteTagPrefix}-{i}",
                    "Description",
                    "AreaCode",
                    $"{_callOffPrefix}-{i}",
                    $"{_disciplinePrefix}-{i}",
                    $"{_mcPkgPrefix}-{i}",
                    $"{_commPkgPrefix}-{i}",
                    $"{_poPrefix}-{i}",
                    "Remark",
                    $"{_tagFunctionPrefix}-{i}",
                    testDataSet.Journey2With1Steps.Steps.ElementAt(0),
                    new List<Requirement>
                    {
                        new Requirement(TestPlant, testDataSet.IntervalWeeks, testDataSet.ReqType2.RequirementDefinitions.ElementAt(0))
                    });

                testDataSet.Project1.AddTag(tag);
            }

            for (var i = 0; i < 10; i++)
            {
                var tag = new Tag(TestPlant,
                    TagType.Standard,
                    $"Another-{i}",
                    "Description",
                    "AreaCode",
                    "Calloff",
                    "DisciplineCode",
                    "McPkgNo",
                    "CommPkgNo",
                    "PurchaseOrderNo",
                    "Remark",
                    "TagFunctionCode",
                    testDataSet.Journey1With2Steps.Steps.ElementAt(0),
                    new List<Requirement>
                    {
                        new Requirement(TestPlant, testDataSet.IntervalWeeks, testDataSet.ReqType1.RequirementDefinitions.ElementAt(0))
                    });
                
                testDataSet.Project2.AddTag(tag);
            }
            
            context.SaveChanges();

            return testDataSet;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.Domain.Time;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.MainApi.Plant;
using Equinor.ProCoSys.Preservation.Query.GetActionsCrossPlant;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetActionsCrossPlant
{
    [TestClass]
    public class GetActionsCrossPlantQueryHandlerTests
    {
        // NB The PlantProvider affects when debugging and locking into DBSets in PreservationContext
        protected DbContextOptions<PreservationContext> _dbContextOptions;
        protected ICurrentUserProvider _currentUserProvider;
        protected IEventDispatcher _eventDispatcher;
        protected ManualTimeProvider _timeProvider;
        protected readonly Guid _currentUserOid = new Guid("12345678-1234-1234-1234-123456789123");

        private readonly PlantProvider _plantProvider = new PlantProvider(null);
        private readonly Mock<IPlantCache> _plantCacheMock = new Mock<IPlantCache>();

        private PCSPlant _plantA = new PCSPlant {Id = "PCS$A", Title = "A"};
        private PCSPlant _plantB = new PCSPlant {Id = "PCS$B", Title = "B"};
        private Project _projectA;
        private Project _projectB;
        private Action _actionA;
        private Action _actionB;

        private Person _currentUser;

        [TestInitialize]
        public void Setup()
        {
            _plantCacheMock.Setup(p => p.GetPlantTitleAsync(_plantA.Id)).Returns(Task.FromResult(_plantA.Title));
            _plantCacheMock.Setup(p => p.GetPlantTitleAsync(_plantB.Id)).Returns(Task.FromResult(_plantB.Title));
            
            var currentUserProviderMock = new Mock<ICurrentUserProvider>();
            currentUserProviderMock.Setup(x => x.GetCurrentUserOid()).Returns(_currentUserOid);
            _currentUserProvider = currentUserProviderMock.Object;

            var eventDispatcher = new Mock<IEventDispatcher>();
            _eventDispatcher = eventDispatcher.Object;

            _timeProvider = new ManualTimeProvider(new DateTime(2020, 2, 1, 0, 0, 0, DateTimeKind.Utc));
            TimeService.SetProvider(_timeProvider);

            _dbContextOptions = new DbContextOptionsBuilder<PreservationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                // ensure current user exists in db
                _currentUser = new Person(_currentUserOid, "Ole", "Lukkøye");
                context.Persons.Add(_currentUser);
                context.SaveChangesAsync().Wait();

                _plantProvider.SetPlant(_plantA.Id);
                (_projectA, _actionA) = CreateAction(context, "PrA", false);
                _plantProvider.SetPlant(_plantB.Id);
                (_projectB, _actionB) = CreateAction(context, "PrB", true);
                _plantProvider.SetCrossPlantQuery();
            }
        }

        private (Project, Action) CreateAction(PreservationContext context, string projectName, bool closeProject)
        {
            var plantId = _plantProvider.Plant;
            var mode = new Mode(plantId, "M1", false);
            context.Modes.Add(mode);
                
            var responsible = new Responsible(plantId, "Resp1", "Resp1-Desc");
            context.Responsibles.Add(responsible);
            context.SaveChangesAsync().Wait();

            var journey = new Journey(plantId, "J1");
            var step = new Step(plantId, "S1", mode, responsible);
            journey.AddStep(step);
            context.Journeys.Add(journey);
            context.SaveChangesAsync().Wait();

            var requirementType = new RequirementType(plantId, "RT", "RT title", RequirementTypeIcon.Other, 1);
            context.RequirementTypes.Add(requirementType);

            var requirementDefinition = new RequirementDefinition(plantId, "RD", 2, RequirementUsage.ForAll, 1);
            requirementType.AddRequirementDefinition(requirementDefinition);
            context.SaveChangesAsync().Wait();

            var project = new Project(plantId, projectName, $"{projectName} Desc") {IsClosed = closeProject};
            context.Projects.Add(project);

            var tag = new Tag(
                plantId, 
                TagType.Standard, 
                "Tag A", 
                "Tag desc", 
                step,
                new List<TagRequirement> {new TagRequirement(plantId, 2, requirementDefinition)});
            project.AddTag(tag);
            context.SaveChangesAsync().Wait();

            var action = new Action(plantId, "A", "D", null);
            tag.AddAction(action);

            var attachment = new ActionAttachment(plantId, Guid.Empty, "fil.txt");
            action.AddAttachment(attachment);

            context.SaveChangesAsync().Wait();
            return (project, action);
        }

        [TestMethod]
        public async Task Handler_ShouldReturnActions_FromDifferentPlants()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetActionsCrossPlantQuery();
                var dut = new GetActionsCrossPlantQueryHandler(context, _plantCacheMock.Object, _plantProvider);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                
                var actionDtos = result.Data;
                Assert.AreEqual(2, actionDtos.Count);

                AssertAction(actionDtos.Single(a => a.Id == _actionA.Id), _actionA, _plantA, _projectA);
                AssertAction(actionDtos.Single(a => a.Id == _actionB.Id), _actionB, _plantB, _projectB);
            }
        }

        private void AssertAction(ActionDto actionDto, Action action, PCSPlant plant, Project project)
        {
            AssertEqualAndNotNull(actionDto.PlantId, plant.Id);
            AssertEqualAndNotNull(actionDto.PlantTitle, plant.Title);
            AssertEqualAndNotNull(actionDto.ProjectName, project.Name);
            Assert.AreEqual(actionDto.IsProjectClosed, project.IsClosed);
            AssertEqualAndNotNull(actionDto.ProjectDescription, project.Description);
            AssertEqualAndNotNull(actionDto.Id, action.Id);
            Assert.AreEqual(actionDto.IsOverDue, action.IsOverDue());
            AssertEqualAndNotNull(actionDto.Title, action.Title);
            AssertEqualAndNotNull(actionDto.Description, action.Description);
            Assert.AreEqual(actionDto.IsClosed, action.IsClosed);
            Assert.AreEqual(actionDto.DueTimeUtc, action.DueTimeUtc);
            AssertEqualAndNotNull(actionDto.AttachmentCount, action.Attachments.Count);
        }

        private void AssertEqualAndNotNull(string expected, string actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        private void AssertEqualAndNotNull(int expected, int actual)
        {
            Assert.IsTrue(actual > 0);
            Assert.AreEqual(expected, actual);
        }
    }
}

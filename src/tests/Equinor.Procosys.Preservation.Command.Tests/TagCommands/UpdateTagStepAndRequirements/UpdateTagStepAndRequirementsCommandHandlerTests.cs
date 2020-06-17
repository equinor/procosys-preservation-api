using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateAreaTag;
using Equinor.Procosys.Preservation.Command.TagCommands.UpdateTagStepAndRequirements;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.MainApi.Area;
using Equinor.Procosys.Preservation.MainApi.Discipline;
using Equinor.Procosys.Preservation.MainApi.Project;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.UpdateTagStepAndRequirements
{
    [TestClass]
    public class UpdateTagStepAndRequirementsCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string TestProjectName = "TestProjectX";
        private const string PlantName = "TestPlant";
        private const int TagId = 123;
        private const int StepId = 11;
        private const int ReqDefId1 = 99;
        private const int ReqDefId2 = 199;
        private const int Interval1 = 2;
        private const int Interval2 = 3;
        private const string DisciplineDescription = "DisciplineDescription";
        private const string AreaDescription = "AreaDescription";

        private Mock<Step> _stepMock;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IRequirementTypeRepository> _rtRepositoryMock;
        private Mock<IProjectApiService> _projectApiServiceMock;
        private Mock<IDisciplineApiService> _disciplineApiServiceMock;
        private Mock<IAreaApiService> _areaApiServiceMock;
        private Mock<Tag> _tagMock;

        private UpdateTagStepAndRequirementsCommand _command;
        private UpdateTagStepAndRequirementsCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _stepMock = new Mock<Step>();
            _stepMock.SetupGet(s => s.Id).Returns(StepId);
            _stepMock.SetupGet(s => s.Plant).Returns(TestPlant);

            _tagMock = new Mock<Tag>();
            _tagMock.Setup(t => t.Plant).Returns(PlantName);

            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journeyRepositoryMock
                .Setup(x => x.GetStepByStepIdAsync(StepId))
                .Returns(Task.FromResult(_stepMock.Object));

            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock.Setup(p => p.GetTagByTagIdAsync(TagId)).Returns(Task.FromResult(_tagMock.Object));



            _rtRepositoryMock = new Mock<IRequirementTypeRepository>();
            var rdMock1 = new Mock<RequirementDefinition>();
            rdMock1.SetupGet(x => x.Id).Returns(ReqDefId1);
            rdMock1.SetupGet(x => x.Plant).Returns(TestPlant);
            var rdMock2 = new Mock<RequirementDefinition>();
            rdMock2.SetupGet(x => x.Id).Returns(ReqDefId2);
            rdMock2.SetupGet(x => x.Plant).Returns(TestPlant);
            _rtRepositoryMock
                .Setup(r => r.GetRequirementDefinitionByIdAsync(ReqDefId2))
                .Returns(Task.FromResult(rdMock2.Object));

            _command = new UpdateTagStepAndRequirementsCommand(
                TagId,
                StepId,
                new List<UpdateRequirementForCommand>(),
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(ReqDefId2, Interval2)
                }, 
                null);

            _dut = new UpdateTagStepAndRequirementsCommandHandler(
                _projectRepositoryMock.Object,
                _journeyRepositoryMock.Object,
                _rtRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object);
        }

        [TestMethod]
        public async Task HandlingUpdateTagStepAndRequirementsCommand_WhenAllIsOk()
        {
            // 
            Assert.AreEqual(0, _tagMock.Object.Requirements.Count);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_stepMock.Object.Id, _tagMock.Object.StepId);
            Assert.AreEqual(1, _tagMock.Object.Requirements.Count);
        }

        [TestMethod]
        public async Task HandlingUpdateTagStepAndRequirementsCommand_RequirementIsUpdatedWithVoided()
        {
            // Arrange
            var updateOnExistingcommand = new UpdateTagStepAndRequirementsCommand(
                TagId,
                StepId,
                new List<UpdateRequirementForCommand>()
                {
                    new UpdateRequirementForCommand(ReqDefId2, Interval1, true)
                },
                new List<RequirementForCommand>(),
                null);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_stepMock.Object.Id, _tagMock.Object.StepId);
            Assert.AreEqual(1, _tagMock.Object.Requirements.Count);
            Assert.AreEqual(false, _tagMock.Object.Requirements.First().IsVoided);
            Assert.AreEqual(Interval2, _tagMock.Object.Requirements.First().IntervalWeeks);
            

            // Act
            var result2 = await _dut.Handle(updateOnExistingcommand, default);

            // Assert
            Assert.AreEqual(0, result2.Errors.Count);
            Assert.AreEqual(_stepMock.Object.Id, _tagMock.Object.StepId);
            Assert.AreEqual(1, _tagMock.Object.Requirements.Count);
            Assert.AreEqual(true, _tagMock.Object.Requirements.First().IsVoided);
            Assert.AreEqual(Interval1, _tagMock.Object.Requirements.First().IntervalWeeks);
        }

        [TestMethod]
        public async Task HandlingUpdateTagStepAndRequirementsCommand_TagNextDueTimeUtcUpdated()
        {
            // Arrange
            var updateOnExistingcommand = new UpdateTagStepAndRequirementsCommand(
                TagId,
                StepId,
                new List<UpdateRequirementForCommand>()
                {
                    new UpdateRequirementForCommand(ReqDefId2, Interval1, false)
                },
                new List<RequirementForCommand>(),
                null);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_stepMock.Object.Id, _tagMock.Object.StepId);
            Assert.AreEqual(1, _tagMock.Object.Requirements.Count);
            Assert.AreEqual(false, _tagMock.Object.Requirements.First().IsVoided);
            Assert.AreEqual(Interval2, _tagMock.Object.Requirements.First().IntervalWeeks);


            // Act
            var result2 = await _dut.Handle(updateOnExistingcommand, default);

            // Assert
            Assert.AreEqual(0, result2.Errors.Count);
            Assert.AreEqual(_stepMock.Object.Id, _tagMock.Object.StepId);
            Assert.AreEqual(1, _tagMock.Object.Requirements.Count);
            Assert.AreEqual(false, _tagMock.Object.Requirements.First().IsVoided);
            Assert.AreEqual(Interval1, _tagMock.Object.Requirements.First().IntervalWeeks);
            Assert.AreEqual(TimeService.UtcNow.AddWeeks(Interval1), _tagMock.Object.NextDueTimeUtc);
        }
    }
}

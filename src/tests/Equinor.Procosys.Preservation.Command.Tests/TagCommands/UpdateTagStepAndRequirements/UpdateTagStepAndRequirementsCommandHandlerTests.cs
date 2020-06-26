using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.UpdateTagStepAndRequirements;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.UpdateTagStepAndRequirements
{
    [TestClass]
    public class UpdateTagStepAndRequirementsCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int TagId = 123;
        private const int StepId1 = 11;
        private const int StepId2 = 12;
        private const int ReqDefId1 = 99;
        private const int ReqDefId2 = 199;
        private const int TagReqId = 10;
        private const int Interval1 = 2;
        private const int Interval2 = 3;
        private const string RowVersion = "AAAAAAAAD6U=";

        private Mock<Step> _stepMock1;
        private Mock<Step> _stepMock2;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IRequirementTypeRepository> _rtRepositoryMock;
        private Tag _tag;

        private UpdateTagStepAndRequirementsCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _stepMock1 = new Mock<Step>();
            _stepMock1.SetupGet(s => s.Id).Returns(StepId1);
            _stepMock1.SetupGet(s => s.Plant).Returns(TestPlant);
            _stepMock2 = new Mock<Step>();
            _stepMock2.SetupGet(s => s.Id).Returns(StepId2);
            _stepMock2.SetupGet(s => s.Plant).Returns(TestPlant);

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

            var tagRequirement = new TagRequirement(TestPlant, Interval1, rdMock1.Object);
            tagRequirement.SetProtectedIdForTesting(TagReqId);
            _tag = new Tag(TestPlant, TagType.Standard, "X", "D", _stepMock1.Object, new List<TagRequirement>
            {
                tagRequirement
            });

            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journeyRepositoryMock
                .Setup(x => x.GetStepByStepIdAsync(StepId1))
                .Returns(Task.FromResult(_stepMock1.Object));
            _journeyRepositoryMock
                .Setup(x => x.GetStepByStepIdAsync(StepId2))
                .Returns(Task.FromResult(_stepMock2.Object));

            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock.Setup(p => p.GetTagByTagIdAsync(TagId)).Returns(Task.FromResult(_tag));

            _dut = new UpdateTagStepAndRequirementsCommandHandler(
                _projectRepositoryMock.Object,
                _journeyRepositoryMock.Object,
                _rtRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object);

            Assert.AreEqual(1, _tag.Requirements.Count);
        }

        [TestMethod]
        public async Task HandlingUpdateTagStepAndRequirementsCommand_ShouldAddNewRequirement_WhenAddingAddNewRequirement()
        {
            // Arrange
            var updatedRequirements = new List<UpdateRequirementForCommand>();
            var newRequirements = new List<RequirementForCommand>
            {
                new RequirementForCommand(ReqDefId2, Interval2)
            };
            var command = new UpdateTagStepAndRequirementsCommand(
                TagId,
                StepId1,
                updatedRequirements,
                newRequirements, 
                RowVersion);
            
            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(StepId1, _tag.StepId);
            Assert.AreEqual(2, _tag.Requirements.Count);
        }

        [TestMethod]
        public async Task HandlingUpdateTagStepAndRequirementsCommand_ShouldUpdateExistingRequirement()
        {
            // Arrange
            Assert.AreEqual(Interval1, _tag.Requirements.First().IntervalWeeks);
            Assert.IsFalse(_tag.Requirements.First().IsVoided);
            var updatedRequirements = new List<UpdateRequirementForCommand>
            {
                new UpdateRequirementForCommand(TagReqId, Interval2, true, RowVersion)
            };
            var command = new UpdateTagStepAndRequirementsCommand(
                TagId,
                StepId2,
                updatedRequirements,
                new List<RequirementForCommand>(),
                RowVersion);

            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(StepId2, _tag.StepId);
            Assert.AreEqual(1, _tag.Requirements.Count);
            Assert.IsTrue(_tag.Requirements.First().IsVoided);
            Assert.AreEqual(Interval2, _tag.Requirements.First().IntervalWeeks);
        }

        [TestMethod]
        public async Task HandlingUpdateTagStepAndRequirementsCommand_ShouldNotStartPreservationOnNewRequirements_WhenPreservationNotStarted()
        {
            // Arrange
            var updatedRequirements = new List<UpdateRequirementForCommand>();
            var newRequirements = new List<RequirementForCommand>
            {
                new RequirementForCommand(ReqDefId2, Interval2)
            };
            var command = new UpdateTagStepAndRequirementsCommand(
                TagId,
                StepId1,
                updatedRequirements,
                newRequirements, 
                RowVersion);
            
            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            var newReq = _tag.Requirements.Single(r => r.RequirementDefinitionId == ReqDefId2);
            Assert.IsNull(newReq.ActivePeriod);
            Assert.IsNull(newReq.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingUpdateTagStepAndRequirementsCommand_ShouldStartPreservationOnNewRequirements_WhenPreservationActive()
        {
            // Arrange
            _tag.StartPreservation();
            var updatedRequirements = new List<UpdateRequirementForCommand>();
            var newRequirements = new List<RequirementForCommand>
            {
                new RequirementForCommand(ReqDefId2, Interval2)
            };
            var command = new UpdateTagStepAndRequirementsCommand(
                TagId,
                StepId1,
                updatedRequirements,
                newRequirements, 
                RowVersion);
            
            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            var newReq = _tag.Requirements.Single(r => r.RequirementDefinitionId == ReqDefId2);
            Assert.IsNotNull(newReq.ActivePeriod);
            Assert.IsNotNull(newReq.NextDueTimeUtc);
        }
    }
}

using System;
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
        private const int TagId1 = 123;
        private const int TagId2 = 124;
        private const int StepId1 = 11;
        private const int StepId2 = 12;
        private const int ReqDefId1 = 99;
        private const int ReqDefId2 = 199;
        private const int TwoWeekInterval = 2;
        private const int ThreeWeekInterval = 3;
        private const string RowVersion = "AAAAAAAAD6U=";

        private Mock<Step> _stepMock1;
        private Mock<Step> _stepMock2;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Mock<IProjectRepository> _projectRepositoryMock;
        private Mock<IRequirementTypeRepository> _rtRepositoryMock;
        private Tag _tagWithOneRequirement;
        private Tag _tagWithTwoRequirements;

        private UpdateTagStepAndRequirementsCommandHandler _dut;
        private TagRequirement _tagRequirement1OnTag1;
        private TagRequirement _tagRequirement1OnTag2;
        private TagRequirement _tagRequirement2OnTag2;

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

            _tagRequirement1OnTag1 = new TagRequirement(TestPlant, ThreeWeekInterval, rdMock1.Object);
            _tagRequirement1OnTag1.SetProtectedIdForTesting(111);
            _tagWithOneRequirement = new Tag(TestPlant, TagType.Standard, "T1", "D", _stepMock1.Object, new List<TagRequirement>
            {
                _tagRequirement1OnTag1
            });
            _tagRequirement1OnTag2 = new TagRequirement(TestPlant, TwoWeekInterval, rdMock1.Object);
            _tagRequirement1OnTag2.SetProtectedIdForTesting(112);
            _tagRequirement2OnTag2 = new TagRequirement(TestPlant, ThreeWeekInterval, rdMock2.Object);
            _tagRequirement2OnTag2.SetProtectedIdForTesting(113);
            _tagWithTwoRequirements = new Tag(TestPlant, TagType.Standard, "T2", "D", _stepMock1.Object, new List<TagRequirement>
            {
                _tagRequirement1OnTag2,
                _tagRequirement2OnTag2
            });

            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journeyRepositoryMock
                .Setup(x => x.GetStepByStepIdAsync(StepId1))
                .Returns(Task.FromResult(_stepMock1.Object));
            _journeyRepositoryMock
                .Setup(x => x.GetStepByStepIdAsync(StepId2))
                .Returns(Task.FromResult(_stepMock2.Object));

            _projectRepositoryMock = new Mock<IProjectRepository>();
            _projectRepositoryMock.Setup(p => p.GetTagByTagIdAsync(TagId1)).Returns(Task.FromResult(_tagWithOneRequirement));
            _projectRepositoryMock.Setup(p => p.GetTagByTagIdAsync(TagId2)).Returns(Task.FromResult(_tagWithTwoRequirements));

            _dut = new UpdateTagStepAndRequirementsCommandHandler(
                _projectRepositoryMock.Object,
                _journeyRepositoryMock.Object,
                _rtRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object);

            Assert.AreEqual(1, _tagWithOneRequirement.Requirements.Count);
        }

        [TestMethod]
        public async Task HandlingUpdateTagStepAndRequirementsCommand_ShouldAddNewRequirement_WhenAddingAddNewRequirement()
        {
            // Arrange
            var updatedRequirements = new List<UpdateRequirementForCommand>();
            var newRequirements = new List<RequirementForCommand>
            {
                new RequirementForCommand(ReqDefId2, ThreeWeekInterval)
            };
            var command = new UpdateTagStepAndRequirementsCommand(
                TagId1,
                StepId1,
                updatedRequirements,
                newRequirements, 
                RowVersion);
            
            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(StepId1, _tagWithOneRequirement.StepId);
            Assert.AreEqual(2, _tagWithOneRequirement.Requirements.Count);
        }

        [TestMethod]
        public async Task HandlingUpdateTagStepAndRequirementsCommand_ShouldUpdateExistingRequirement()
        {
            // Arrange
            var tagRequirement = _tagWithOneRequirement.Requirements.First();
            Assert.AreEqual(ThreeWeekInterval, tagRequirement.IntervalWeeks);
            var updatedRequirements = new List<UpdateRequirementForCommand>
            {
                new UpdateRequirementForCommand(tagRequirement.Id, TwoWeekInterval, false, RowVersion)
            };
            var command = new UpdateTagStepAndRequirementsCommand(
                TagId1,
                StepId2,
                updatedRequirements,
                new List<RequirementForCommand>(),
                RowVersion);

            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(StepId2, _tagWithOneRequirement.StepId);
            Assert.AreEqual(1, _tagWithOneRequirement.Requirements.Count);
            Assert.AreEqual(TwoWeekInterval, tagRequirement.IntervalWeeks);
        }

        [TestMethod]
        public async Task HandlingUpdateTagStepAndRequirementsCommand_ShouldNotStartPreservationOnNewRequirements_WhenPreservationNotStarted()
        {
            // Arrange
            var updatedRequirements = new List<UpdateRequirementForCommand>();
            var newRequirements = new List<RequirementForCommand>
            {
                new RequirementForCommand(ReqDefId2, ThreeWeekInterval)
            };
            var command = new UpdateTagStepAndRequirementsCommand(
                TagId1,
                StepId1,
                updatedRequirements,
                newRequirements, 
                RowVersion);
            
            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            var newReq = _tagWithOneRequirement.Requirements.Single(r => r.RequirementDefinitionId == ReqDefId2);
            Assert.IsNull(newReq.ActivePeriod);
            Assert.IsNull(newReq.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingUpdateTagStepAndRequirementsCommand_ShouldStartPreservationOnNewRequirements_WhenPreservationActive()
        {
            // Arrange
            Assert.IsNull(_tagWithOneRequirement.NextDueTimeUtc);
            _tagWithOneRequirement.StartPreservation();
            Assert.IsNotNull(_tagWithOneRequirement.NextDueTimeUtc);

            var updatedRequirements = new List<UpdateRequirementForCommand>();
            var newRequirements = new List<RequirementForCommand>
            {
                new RequirementForCommand(ReqDefId2, ThreeWeekInterval)
            };
            var command = new UpdateTagStepAndRequirementsCommand(
                TagId1,
                StepId1,
                updatedRequirements,
                newRequirements, 
                RowVersion);
            
            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            var newReq = _tagWithOneRequirement.Requirements.Single(r => r.RequirementDefinitionId == ReqDefId2);
            Assert.IsNotNull(newReq.ActivePeriod);
            Assert.IsNotNull(newReq.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingUpdateTagStepAndRequirementsCommand_ShouldBeAbleToVoidRequirement_WhenNonVoidingRequirementExistsAfter()
        {
            // Arrange
            var tagRequirement1 = _tagWithTwoRequirements.Requirements.Single(r => r.RequirementDefinitionId == ReqDefId1);
            var tagRequirement2 = _tagWithTwoRequirements.Requirements.Single(r => r.RequirementDefinitionId == ReqDefId2);

            Assert.IsFalse(tagRequirement1.IsVoided);
            Assert.IsFalse(tagRequirement2.IsVoided);
            
            var updatedRequirements = new List<UpdateRequirementForCommand>
            {
                new UpdateRequirementForCommand(tagRequirement1.Id, ThreeWeekInterval, true, RowVersion)
            };
            var command = new UpdateTagStepAndRequirementsCommand(
                TagId2,
                StepId2,
                updatedRequirements,
                new List<RequirementForCommand>(),
                RowVersion);

            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsTrue(tagRequirement1.IsVoided);
            Assert.IsFalse(tagRequirement2.IsVoided);
        }

        [TestMethod]
        public async Task HandlingUpdateTagStepAndRequirementsCommand_ShouldUpdateNextDueOnActiveTag_WhenUpdatingExistingWithShorterInterval()
        {
            // Arrange
            _tagWithOneRequirement.StartPreservation();

            var tagRequirement = _tagWithOneRequirement.Requirements.First();
            Assert.AreEqual(ThreeWeekInterval, tagRequirement.IntervalWeeks);
            var oldNextDueTimeOnTag = _tagWithOneRequirement.NextDueTimeUtc;
            Assert.IsNotNull(oldNextDueTimeOnTag);
            var updatedRequirements = new List<UpdateRequirementForCommand>
            {
                new UpdateRequirementForCommand(tagRequirement.Id, TwoWeekInterval, false, RowVersion)
            };
            var command = new UpdateTagStepAndRequirementsCommand(
                TagId1,
                StepId2,
                updatedRequirements,
                new List<RequirementForCommand>(),
                RowVersion);

            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(1, _tagWithOneRequirement.Requirements.Count);
            var newNextDueTimeOnTag = _tagWithOneRequirement.NextDueTimeUtc;
            Assert.IsNotNull(newNextDueTimeOnTag);
            Assert.IsTrue(newNextDueTimeOnTag.Value < oldNextDueTimeOnTag.Value);
            var nextDueTimeUtcOnRequirement = tagRequirement.NextDueTimeUtc;
            Assert.IsNotNull(nextDueTimeUtcOnRequirement);
            Assert.AreEqual(newNextDueTimeOnTag.Value, nextDueTimeUtcOnRequirement.Value);
        }

        [TestMethod]
        public async Task HandlingUpdateTagStepAndRequirementsCommand_ShouldUpdateNextDueOnActiveTag_WhenAddingNewRequirementWithShorterIntervalThanExisting()
        {
            // Arrange
            _tagWithOneRequirement.StartPreservation();

            Assert.AreEqual(ThreeWeekInterval, _tagWithOneRequirement.Requirements.First().IntervalWeeks);
            var oldNextDueTimeOnTag = _tagWithOneRequirement.NextDueTimeUtc;
            Assert.IsNotNull(oldNextDueTimeOnTag);

            var updatedRequirements = new List<UpdateRequirementForCommand>();
            var newRequirements = new List<RequirementForCommand>
            {
                new RequirementForCommand(ReqDefId2, TwoWeekInterval)
            };
            var command = new UpdateTagStepAndRequirementsCommand(
                TagId1,
                StepId1,
                updatedRequirements,
                newRequirements, 
                RowVersion);

            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(2, _tagWithOneRequirement.Requirements.Count);
            var newNextDueTimeOnTag = _tagWithOneRequirement.NextDueTimeUtc;
            Assert.IsNotNull(newNextDueTimeOnTag);
            Assert.IsTrue(newNextDueTimeOnTag.Value < oldNextDueTimeOnTag.Value);
            var nearestRequirement = _tagWithOneRequirement.Requirements.Single(r => r.RequirementDefinitionId == ReqDefId2);
            Assert.AreEqual(TwoWeekInterval, nearestRequirement.IntervalWeeks);
            var nextDueTimeUtcOnRequirement = nearestRequirement.NextDueTimeUtc;
            Assert.IsNotNull(nextDueTimeUtcOnRequirement);
            Assert.AreEqual(newNextDueTimeOnTag.Value, nextDueTimeUtcOnRequirement.Value);
        }
    }
}

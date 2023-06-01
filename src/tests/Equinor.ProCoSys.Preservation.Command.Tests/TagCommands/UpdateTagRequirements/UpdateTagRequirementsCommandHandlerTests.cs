using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTagRequirements;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.UpdateTagRequirements
{
    [TestClass]
    public class UpdateTagRequirementsCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int StandardTagId1 = 123;
        private const int StandardTagId2 = 124;
        private const int AreaTagId = 125;
        private const int StepId = 13;
        private const int ReqDefId1 = 99;
        private const int ReqDefId2 = 199;
        private const int TwoWeekInterval = 2;
        private const int ThreeWeekInterval = 3;
        private const string RowVersion = "AAAAAAAAD6U=";
        private const string Description = "Desc";

        private Tag _areaTagWithOneRequirement;
        private Tag _standardTagWithOneRequirement;
        private Tag _standardTagWithTwoRequirements;

        private UpdateTagRequirementsCommandHandler _dut;
        private TagRequirement _tagRequirement1OnStandardTag1;
        private TagRequirement _tagRequirement1OnAreaTag;
        private TagRequirement _tagRequirement1OnStandardTag2;
        private TagRequirement _tagRequirement2OnStandardTag2;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Id).Returns(StepId);
            stepMock.SetupGet(s => s.Plant).Returns(TestPlant);

            var rtRepositoryMock = new Mock<IRequirementTypeRepository>();
            var rdMock1 = new Mock<RequirementDefinition>();
            rdMock1.SetupGet(x => x.Id).Returns(ReqDefId1);
            rdMock1.SetupGet(x => x.Plant).Returns(TestPlant);
            var rdMock2 = new Mock<RequirementDefinition>();
            rdMock2.SetupGet(x => x.Id).Returns(ReqDefId2);
            rdMock2.SetupGet(x => x.Plant).Returns(TestPlant);
            rtRepositoryMock
                .Setup(r => r.GetRequirementDefinitionByIdAsync(ReqDefId2))
                .Returns(Task.FromResult(rdMock2.Object));

            var reqId = 111;
            _tagRequirement1OnStandardTag1 = new TagRequirement(TestPlant, ThreeWeekInterval, rdMock1.Object);
            _tagRequirement1OnStandardTag1.SetProtectedIdForTesting(++reqId);
            _standardTagWithOneRequirement = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "T1", Description, stepMock.Object, new List<TagRequirement>
            {
                _tagRequirement1OnStandardTag1
            });
            _tagRequirement1OnStandardTag2 = new TagRequirement(TestPlant, TwoWeekInterval, rdMock1.Object);
            _tagRequirement1OnStandardTag2.SetProtectedIdForTesting(++reqId);
            _tagRequirement2OnStandardTag2 = new TagRequirement(TestPlant, ThreeWeekInterval, rdMock2.Object);
            _tagRequirement2OnStandardTag2.SetProtectedIdForTesting(++reqId);
            _standardTagWithTwoRequirements = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "T2", "D", stepMock.Object, new List<TagRequirement>
            {
                _tagRequirement1OnStandardTag2,
                _tagRequirement2OnStandardTag2
            });
            _tagRequirement1OnAreaTag = new TagRequirement(TestPlant, ThreeWeekInterval, rdMock1.Object);
            _tagRequirement1OnAreaTag.SetProtectedIdForTesting(++reqId);
            _areaTagWithOneRequirement = new Tag(TestPlant, TagType.PoArea, Guid.NewGuid(), "T3", Description, stepMock.Object, new List<TagRequirement>
            {
                _tagRequirement1OnAreaTag
            });

            var projectRepositoryMock = new Mock<IProjectRepository>();
            projectRepositoryMock.Setup(p => p.GetTagWithPreservationHistoryByTagIdAsync(StandardTagId1))
                .Returns(Task.FromResult(_standardTagWithOneRequirement));
            projectRepositoryMock.Setup(p => p.GetTagWithPreservationHistoryByTagIdAsync(StandardTagId2))
                .Returns(Task.FromResult(_standardTagWithTwoRequirements));
            projectRepositoryMock.Setup(p => p.GetTagWithPreservationHistoryByTagIdAsync(AreaTagId))
                .Returns(Task.FromResult(_areaTagWithOneRequirement));

            _dut = new UpdateTagRequirementsCommandHandler(
                projectRepositoryMock.Object,
                rtRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object);

            Assert.AreEqual(1, _standardTagWithOneRequirement.Requirements.Count);
        }

        [TestMethod]
        public async Task HandlingUpdateTagRequirementsCommand_ShouldAddNewRequirement_WhenAddingAddNewRequirement()
        {
            // Arrange
            var newRequirements = new List<RequirementForCommand>
            {
                new RequirementForCommand(ReqDefId2, ThreeWeekInterval)
            };
            var command = new UpdateTagRequirementsCommand(
                StandardTagId1,
                null,
                null, 
                newRequirements, 
                null, 
                RowVersion);
            
            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(2, _standardTagWithOneRequirement.Requirements.Count);
        }

        [TestMethod]
        public async Task HandlingUpdateTagRequirementsCommand_ShouldUpdateExistingRequirement()
        {
            // Arrange
            var tagRequirement = _standardTagWithOneRequirement.Requirements.First();
            Assert.AreEqual(ThreeWeekInterval, tagRequirement.IntervalWeeks);
            var updatedRequirements = new List<UpdateRequirementForCommand>
            {
                new UpdateRequirementForCommand(tagRequirement.Id, TwoWeekInterval, false, RowVersion)
            };
            var command = new UpdateTagRequirementsCommand(
                StandardTagId1,
                null,
                updatedRequirements,
                null,
                null, 
                RowVersion);

            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(1, _standardTagWithOneRequirement.Requirements.Count);
            Assert.AreEqual(TwoWeekInterval, tagRequirement.IntervalWeeks);
        }

        [TestMethod]
        public async Task HandlingUpdateTagRequirementsCommand_ShouldNotStartPreservationOnNewRequirements_WhenPreservationNotStarted()
        {
            // Arrange
            var newRequirements = new List<RequirementForCommand>
            {
                new RequirementForCommand(ReqDefId2, ThreeWeekInterval)
            };
            var command = new UpdateTagRequirementsCommand(
                StandardTagId1,
                null,
                null, 
                newRequirements, 
                null, 
                RowVersion);
            
            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            var newReq = _standardTagWithOneRequirement.Requirements.Single(r => r.RequirementDefinitionId == ReqDefId2);
            Assert.IsNull(newReq.ActivePeriod);
            Assert.IsNull(newReq.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingUpdateTagRequirementsCommand_ShouldStartPreservationOnNewRequirements_WhenPreservationActive()
        {
            // Arrange
            Assert.IsNull(_standardTagWithOneRequirement.NextDueTimeUtc);
            _standardTagWithOneRequirement.StartPreservation();
            Assert.IsNotNull(_standardTagWithOneRequirement.NextDueTimeUtc);

            var newRequirements = new List<RequirementForCommand>
            {
                new RequirementForCommand(ReqDefId2, ThreeWeekInterval)
            };
            var command = new UpdateTagRequirementsCommand(
                StandardTagId1,
                null,
                null, 
                newRequirements, 
                null, 
                RowVersion);
            
            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            var newReq = _standardTagWithOneRequirement.Requirements.Single(r => r.RequirementDefinitionId == ReqDefId2);
            Assert.IsNotNull(newReq.ActivePeriod);
            Assert.IsNotNull(newReq.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingUpdateTagRequirementsCommand_ShouldBeAbleToVoidRequirement_WhenNonVoidedRequirementExistsAfter()
        {
            // Arrange
            var tagRequirement1 = _standardTagWithTwoRequirements.Requirements.Single(r => r.RequirementDefinitionId == ReqDefId1);
            var tagRequirement2 = _standardTagWithTwoRequirements.Requirements.Single(r => r.RequirementDefinitionId == ReqDefId2);

            Assert.IsFalse(tagRequirement1.IsVoided);
            Assert.IsFalse(tagRequirement2.IsVoided);
            
            var updatedRequirements = new List<UpdateRequirementForCommand>
            {
                new UpdateRequirementForCommand(tagRequirement1.Id, ThreeWeekInterval, true, RowVersion)
            };
            var command = new UpdateTagRequirementsCommand(
                StandardTagId2,
                null,
                updatedRequirements,
                null,
                null,
                RowVersion);

            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsTrue(tagRequirement1.IsVoided);
            Assert.IsFalse(tagRequirement2.IsVoided);
        }

        [TestMethod]
        public async Task HandlingUpdateTagRequirementsCommand_ShouldUpdateNextDueOnActiveTag_WhenUpdatingExistingWithShorterInterval()
        {
            // Arrange
            _standardTagWithOneRequirement.StartPreservation();

            var tagRequirement = _standardTagWithOneRequirement.Requirements.First();
            Assert.AreEqual(ThreeWeekInterval, tagRequirement.IntervalWeeks);
            var oldNextDueTimeOnTag = _standardTagWithOneRequirement.NextDueTimeUtc;
            Assert.IsNotNull(oldNextDueTimeOnTag);
            var updatedRequirements = new List<UpdateRequirementForCommand>
            {
                new UpdateRequirementForCommand(tagRequirement.Id, TwoWeekInterval, false, RowVersion)
            };
            var command = new UpdateTagRequirementsCommand(
                StandardTagId1,
                null,
                updatedRequirements,
                null,
                null,
                RowVersion);

            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(1, _standardTagWithOneRequirement.Requirements.Count);
            var newNextDueTimeOnTag = _standardTagWithOneRequirement.NextDueTimeUtc;
            Assert.IsNotNull(newNextDueTimeOnTag);
            Assert.IsTrue(newNextDueTimeOnTag.Value < oldNextDueTimeOnTag.Value);
            var nextDueTimeUtcOnRequirement = tagRequirement.NextDueTimeUtc;
            Assert.IsNotNull(nextDueTimeUtcOnRequirement);
            Assert.AreEqual(newNextDueTimeOnTag.Value, nextDueTimeUtcOnRequirement.Value);
        }

        [TestMethod]
        public async Task HandlingUpdateTagRequirementsCommand_ShouldUpdateNextDueOnActiveTag_WhenAddingNewRequirementWithShorterIntervalThanExisting()
        {
            // Arrange
            _standardTagWithOneRequirement.StartPreservation();

            Assert.AreEqual(ThreeWeekInterval, _standardTagWithOneRequirement.Requirements.First().IntervalWeeks);
            var oldNextDueTimeOnTag = _standardTagWithOneRequirement.NextDueTimeUtc;
            Assert.IsNotNull(oldNextDueTimeOnTag);

            var newRequirements = new List<RequirementForCommand>
            {
                new RequirementForCommand(ReqDefId2, TwoWeekInterval)
            };
            var command = new UpdateTagRequirementsCommand(
                StandardTagId1,
                null,
                null,
                newRequirements, 
                null,
                RowVersion);

            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(2, _standardTagWithOneRequirement.Requirements.Count);
            var newNextDueTimeOnTag = _standardTagWithOneRequirement.NextDueTimeUtc;
            Assert.IsNotNull(newNextDueTimeOnTag);
            Assert.IsTrue(newNextDueTimeOnTag.Value < oldNextDueTimeOnTag.Value);
            var nearestRequirement = _standardTagWithOneRequirement.Requirements.Single(r => r.RequirementDefinitionId == ReqDefId2);
            Assert.AreEqual(TwoWeekInterval, nearestRequirement.IntervalWeeks);
            var nextDueTimeUtcOnRequirement = nearestRequirement.NextDueTimeUtc;
            Assert.IsNotNull(nextDueTimeUtcOnRequirement);
            Assert.AreEqual(newNextDueTimeOnTag.Value, nextDueTimeUtcOnRequirement.Value);
        }

        [TestMethod]
        public async Task HandlingUpdateTagRequirementsCommand_ShouldKeepDescription_WhenDescriptionNotGiven()
        {
            // Arrange
            Assert.AreEqual(Description, _areaTagWithOneRequirement.Description);

            var command = new UpdateTagRequirementsCommand(
                AreaTagId,
                null,
                null,
                null,
                null,
                RowVersion);

            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(Description, _areaTagWithOneRequirement.Description);
        }

        [TestMethod]
        public async Task HandlingUpdateTagRequirementsCommand_ShouldChangeDescription_ForAreaTag()
        {
            // Arrange
            Assert.AreEqual(Description, _areaTagWithOneRequirement.Description);

            var newDescription = "NewDescription";
            var command = new UpdateTagRequirementsCommand(
                AreaTagId,
                newDescription,
                null,
                null,
                null,
                RowVersion);

            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(newDescription, _areaTagWithOneRequirement.Description);
        }

        [TestMethod]
        public async Task HandlingUpdateTagRequirementsCommand_ShouldKeepDescription_ForStandardTag()
        {
            // Act
            var command = new UpdateTagRequirementsCommand(
                StandardTagId1,
                "NewDescription",
                null,
                null,
                null,
                RowVersion);

            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(Description, _areaTagWithOneRequirement.Description);
        }

        [TestMethod]
        public async Task HandlingUpdateTagRequirementsCommand_ShouldBeAbleToDeleteRequirement_WhenRequirementIsUpdatedAsVoided()
        {
            // Arrange
            var tagRequirement1 = _standardTagWithTwoRequirements.Requirements.Single(r => r.RequirementDefinitionId == ReqDefId1);

            Assert.AreEqual(2, _standardTagWithTwoRequirements.Requirements.Count);
            
            var updatedRequirements = new List<UpdateRequirementForCommand>
            {
                new UpdateRequirementForCommand(tagRequirement1.Id, ThreeWeekInterval, true, RowVersion)
            };

            var deleteRequirements = new List<DeleteRequirementForCommand>
            {
                new DeleteRequirementForCommand(tagRequirement1.Id, RowVersion)
            };

            var command = new UpdateTagRequirementsCommand(
                StandardTagId2,
                null,
                updatedRequirements,
                null,
                deleteRequirements, 
                RowVersion);

            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(1, _standardTagWithTwoRequirements.Requirements.Count);
        }

        [TestMethod]
        public async Task HandlingUpdateTagRequirementsCommand_ShouldBeAbleToDeleteRequirement_WhenRequirementIsAlreadyVoided()
        {
            // Arrange
            var tagRequirement1 = _standardTagWithTwoRequirements.Requirements.Single(r => r.RequirementDefinitionId == ReqDefId1);
            tagRequirement1.IsVoided = true;

            Assert.AreEqual(2, _standardTagWithTwoRequirements.Requirements.Count);

            var deleteRequirements = new List<DeleteRequirementForCommand>
            {
                new DeleteRequirementForCommand(tagRequirement1.Id, RowVersion)
            };

            var command = new UpdateTagRequirementsCommand(
                StandardTagId2,
                null,
                null,
                null,
                deleteRequirements, 
                RowVersion);

            // Act
            var result = await _dut.Handle(command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(1, _standardTagWithTwoRequirements.Requirements.Count);
        }
    }
}

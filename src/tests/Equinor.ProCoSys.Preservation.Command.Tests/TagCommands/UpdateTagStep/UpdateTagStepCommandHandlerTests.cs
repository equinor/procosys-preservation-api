using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTagStep;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.UpdateTagStep
{
    [TestClass]
    public class UpdateTagStepCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int FromStepId = 1;
        private const int ToStepId = 2;

        private const string RowVersion1 = "AAAAAAAAABA=";
        private const string RowVersion2 = "AAAAAAAABBA=";

        private UpdateTagStepCommand _command;

        private Tag _stdTag;
        private Tag _poAreaTag;

        private UpdateTagStepCommandHandler _dut;
        private Mock<Step> _toStepMock;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var fromStepMock = new Mock<Step>();
            fromStepMock.SetupGet(s => s.Id).Returns(FromStepId);
            fromStepMock.SetupGet(s => s.Plant).Returns(TestPlant);

            _toStepMock = new Mock<Step>();
            _toStepMock.SetupGet(s => s.Id).Returns(ToStepId);
            _toStepMock.SetupGet(s => s.Plant).Returns(TestPlant);
            _toStepMock.SetupGet(s => s.IsSupplierStep).Returns(true);

            var journeyRepositoryMock = new Mock<IJourneyRepository>();
            journeyRepositoryMock
                .Setup(x => x.GetStepByStepIdAsync(ToStepId))
                .Returns(Task.FromResult(_toStepMock.Object));

            var reqMock1 = new Mock<TagRequirement>();
            reqMock1.SetupGet(r => r.Plant).Returns(TestPlant);

            var tagId1 = 7;
            var tagId2 = 8;
            _stdTag = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "", "", fromStepMock.Object,
                new List<TagRequirement> { reqMock1.Object });
            _stdTag.SetProtectedIdForTesting(tagId1);

            var reqMock2 = new Mock<TagRequirement>();
            reqMock2.SetupGet(r => r.Plant).Returns(TestPlant);
            _poAreaTag = new Tag(TestPlant, TagType.PoArea, Guid.NewGuid(), "", "", fromStepMock.Object,
                new List<TagRequirement> { reqMock2.Object });
            _poAreaTag.SetProtectedIdForTesting(tagId2);

            var projectRepositoryMock = new Mock<IProjectRepository>();
            var tagIds = new List<int> { tagId1, tagId2 };
            var tagIdsWithRowVersion = new List<IdAndRowVersion> {
                new IdAndRowVersion(tagId1, RowVersion1),
                new IdAndRowVersion(tagId2, RowVersion2)
            };

            projectRepositoryMock
                .Setup(r => r.GetTagsOnlyByTagIdsAsync(tagIds))
                .Returns(Task.FromResult(new List<Tag> { _stdTag, _poAreaTag }));

            _command = new UpdateTagStepCommand(tagIdsWithRowVersion, ToStepId);

            _dut = new UpdateTagStepCommandHandler(
                projectRepositoryMock.Object,
                journeyRepositoryMock.Object,
                UnitOfWorkMock.Object);

            Assert.AreEqual(1, _poAreaTag.Requirements.Count);
        }

        [TestMethod]
        public async Task HandlingUpdateTagStepCommand_ShouldSetNewStepOnAllTags_WhenUpdateToSupplierStep()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(ToStepId, _stdTag.StepId);
            Assert.AreEqual(ToStepId, _poAreaTag.StepId);
        }

        [TestMethod]
        public async Task HandlingUpdateTagStepCommand_ShouldThrowException_WhenUpdatePoTagToNonSupplierStep()
        {
            _toStepMock.SetupGet(s => s.IsSupplierStep).Returns(false);

            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _dut.Handle(_command, default)
            );
        }

        [TestMethod]
        public async Task HandlingTransferCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingTransferCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            Assert.AreEqual(RowVersion1, result.Data.First().RowVersion);
            Assert.AreEqual(RowVersion1, _stdTag.RowVersion.ConvertToString());
        }
    }
}

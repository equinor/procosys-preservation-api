using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.TagCommands.UnvoidTag;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.UnvoidTag
{
    [TestClass]
    public class UnvoidTagCommandHandlerTests : CommandHandlerTestsBase
    {
        private Tag _tag;
        private UnvoidTagCommand _command;
        private UnvoidTagCommandHandler _dut;
        private readonly string _rowVersion = "AAAAAAAAABA=";

        [TestInitialize]
        public void Setup()
        {
            var tagId = 2;

            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Plant).Returns(TestPlant);

            var rdMock = new Mock<RequirementDefinition>();
            rdMock.SetupGet(rd => rd.Plant).Returns(TestPlant);

            var requirement = new TagRequirement(TestPlant, 2, rdMock.Object);
            _tag = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "", "", stepMock.Object,
                new List<TagRequirement> { requirement });

            var projectRepositoryMock = new Mock<IProjectRepository>();
            projectRepositoryMock
                .Setup(r => r.GetTagOnlyByTagIdAsync(tagId))
                .Returns(Task.FromResult(_tag));

            _command = new UnvoidTagCommand(tagId, _rowVersion);

            _dut = new UnvoidTagCommandHandler(
                projectRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUnvoidTagCommand_ShouldUnvoidTag()
        {
            // Arrange
            _tag.IsVoided = true;
            Assert.IsTrue(_tag.IsVoided);

            // Act
            var result = await _dut.Handle(_command, default);

            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsFalse(_tag.IsVoided);
        }

        [TestMethod]
        public async Task HandlingUnvoidTagCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingVoidTagCommand_ShouldSetAndReturnRowVersion()
        {
            // Arrange
            Assert.AreNotEqual(_command.RowVersion, _tag.RowVersion.ConvertToString());

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _tag.RowVersion.ConvertToString());
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.UnvoidTag;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.UnvoidTag
{
    [TestClass]
    public class UnvoidTagCommandHandlerTests : CommandHandlerTestsBase
    {
        private Tag _tag;
        private UnvoidTagCommand _command;
        private UnvoidTagCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var tagId = 2;
            var rowVersion = "AAAAAAAAABA=";

            var projectRepositoryMock = new Mock<IProjectRepository>();

            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Plant).Returns(TestPlant);

            var rdMock = new Mock<RequirementDefinition>();
            rdMock.SetupGet(rd => rd.Plant).Returns(TestPlant);

            var requirement = new TagRequirement(TestPlant, 2, rdMock.Object);
            _tag = new Tag(TestPlant, TagType.Standard, "", "", stepMock.Object,
                new List<TagRequirement> {requirement});

            projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(tagId))
                .Returns(Task.FromResult(_tag));

            _command = new UnvoidTagCommand(tagId, rowVersion);

            _dut = new UnvoidTagCommandHandler(
                projectRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUnvoidTagCommand_ShouldUnvoidTag()
        {
            // Arrange
            _tag.Void();
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
        public async Task HandlingVoidTagCommand_ShouldSetRowVersion()
        {
            // Act
            Assert.AreNotEqual(_command.RowVersion, _tag.RowVersion.ConvertToString());
            await _dut.Handle(_command, default);

            // Assert
            var updatedRowVersion = _tag.RowVersion.ConvertToString();
            Assert.AreEqual(_command.RowVersion, updatedRowVersion);
        }
    }
}

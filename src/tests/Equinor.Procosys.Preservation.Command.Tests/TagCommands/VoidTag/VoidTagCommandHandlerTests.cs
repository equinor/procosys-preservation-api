using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.VoidTag;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.VoidTag
{
    [TestClass]
    public class VoidTagCommandHandlerTests : CommandHandlerTestsBase
    {
        private Tag _tag;
        private VoidTagCommand _command;
        private VoidTagCommandHandler _dut;
        private readonly string _rowVersion = "AAAAAAAAABA=";

        [TestInitialize]
        public void Setup()
        {
            var tagId = 2;
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

            _command = new VoidTagCommand(tagId, _rowVersion, TestUserOid);

            _dut = new VoidTagCommandHandler(
                projectRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingVoidTagCommand_ShouldVoidTag()
        {
            // Arrange
            Assert.IsFalse(_tag.IsVoided);

            // Act
            var result = await _dut.Handle(_command, default);

            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsTrue(_tag.IsVoided);
        }

        [TestMethod]
        public async Task HandlingVoidTagCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(TestUserOid, default), Times.Once);
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

using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.VoidTag;
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
        private readonly bool _oldIsVoided = false;
        private readonly bool _newIsVoided = true;
        private Tag _tag;
        private VoidTagCommand _command;
        private VoidTagCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            int tagId = 2;
            int rdId1 = 17;
            int intervalWeeks = 2;

            var projectRepositoryMock = new Mock<IProjectRepository>();

            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Plant).Returns(TestPlant);

            var rdMock = new Mock<RequirementDefinition>();
            rdMock.SetupGet(rd => rd.Id).Returns(rdId1);
            rdMock.SetupGet(rd => rd.Plant).Returns(TestPlant);

            var requirement = new TagRequirement(TestPlant, intervalWeeks, rdMock.Object);
            _tag = new Tag(TestPlant, TagType.Standard, "", "", stepMock.Object, new List<TagRequirement> {requirement})
            {
                IsVoided = _oldIsVoided
            };

            projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(tagId))
                .Returns(Task.FromResult(_tag));

            _command = new VoidTagCommand(tagId, _newIsVoided);

            _dut = new VoidTagCommandHandler(
                projectRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingVoidTagCommand_ShouldVoidTag()
        {
            // Arrange
            Assert.AreEqual(_oldIsVoided, _tag.IsVoided);

            // Act
            var result = await _dut.Handle(_command, default);

            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_tag.IsVoided, _newIsVoided);
        }

        [TestMethod]
        public async Task HandlingVoidTagCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.UpdateTag;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.UpdateTag
{
    [TestClass]
    public class UpdateTagCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly string _oldRemark = "RemarkOld";
        private readonly string _newRemark = "RemarkNew";
        private readonly string _oldStorageArea = "StorageAreaOld";
        private readonly string _newStorageArea = "StorageAreaNew";
        private readonly int _rdId1 = 17;
        private readonly int _intervalWeeks = 2;
        private readonly string _rowVersion = "AAAAAAAAABA=";

        private TagRequirement _requirement;
        private Tag _tag;
        private UpdateTagCommand _command;
        private UpdateTagCommandHandler _dut;

        private Mock<IProjectRepository> _projectRepositoryMock;

        [TestInitialize]
        public void Setup()
        {
            _projectRepositoryMock = new Mock<IProjectRepository>();

            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Plant).Returns(TestPlant);

            var rdMock = new Mock<RequirementDefinition>();
            rdMock.SetupGet(rd => rd.Id).Returns(_rdId1);
            rdMock.SetupGet(rd => rd.Plant).Returns(TestPlant);

            _requirement = new TagRequirement(TestPlant, _intervalWeeks, rdMock.Object);
            _tag = new Tag(TestPlant, TagType.Standard, "", "", stepMock.Object, new List<TagRequirement> {_requirement})
            {
                StorageArea = _oldStorageArea,
                Remark = _oldRemark
            };

            var tagId = 2;
            _projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(tagId))
                .Returns(Task.FromResult(_tag));

            _command = new UpdateTagCommand(tagId, _newRemark, _newStorageArea, _rowVersion);

            _dut = new UpdateTagCommandHandler(
                _projectRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUpdateTagCommand_ShouldUpdateTag()
        {
            // Arrange
            Assert.AreEqual(_oldStorageArea, _tag.StorageArea);
            Assert.AreEqual(_oldRemark, _tag.Remark);

            // Act
            await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(_newStorageArea, _tag.StorageArea);
            Assert.AreEqual(_newRemark, _tag.Remark);
        }
                        
        [TestMethod]
        public async Task HandlingUpdateModeCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _tag.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingUpdateTagCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

    }
}

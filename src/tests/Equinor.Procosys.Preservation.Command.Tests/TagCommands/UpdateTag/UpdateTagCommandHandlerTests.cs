using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.UpdateTag;
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
        private readonly int _tagId = 2;
        private readonly string _oldRemark = "RemarkOld";
        private readonly string _newRemark = "RemarkNew";
        private readonly string _oldStorageArea = "StorageAreaOld";
        private readonly string _newStorageArea = "StorageAreaNew";
        private readonly int _rdId1 = 17;
        private readonly int _intervalWeeks = 2;

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

            var _rdMock = new Mock<RequirementDefinition>();
            _rdMock.SetupGet(rd => rd.Id).Returns(_rdId1);
            _rdMock.SetupGet(rd => rd.Plant).Returns(TestPlant);

            _requirement = new TagRequirement(TestPlant, _intervalWeeks, _rdMock.Object);
            _tag = new Tag(TestPlant, TagType.Standard, "", "", stepMock.Object, new List<TagRequirement> {_requirement})
            {
                StorageArea = _oldStorageArea,
                Remark = _oldRemark
            };

            _projectRepositoryMock
                .Setup(r => r.GetTagByTagIdAsync(_tagId))
                .Returns(Task.FromResult(_tag));

            _command = new UpdateTagCommand(_tagId, _newRemark, _newStorageArea);

            _dut = new UpdateTagCommandHandler(
                _projectRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUpdateTagCommand_ShouldUpdateTag()
        {
            var result = await _dut.Handle(_command, default);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_tag.StorageArea, _newStorageArea);
            Assert.AreEqual(_tag.Remark, _newRemark);
        }

        [TestMethod]
        public async Task HandlingUpdateTagCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.StartPreservation;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TagRequirement = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Requirement;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.StartPreservation
{
    [TestClass]
    public class StartPreservationCommandHandlerTests : CommandHandlerTestsBase
    {
        private Mock<IProjectRepository> _tagRepoMock;
        private StartPreservationCommand _command;
        private Tag _tag1;
        private Tag _tag2;
        private TagRequirement _req1OnTag1;
        private TagRequirement _req2OnTag1;
        private TagRequirement _req1OnTag2;
        private TagRequirement _req2OnTag2;
        private Mock<RequirementDefinition> _rd1Mock;
        private Mock<RequirementDefinition> _rd2Mock;

        private int _rdId1 = 17;
        private int _rdId2 = 18;
        private int _tagId1 = 7;
        private int _tagId2 = 8;
        private int _intervalWeeks = 2;

        private StartPreservationCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Schema).Returns(TestPlant);
            _rd1Mock = new Mock<RequirementDefinition>();
            _rd1Mock.SetupGet(rd => rd.Id).Returns(_rdId1);
            _rd1Mock.SetupGet(rd => rd.Schema).Returns(TestPlant);
            _rd2Mock = new Mock<RequirementDefinition>();
            _rd2Mock.SetupGet(rd => rd.Id).Returns(_rdId2);
            _rd2Mock.SetupGet(rd => rd.Schema).Returns(TestPlant);

            _req1OnTag1 = new TagRequirement(TestPlant, _intervalWeeks, _rd1Mock.Object);
            _req2OnTag1 = new TagRequirement(TestPlant, _intervalWeeks, _rd2Mock.Object);
            _req1OnTag2 = new TagRequirement(TestPlant, _intervalWeeks, _rd1Mock.Object);
            _req2OnTag2 = new TagRequirement(TestPlant, _intervalWeeks, _rd2Mock.Object);
            _tag1 = new Tag(TestPlant, TagType.Standard, "", "", "", "", "", "", "", "", "", "", "", stepMock.Object, new List<TagRequirement>
            {
                _req1OnTag1, _req2OnTag1
            });
            _tag2 = new Tag(TestPlant, TagType.Standard, "", "", "", "", "", "", "", "", "", "", "", stepMock.Object, new List<TagRequirement>
            {
                _req1OnTag2, _req2OnTag2
            });
            var tags = new List<Tag>
            {
                _tag1, _tag2
            };

            var tagIds = new List<int> {_tagId1, _tagId2};
            _tagRepoMock = new Mock<IProjectRepository>();
            _tagRepoMock.Setup(r => r.GetTagsByTagIdsAsync(tagIds)).Returns(Task.FromResult(tags));
            _command = new StartPreservationCommand(tagIds);

            _dut = new StartPreservationCommandHandler(_tagRepoMock.Object, UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingStartPreservationCommand_ShouldStartPreservationOnAllTags()
        {
            var result = await _dut.Handle(_command, default);

            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsInstanceOfType(result.Data, typeof(Unit));

            Assert.AreEqual(PreservationStatus.Active, _tag1.Status);
            Assert.AreEqual(PreservationStatus.Active, _tag2.Status);
        }

        [TestMethod]
        public async Task HandlingStartPreservationCommand_ShouldStartPreservationOnAllRequirementsOnAllTags()
        {
            await _dut.Handle(_command, default);

            var expectedNextDueTimeUtc = _utcNow.AddWeeks(_intervalWeeks);
            Assert.AreEqual(expectedNextDueTimeUtc, _req1OnTag1.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtc, _req2OnTag1.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtc, _req1OnTag2.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtc, _req2OnTag2.NextDueTimeUtc);

            Assert.AreEqual(expectedNextDueTimeUtc, _tag1.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtc, _tag2.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingStartPreservationCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.StopPreservation;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TagRequirement = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Requirement;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.StopPreservation
{
    [TestClass]
    public class StopPreservationCommandHandlerTests : CommandHandlerTestsBase
    {
        private Mock<IProjectRepository> _tagRepoMock;
        private StopPreservationCommand _command;
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

        private int _stepId1 = 9;
        private int _stepId2 = 10;

        private StopPreservationCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var step1Mock = new Mock<Step>();
            step1Mock.SetupGet(s => s.Plant).Returns(TestPlant);
            step1Mock.SetupGet(s => s.Id).Returns(_stepId1);
            
            var step2Mock = new Mock<Step>();
            step2Mock.SetupGet(s => s.Plant).Returns(TestPlant);
            step2Mock.SetupGet(s => s.Id).Returns(_stepId2);

            var journey = new Journey(TestPlant, "Demissie");
            journey.AddStep(step1Mock.Object);
            journey.AddStep(step2Mock.Object);

            _rd1Mock = new Mock<RequirementDefinition>();
            _rd1Mock.SetupGet(rd => rd.Id).Returns(_rdId1);
            _rd1Mock.SetupGet(rd => rd.Plant).Returns(TestPlant);
            _rd2Mock = new Mock<RequirementDefinition>();
            _rd2Mock.SetupGet(rd => rd.Id).Returns(_rdId2);
            _rd2Mock.SetupGet(rd => rd.Plant).Returns(TestPlant);

            _req1OnTag1 = new TagRequirement(TestPlant, 2, _rd1Mock.Object);
            _req2OnTag1 = new TagRequirement(TestPlant, 2, _rd2Mock.Object);
            _req1OnTag2 = new TagRequirement(TestPlant, 2, _rd1Mock.Object);
            _req2OnTag2 = new TagRequirement(TestPlant, 2, _rd2Mock.Object);
            _tag1 = new Tag(TestPlant, TagType.Standard, "", "", step2Mock.Object, new List<TagRequirement>
            {
                _req1OnTag1, _req2OnTag1
            });
            _tag1.StartPreservation();

            _tag2 = new Tag(TestPlant, TagType.Standard, "", "", step2Mock.Object, new List<TagRequirement>
            {
                _req1OnTag2, _req2OnTag2
            });
            _tag2.StartPreservation();

            var tags = new List<Tag>
            {
                _tag1, _tag2
            };
            
            var tagIds = new List<int> { _tagId1, _tagId2 };
            _tagRepoMock = new Mock<IProjectRepository>();
            _tagRepoMock.Setup(r => r.GetTagsByTagIdsAsync(tagIds)).Returns(Task.FromResult(tags));

            var journeyRepoMock = new Mock<IJourneyRepository>();
            journeyRepoMock
                .Setup(r => r.GetJourneysByStepIdsAsync(new List<int> { _stepId2 }))
                .Returns(Task.FromResult(new List<Journey> { journey }));
            
            _command = new StopPreservationCommand(tagIds);

            _dut = new StopPreservationCommandHandler(_tagRepoMock.Object, journeyRepoMock.Object, UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingStopPreservationCommand_ShouldStopPreservationOnAllTags()
        {
            var result = await _dut.Handle(_command, default);

            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsInstanceOfType(result.Data, typeof(Unit));

            Assert.AreEqual(PreservationStatus.Completed, _tag1.Status);
            Assert.AreEqual(PreservationStatus.Completed, _tag2.Status);
        }

        [TestMethod]
        public async Task HandlingStopPreservationCommand_ShouldClearNextDueOnAllRequirementsOnAllTags()
        {
            await _dut.Handle(_command, default);

            Assert.IsNull(_req1OnTag1.NextDueTimeUtc);
            Assert.IsNull(_req2OnTag1.NextDueTimeUtc);
            Assert.IsNull(_req1OnTag2.NextDueTimeUtc);
            Assert.IsNull(_req2OnTag2.NextDueTimeUtc);

            Assert.IsNull(_tag1.NextDueTimeUtc);
            Assert.IsNull(_tag2.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingStopPreservationCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }
    }
}

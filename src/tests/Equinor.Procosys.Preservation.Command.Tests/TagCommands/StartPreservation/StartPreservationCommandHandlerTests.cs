using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.StartPreservation;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.StartPreservation
{
    [TestClass]
    public class StartPreservationCommandHandlerTests
    {
        private DateTime _utcNow;
        private Mock<ITagRepository> _tagRepoMock;
        private Mock<IRequirementTypeRepository> _rtRepoMock;
        private Mock<ITimeService> _timeServiceMock;
        private Mock<IUnitOfWork> _uowMock;
        private StartPreservationCommand _command;
        private Tag _tag1;
        private Tag _tag2;
        private Requirement _req1OnTag1;
        private Requirement _req2OnTag1;
        private Requirement _req1OnTag2;
        private Requirement _req2OnTag2;
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
            _rd1Mock = new Mock<RequirementDefinition>();
            _rd1Mock.SetupGet(rd => rd.Id).Returns(_rdId1);
            _rd2Mock = new Mock<RequirementDefinition>();
            _rd2Mock.SetupGet(rd => rd.Id).Returns(_rdId2);

            _req1OnTag1 = new Requirement("", _intervalWeeks, _rd1Mock.Object);
            _req2OnTag1 = new Requirement("", _intervalWeeks, _rd2Mock.Object);
            _req1OnTag2 = new Requirement("", _intervalWeeks, _rd1Mock.Object);
            _req2OnTag2 = new Requirement("", _intervalWeeks, _rd2Mock.Object);
            _tag1 = new Tag("", "", "", "", "", "", "", "", "", "", stepMock.Object, new List<Requirement>
            {
                _req1OnTag1, _req2OnTag1
            });
            _tag2 = new Tag("", "", "", "", "", "", "", "", "", "", stepMock.Object, new List<Requirement>
            {
                _req1OnTag2, _req2OnTag2
            });
            var tags = new List<Tag>
            {
                _tag1, _tag2
            };

            var tagIds = new List<int> {_tagId1, _tagId2};
            _tagRepoMock = new Mock<ITagRepository>();
            _rtRepoMock = new Mock<IRequirementTypeRepository>();
            _rtRepoMock.Setup(r => r.GetRequirementDefinitionsByIdsAsync(new List<int> {_rdId1, _rdId2}))
                .Returns(Task.FromResult(new List<RequirementDefinition> {_rd1Mock.Object, _rd2Mock.Object}));
            _tagRepoMock.Setup(r => r.GetByIdsAsync(tagIds)).Returns(Task.FromResult(tags));
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _timeServiceMock = new Mock<ITimeService>();
            _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(_utcNow);
            _uowMock = new Mock<IUnitOfWork>();
            _command = new StartPreservationCommand(tagIds);

            _dut = new StartPreservationCommandHandler(_tagRepoMock.Object, _rtRepoMock.Object, _timeServiceMock.Object, _uowMock.Object);
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
        }

        [TestMethod]
        public async Task HandlingStartPreservationCommand_ShouldUpdateNeedsUserInputOnAllRequirementsOnAllTags()
        {
            await _dut.Handle(_command, default);

            Assert.AreEqual(_rd1Mock.Object.NeedsUserInput, _req1OnTag1.NeedsUserInput);
            Assert.AreEqual(_rd2Mock.Object.NeedsUserInput, _req2OnTag1.NeedsUserInput);
            Assert.AreEqual(_rd1Mock.Object.NeedsUserInput, _req1OnTag2.NeedsUserInput);
            Assert.AreEqual(_rd2Mock.Object.NeedsUserInput, _req2OnTag2.NeedsUserInput);
        }

        [TestMethod]
        public async Task HandlingStartPreservationCommand_ShouldSaveUnitOfWork()
        {
            var token = new CancellationToken();
            
            await _dut.Handle(_command, token);

            _uowMock.Verify(r => r.SaveChangesAsync(token), Times.Once);
        }
    }
}

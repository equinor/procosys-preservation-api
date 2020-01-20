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
        private Mock<Requirement> _req1OnTag1Mock;
        private Mock<Requirement> _req2OnTag1Mock;
        private Mock<Requirement> _req1OnTag2Mock;
        private Mock<Requirement> _req2OnTag2Mock;

        private int _tagId1 = 7;
        private int _tagId2 = 8;

        private StartPreservationCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var stepMock = new Mock<Step>();
            _req1OnTag1Mock = new Mock<Requirement>();
            _req2OnTag1Mock = new Mock<Requirement>();
            _req1OnTag2Mock = new Mock<Requirement>();
            _req2OnTag2Mock = new Mock<Requirement>();
            _tag1 = new Tag("", "", "", "", "", "", "", "", "", "", stepMock.Object, new List<Requirement>
            {
                _req1OnTag1Mock.Object, _req2OnTag1Mock.Object
            });
            _tag2 = new Tag("", "", "", "", "", "", "", "", "", "", stepMock.Object, new List<Requirement>
            {
                _req1OnTag2Mock.Object, _req2OnTag2Mock.Object
            });
            var tags = new List<Tag>
            {
                _tag1, _tag2
            };

            var tagIds = new List<int> {_tagId1, _tagId2};
            _tagRepoMock = new Mock<ITagRepository>();
            _rtRepoMock = new Mock<IRequirementTypeRepository>();
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
        public async Task HandlingStartPreservationCommand_ShouldSetNextDueTimeOnAllRequirementsOnAllTags()
        {
            await _dut.Handle(_command, default);

            _req1OnTag1Mock.Verify(r => r.StartPreservation(_utcNow), Times.Once);
            _req2OnTag1Mock.Verify(r => r.StartPreservation(_utcNow), Times.Once);
            
            _req1OnTag2Mock.Verify(r => r.StartPreservation(_utcNow), Times.Once);
            _req2OnTag2Mock.Verify(r => r.StartPreservation(_utcNow), Times.Once);
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

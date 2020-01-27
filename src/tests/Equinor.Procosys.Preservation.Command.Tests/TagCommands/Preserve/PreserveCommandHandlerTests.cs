using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.Preserve;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.Preserve
{
    [TestClass]
    public class PreserveCommandHandlerTests : CommandHandlerTestsBase
    {
        private DateTime _utcNow;
        private Mock<IProjectRepository> _projectRepoMock;
        private Mock<IPersonRepository> _personRepoMock;
        private Mock<ITimeService> _timeServiceMock;
        private PreserveCommand _command;
        private Tag _tag1;
        private Tag _tag2;
        private Requirement _req1OnTag1;
        private Requirement _req2OnTag1;
        private Requirement _req1OnTag2;
        private Requirement _req2OnTag2;
        private Mock<RequirementDefinition> _rd1Mock;
        private Mock<RequirementDefinition> _rd2Mock;

        private const int _rdId1 = 17;
        private const int _rdId2 = 18;
        private const int _tagId1 = 7;
        private const int _tagId2 = 8;
        private const int _personId = 142;
        private const int _intervalWeeks = 2;

        private PreserveCommandHandler _dut;
        private Mock<Person> _personMock;

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

            _personMock = new Mock<Person>();
            _personMock.SetupGet(rd => rd.Id).Returns(_personId);
            var tagIds = new List<int> {_tagId1, _tagId2};
            _projectRepoMock = new Mock<IProjectRepository>();
            _projectRepoMock.Setup(r => r.GetTagsByTagIdsAsync(tagIds)).Returns(Task.FromResult(tags));
            _personRepoMock = new Mock<IPersonRepository>();
            _personRepoMock.Setup(r => r.GetByOidAsync(Guid.Empty)).Returns(Task.FromResult(_personMock.Object));
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _timeServiceMock = new Mock<ITimeService>();
            _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(_utcNow);
            _command = new PreserveCommand(tagIds, true);

            _tag1.StartPreservation(_utcNow.AddDays(-14));
            _tag2.StartPreservation(_utcNow.AddDays(-14));

            _dut = new PreserveCommandHandler(_projectRepoMock.Object, _personRepoMock.Object, _timeServiceMock.Object, UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldPreserveAllRequirementsOnAllTags()
        {
            await _dut.Handle(_command, default);

            var expectedNextDueTimeUtc = _utcNow.AddWeeks(_intervalWeeks);
            Assert.AreEqual(expectedNextDueTimeUtc, _req1OnTag1.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtc, _req2OnTag1.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtc, _req1OnTag2.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtc, _req2OnTag2.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }
    }
}

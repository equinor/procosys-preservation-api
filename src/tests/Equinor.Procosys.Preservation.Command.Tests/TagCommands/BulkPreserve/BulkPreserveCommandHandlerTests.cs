using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.BulkPreserve;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.BulkPreserve
{
    [TestClass]
    public class BulkPreserveCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int RdId1 = 17;
        private const int RdId2 = 18;
        private const int TagId1 = 7;
        private const int TagId2 = 8;
        private const int TwoWeeksInterval = 2;
        private const int FourWeeksInterval = 4;

        private DateTime _startedPreservedAtUtc;
        private Mock<IProjectRepository> _projectRepoMock;
        private Mock<ICurrentUserProvider> _currentUserProvider;
        private Mock<ITimeService> _timeServiceMock;
        private BulkPreserveCommand _command;
        private Tag _tag1;
        private Tag _tag2;
        private Requirement _req1OnTag1WithTwoWeekInterval;
        private Requirement _req2OnTag1WithFourWeekInterval;
        private Requirement _req1OnTag2WithTwoWeekInterval;
        private Requirement _req2OnTag2WithFourWeekInterval;
        private Mock<RequirementDefinition> _rd1Mock;
        private Mock<RequirementDefinition> _rd2Mock;

        private BulkPreserveCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var stepMock = new Mock<Step>();
            _rd1Mock = new Mock<RequirementDefinition>();
            _rd1Mock.SetupGet(rd => rd.Id).Returns(RdId1);
            _rd2Mock = new Mock<RequirementDefinition>();
            _rd2Mock.SetupGet(rd => rd.Id).Returns(RdId2);

            _req1OnTag1WithTwoWeekInterval = new Requirement("", TwoWeeksInterval, _rd1Mock.Object);
            _req2OnTag1WithFourWeekInterval = new Requirement("", FourWeeksInterval, _rd2Mock.Object);
            _req1OnTag2WithTwoWeekInterval = new Requirement("", TwoWeeksInterval, _rd1Mock.Object);
            _req2OnTag2WithFourWeekInterval = new Requirement("", FourWeeksInterval, _rd2Mock.Object);
            _tag1 = new Tag("", "", "", "", "", "", "", "", "", "", "", stepMock.Object, new List<Requirement>
            {
                _req1OnTag1WithTwoWeekInterval, _req2OnTag1WithFourWeekInterval
            });
            _tag2 = new Tag("", "", "", "", "", "", "", "", "", "", "", stepMock.Object, new List<Requirement>
            {
                _req1OnTag2WithTwoWeekInterval, _req2OnTag2WithFourWeekInterval
            });
            var tags = new List<Tag>
            {
                _tag1, _tag2
            };
            _currentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUserProvider
                .Setup(x => x.GetCurrentUserAsync())
                .Returns(Task.FromResult(new Person(new Guid("12345678-1234-1234-1234-123456789123"), "Firstname", "Lastname")));
            var tagIds = new List<int> {TagId1, TagId2};
            _projectRepoMock = new Mock<IProjectRepository>();
            _projectRepoMock.Setup(r => r.GetTagsByTagIdsAsync(tagIds)).Returns(Task.FromResult(tags));
            _startedPreservedAtUtc = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _timeServiceMock = new Mock<ITimeService>();
            _command = new BulkPreserveCommand(tagIds);

            _tag1.StartPreservation(_startedPreservedAtUtc);
            _tag2.StartPreservation(_startedPreservedAtUtc);

            _dut = new BulkPreserveCommandHandler(
                _projectRepoMock.Object,
                _timeServiceMock.Object,
                UnitOfWorkMock.Object,
                _currentUserProvider.Object);
        }

        [TestMethod]
        public async Task HandlingBulkPreserveCommand_ShouldPreserveFirstRequirementsOnAllTags_WhenOnDueAtFirstRequirement()
        {
            var currentTimeUtc = _startedPreservedAtUtc.AddWeeks(TwoWeeksInterval);
            var oldNextDueOnReq2OnTag1 = _req2OnTag1WithFourWeekInterval.NextDueTimeUtc;
            var oldNextDueOnReq2OnTag2 = _req2OnTag2WithFourWeekInterval.NextDueTimeUtc;
            
            _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(currentTimeUtc);

            await _dut.Handle(_command, default);

            var expectedNextDueTimeUtcForTwoWeeksInterval = currentTimeUtc.AddWeeks(TwoWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeUtcForTwoWeeksInterval, _req1OnTag1WithTwoWeekInterval.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtcForTwoWeeksInterval, _req1OnTag2WithTwoWeekInterval.NextDueTimeUtc);

            Assert.AreEqual(oldNextDueOnReq2OnTag1, _req2OnTag1WithFourWeekInterval.NextDueTimeUtc);
            Assert.AreEqual(oldNextDueOnReq2OnTag2, _req2OnTag2WithFourWeekInterval.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingBulkPreserveCommand_ShouldPreserveAllRequirementsOnAllTags_WhenOnDueAtLatestRequirement()
        {
            var currentTimeUtc = _startedPreservedAtUtc.AddWeeks(FourWeeksInterval);
            
            _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(currentTimeUtc);

            await _dut.Handle(_command, default);

            var expectedNextDueTimeUtcForTwoWeeksInterval = currentTimeUtc.AddWeeks(TwoWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeUtcForTwoWeeksInterval, _req1OnTag1WithTwoWeekInterval.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtcForTwoWeeksInterval, _req1OnTag2WithTwoWeekInterval.NextDueTimeUtc);

            var expectedNextDueTimeUtcForFourWeeksInterval = currentTimeUtc.AddWeeks(FourWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeUtcForFourWeeksInterval, _req2OnTag1WithFourWeekInterval.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtcForFourWeeksInterval, _req2OnTag2WithFourWeekInterval.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingBulkPreserveCommand_ShouldThrowException_WhenBeforeDueForAnyRequirement()
        {
            _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(_startedPreservedAtUtc);

            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _dut.Handle(_command, default)
            );
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldSave_WhenOnDueForFirstRequirement()
        {
            _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(_startedPreservedAtUtc.AddWeeks(TwoWeeksInterval));
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldSave_WhenOnDueForLastRequirement()
        {
            _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(_startedPreservedAtUtc.AddWeeks(FourWeeksInterval));
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingBulkPreserveCommand_ShouldNotSave_WhenBeforeDueForAnyRequirement()
        {
            _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(_startedPreservedAtUtc);
            
            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _dut.Handle(_command, default)
            );

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }
    }
}

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
        private const int TagId1 = 7;
        private const int TagId2 = 8;
        private const int TwoWeeksInterval = 2;
        private const int FourWeeksInterval = 4;

        private Guid _currentUserOid = new Guid("12345678-1234-1234-1234-123456789123");
        private Mock<IProjectRepository> _projectRepoMock;
        private Mock<IPersonRepository> _personRepoMock;
        private Mock<ICurrentUserProvider> _currentUserProvider;
        private BulkPreserveCommand _command;
        private Tag _tag1;
        private Tag _tag2;
        private Requirement _req1OnTag1WithTwoWeekInterval;
        private Requirement _req2OnTag1WithFourWeekInterval;
        private Requirement _req1OnTag2WithTwoWeekInterval;
        private Requirement _req2OnTag2WithFourWeekInterval;

        private BulkPreserveCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
            var rdMock = new Mock<RequirementDefinition>();
            rdMock.SetupGet(rd => rd.Plant).Returns(TestPlant);

            _req1OnTag1WithTwoWeekInterval = new Requirement(TestPlant, TwoWeeksInterval, rdMock.Object);
            _req2OnTag1WithFourWeekInterval = new Requirement(TestPlant, FourWeeksInterval, rdMock.Object);
            _req1OnTag2WithTwoWeekInterval = new Requirement(TestPlant, TwoWeeksInterval, rdMock.Object);
            _req2OnTag2WithFourWeekInterval = new Requirement(TestPlant, FourWeeksInterval, rdMock.Object);
            _tag1 = new Tag(TestPlant, TagType.Standard, "", "", stepMock.Object, new List<Requirement>
            {
                _req1OnTag1WithTwoWeekInterval, _req2OnTag1WithFourWeekInterval
            });
            _tag2 = new Tag(TestPlant, TagType.Standard, "", "", stepMock.Object, new List<Requirement>
            {
                _req1OnTag2WithTwoWeekInterval, _req2OnTag2WithFourWeekInterval
            });
            var tags = new List<Tag>
            {
                _tag1, _tag2
            };
            _currentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUserProvider
                .Setup(x => x.GetCurrentUser())
                .Returns(_currentUserOid);
            var tagIds = new List<int> {TagId1, TagId2};
            _projectRepoMock = new Mock<IProjectRepository>();
            _projectRepoMock.Setup(r => r.GetTagsByTagIdsAsync(tagIds)).Returns(Task.FromResult(tags));
            _personRepoMock = new Mock<IPersonRepository>();
            _personRepoMock
                .Setup(p => p.GetByOidAsync(It.Is<Guid>(x => x == _currentUserOid)))
                .Returns(Task.FromResult(new Person(_currentUserOid, "Test", "User")));
            _command = new BulkPreserveCommand(tagIds);

            _tag1.StartPreservation();
            _tag2.StartPreservation();

            _dut = new BulkPreserveCommandHandler(
                _projectRepoMock.Object,
                _personRepoMock.Object,
                UnitOfWorkMock.Object,
                _currentUserProvider.Object);
        }

        [TestMethod]
        public async Task HandlingBulkPreserveCommand_ShouldPreserveFirstRequirementsOnAllTags_WhenOnDueAtFirstRequirement()
        {
            var oldNextDueOnReq2OnTag1 = _req2OnTag1WithFourWeekInterval.NextDueTimeUtc;
            var oldNextDueOnReq2OnTag2 = _req2OnTag2WithFourWeekInterval.NextDueTimeUtc;
            var req1OnTag1WithTwoWeekIntervalInitialPeriod = _req1OnTag1WithTwoWeekInterval.ActivePeriod;
            var req1OnTag2WithTwoWeekIntervalInitialPeriod = _req1OnTag2WithTwoWeekInterval.ActivePeriod;
            var req2OnTag1WithFourWeekIntervalInitialPeriod = _req2OnTag1WithFourWeekInterval.ActivePeriod;
            var req2OnTag2WithFourWeekIntervalInitialPeriod = _req2OnTag2WithFourWeekInterval.ActivePeriod;

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            await _dut.Handle(_command, default);

            var expectedNextDueTimeUtcForTwoWeeksInterval = _timeProvider.UtcNow.AddWeeks(TwoWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeUtcForTwoWeeksInterval, _req1OnTag1WithTwoWeekInterval.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtcForTwoWeeksInterval, _req1OnTag2WithTwoWeekInterval.NextDueTimeUtc);

            Assert.AreEqual(oldNextDueOnReq2OnTag1, _req2OnTag1WithFourWeekInterval.NextDueTimeUtc);
            Assert.AreEqual(oldNextDueOnReq2OnTag2, _req2OnTag2WithFourWeekInterval.NextDueTimeUtc);

            Assert.IsNotNull(req1OnTag1WithTwoWeekIntervalInitialPeriod.PreservationRecord);
            Assert.IsNotNull(req1OnTag2WithTwoWeekIntervalInitialPeriod.PreservationRecord);
            Assert.IsNull(req2OnTag1WithFourWeekIntervalInitialPeriod.PreservationRecord);
            Assert.IsNull(req2OnTag2WithFourWeekIntervalInitialPeriod.PreservationRecord);
        }

        [TestMethod]
        public async Task HandlingBulkPreserveCommand_ShouldPreserveAllRequirementsOnAllTags_WhenOnDueAtLatestRequirement()
        {
            _timeProvider.ElapseWeeks(FourWeeksInterval);

            await _dut.Handle(_command, default);

            var expectedNextDueTimeUtcForTwoWeeksInterval = _timeProvider.UtcNow.AddWeeks(TwoWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeUtcForTwoWeeksInterval, _req1OnTag1WithTwoWeekInterval.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtcForTwoWeeksInterval, _req1OnTag2WithTwoWeekInterval.NextDueTimeUtc);

            var expectedNextDueTimeUtcForFourWeeksInterval = _timeProvider.UtcNow.AddWeeks(FourWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeUtcForFourWeeksInterval, _req2OnTag1WithFourWeekInterval.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtcForFourWeeksInterval, _req2OnTag2WithFourWeekInterval.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingBulkPreserveCommand_ShouldThrowException_WhenBeforeDueForAnyRequirement()
        {
            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _dut.Handle(_command, default)
            );
        }

        [TestMethod]
        public async Task HandlingBulkPreserveCommand_ShouldSave_WhenOnDueForFirstRequirement()
        {
            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingBulkPreserveCommand_ShouldSave_WhenOnDueForLastRequirement()
        {
            _timeProvider.ElapseWeeks(FourWeeksInterval);
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingBulkPreserveCommand_ShouldNotSave_WhenBeforeDueForAnyRequirement()
        {            
            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _dut.Handle(_command, default)
            );

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }
    }
}

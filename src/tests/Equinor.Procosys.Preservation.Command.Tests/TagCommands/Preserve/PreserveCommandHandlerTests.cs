using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.Preserve;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.Preserve
{
    [TestClass]
    public class PreserveCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int TagId = 7;
        private const int TwoWeeksInterval = 2;
        private const int FourWeeksInterval = 4;

        private DateTime _utcNow;
        private Mock<IProjectRepository> _projectRepoMock;
        private Mock<ICurrentUserProvider> _currentUserProvider;
        private Mock<ITimeService> _timeServiceMock;
        private PreserveCommand _command;
        private Tag _tag;
        private Requirement _req1WithTwoWeekInterval;
        private Requirement _req2WithTwoWeekInterval;
        private Requirement _req3WithFourWeekInterval;

        private PreserveCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var stepMock = new Mock<Step>();
            var rdMock = new Mock<RequirementDefinition>();

            _req1WithTwoWeekInterval = new Requirement("", TwoWeeksInterval, rdMock.Object);
            _req2WithTwoWeekInterval = new Requirement("", TwoWeeksInterval, rdMock.Object);
            _req3WithFourWeekInterval = new Requirement("", FourWeeksInterval, rdMock.Object);
            _tag = new Tag("", "", "", "", "", "", "", "", "", "", "", stepMock.Object, new List<Requirement>
            {
                _req1WithTwoWeekInterval, 
                _req2WithTwoWeekInterval,
                _req3WithFourWeekInterval
            });
            _currentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUserProvider
                .Setup(x => x.GetCurrentUserAsync())
                .Returns(Task.FromResult(new Person(new Guid("12345678-1234-1234-1234-123456789123"), "Firstname", "Lastname")));
            _projectRepoMock = new Mock<IProjectRepository>();
            _projectRepoMock.Setup(r => r.GetTagByTagIdAsync(TagId)).Returns(Task.FromResult(_tag));
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _timeServiceMock = new Mock<ITimeService>();
            _command = new PreserveCommand(TagId);

            _tag.StartPreservation(_utcNow);

            _dut = new PreserveCommandHandler(_projectRepoMock.Object, _timeServiceMock.Object, UnitOfWorkMock.Object, _currentUserProvider.Object);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldPreserveRequirementsOnTag_IsDue()
        {
            var preservedAtUtc = _utcNow.AddWeeks(TwoWeeksInterval);
            _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(preservedAtUtc);

            await _dut.Handle(_command, default);

            var expectedNextDueTimeUtc = preservedAtUtc.AddWeeks(TwoWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeUtc, _req1WithTwoWeekInterval.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtc, _req2WithTwoWeekInterval.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldSkipPreservingRequirementsOnTag_NotDue()
        {
            var preservedAtUtc = _utcNow.AddWeeks(TwoWeeksInterval);
            _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(preservedAtUtc);
            var oldNextDue = _req3WithFourWeekInterval.NextDueTimeUtc;

            await _dut.Handle(_command, default);

            Assert.AreEqual(oldNextDue, _req3WithFourWeekInterval.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }
    }
}

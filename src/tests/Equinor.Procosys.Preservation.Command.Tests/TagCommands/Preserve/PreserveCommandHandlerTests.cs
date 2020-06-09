using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.Preserve;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.Preserve
{
    [TestClass]
    public class PreserveCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int ModeId = 1;
        private const int TagId = 7;
        private const int StepId = 17;
        private const int TwoWeeksInterval = 2;
        private const int FourWeeksInterval = 4;

        private readonly Guid _currentUserOid = new Guid("12345678-1234-1234-1234-123456789123");
        private Mock<IProjectRepository> _projectRepoMock;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Mock<IModeRepository> _modeRepositoryMock;
        private Mock<IPersonRepository> _personRepoMock;
        private Mock<ICurrentUserProvider> _currentUserProvider;
        private PreserveCommand _command;
        private Tag _tag;
        private TagRequirement _req1WithTwoWeekInterval;
        private TagRequirement _req2WithTwoWeekInterval;
        private TagRequirement _req3WithFourWeekInterval;

        private PreserveCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var mode = new Mode(TestPlant, "SUP", true);
            mode.SetProtectedIdForTesting(ModeId);
            var step = new Step(TestPlant, "SUP", mode, new Responsible(TestPlant, "C", "T"));
            step.SetProtectedIdForTesting(StepId);
            var rdMock = new Mock<RequirementDefinition>();
            rdMock.SetupGet(s => s.Plant).Returns(TestPlant);

            _req1WithTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, rdMock.Object);
            _req2WithTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, rdMock.Object);
            _req3WithFourWeekInterval = new TagRequirement(TestPlant, FourWeeksInterval, rdMock.Object);
            _tag = new Tag(TestPlant, TagType.Standard, "", "", step, new List<TagRequirement>
            {
                _req1WithTwoWeekInterval, 
                _req2WithTwoWeekInterval,
                _req3WithFourWeekInterval
            });
            _currentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUserProvider
                .Setup(x => x.GetCurrentUserOid())
                .Returns(_currentUserOid);
            _projectRepoMock = new Mock<IProjectRepository>();
            _projectRepoMock.Setup(r => r.GetTagByTagIdAsync(TagId)).Returns(Task.FromResult(_tag));
            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journeyRepositoryMock.Setup(j => j.GetStepByStepIdAsync(StepId)).Returns(Task.FromResult(step));
            _modeRepositoryMock = new Mock<IModeRepository>();
            _modeRepositoryMock.Setup(m => m.GetByIdAsync(ModeId)).Returns(Task.FromResult(mode));
            _personRepoMock = new Mock<IPersonRepository>();
            _personRepoMock
                .Setup(p => p.GetByOidAsync(It.Is<Guid>(x => x == _currentUserOid)))
                .Returns(Task.FromResult(new Person(_currentUserOid, "Test", "User")));
            _command = new PreserveCommand(TagId);

            _tag.StartPreservation();

            _dut = new PreserveCommandHandler(
                _projectRepoMock.Object,
                _journeyRepositoryMock.Object,
                _modeRepositoryMock.Object,
                _personRepoMock.Object,
                UnitOfWorkMock.Object,
                _currentUserProvider.Object);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldPreserveRequirementsOnTag_IsDue()
        {
            var req1WithTwoWeekIntervalInitialPeriod = _req1WithTwoWeekInterval.ActivePeriod;
            var req2WithTwoWeekIntervalInitialPeriod = _req2WithTwoWeekInterval.ActivePeriod;
            var req3WithFourWeekIntervalInitialPeriod = _req3WithFourWeekInterval.ActivePeriod;

            _timeProvider.ElapseWeeks(TwoWeeksInterval);

            await _dut.Handle(_command, default);

            var expectedNextDueTimeUtc = _timeProvider.UtcNow.AddWeeks(TwoWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeUtc, _req1WithTwoWeekInterval.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtc, _tag.NextDueTimeUtc);
            Assert.IsNotNull(req1WithTwoWeekIntervalInitialPeriod.PreservationRecord);
            Assert.AreEqual(expectedNextDueTimeUtc, _req2WithTwoWeekInterval.NextDueTimeUtc);
            Assert.IsNotNull(req2WithTwoWeekIntervalInitialPeriod.PreservationRecord);
            Assert.IsNull(req3WithFourWeekIntervalInitialPeriod.PreservationRecord);
        }

        public async Task HandlingPreserveCommand_ShouldPreserveRequirementsForSupplier_WhenTagIsInSupplierStep()
        {
            // todo 
        }

        public async Task HandlingPreserveCommand_ShouldNotPreserveRequirementsForSupplier_WhenTagIsInOtherStep()
        {
            // todo 
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldSkipPreservingRequirementsOnTag_NotDue()
        {
            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            var oldNextDue = _req3WithFourWeekInterval.NextDueTimeUtc;

            await _dut.Handle(_command, default);

            Assert.AreEqual(oldNextDue, _req3WithFourWeekInterval.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldSave_WhenOnDueForFirstRequirement()
        {
            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldSave_WhenOnDueForLastRequirement()
        {
            _timeProvider.ElapseWeeks(FourWeeksInterval);
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldNotSave_WhenBeforeDueForAnyRequirement()
        {
            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _dut.Handle(_command, default)
            );

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }
    }
}

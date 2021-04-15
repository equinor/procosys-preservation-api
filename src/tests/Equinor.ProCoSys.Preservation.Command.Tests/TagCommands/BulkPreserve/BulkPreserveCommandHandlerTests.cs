using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.BulkPreserve;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using HeboTech.TimeService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.BulkPreserve
{
    [TestClass]
    public class BulkPreserveCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int ModeId = 1;
        private const int StepId = 17;
        private const int TagId1 = 7;
        private const int TagId2 = 8;
        private const int TwoWeeksInterval = 2;
        private const int FourWeeksInterval = 4;

        private Mock<IProjectRepository> _projectRepoMock;
        private Mock<IPersonRepository> _personRepoMock;
        private BulkPreserveCommand _command;
        private Tag _tag1;
        private Tag _tag2;
        private TagRequirement _req1OnTag1WithTwoWeekInterval;
        private TagRequirement _req2OnTag1WithFourWeekInterval;
        private TagRequirement _req1OnTag2WithTwoWeekInterval;
        private TagRequirement _req2OnTag2WithFourWeekInterval;

        private BulkPreserveCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var mode = new Mode(TestPlant, "SUP", true);
            mode.SetProtectedIdForTesting(ModeId);
            var step = new Step(TestPlant, "SUP", mode, new Responsible(TestPlant, "C", "D"));
            step.SetProtectedIdForTesting(StepId);
            var rdMock = new Mock<RequirementDefinition>();
            rdMock.SetupGet(rd => rd.Plant).Returns(TestPlant);

            _req1OnTag1WithTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, rdMock.Object);
            _req2OnTag1WithFourWeekInterval = new TagRequirement(TestPlant, FourWeeksInterval, rdMock.Object);
            _req1OnTag2WithTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, rdMock.Object);
            _req2OnTag2WithFourWeekInterval = new TagRequirement(TestPlant, FourWeeksInterval, rdMock.Object);
            _tag1 = new Tag(TestPlant, TagType.Standard, "", "", step, new List<TagRequirement>
            {
                _req1OnTag1WithTwoWeekInterval, _req2OnTag1WithFourWeekInterval
            });
            _tag2 = new Tag(TestPlant, TagType.Standard, "", "", step, new List<TagRequirement>
            {
                _req1OnTag2WithTwoWeekInterval, _req2OnTag2WithFourWeekInterval
            });
            var tags = new List<Tag>
            {
                _tag1, _tag2
            };
            var tagIds = new List<int> {TagId1, TagId2};
            _projectRepoMock = new Mock<IProjectRepository>();
            _projectRepoMock.Setup(r => r.GetTagsByTagIdsAsync(tagIds)).Returns(Task.FromResult(tags));
            _personRepoMock = new Mock<IPersonRepository>();
            _personRepoMock
                .Setup(p => p.GetByOidAsync(It.Is<Guid>(x => x == CurrentUserOid)))
                .Returns(Task.FromResult(new Person(CurrentUserOid, "Test", "User")));
            _command = new BulkPreserveCommand(tagIds);

            _tag1.StartPreservation();
            _tag2.StartPreservation();

            _dut = new BulkPreserveCommandHandler(
                _projectRepoMock.Object,
                _personRepoMock.Object,
                UnitOfWorkMock.Object,
                CurrentUserProviderMock.Object);
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

            TimeService.SetConstant(TimeService.Now.AddWeeks(TwoWeeksInterval));
            await _dut.Handle(_command, default);

            var expectedNextDueTimeUtcForTwoWeeksInterval = TimeService.Now.AddWeeks(TwoWeeksInterval);
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
            TimeService.SetConstant(TimeService.Now.AddWeeks(FourWeeksInterval));

            await _dut.Handle(_command, default);

            var expectedNextDueTimeUtcForTwoWeeksInterval = TimeService.Now.AddWeeks(TwoWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeUtcForTwoWeeksInterval, _req1OnTag1WithTwoWeekInterval.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtcForTwoWeeksInterval, _req1OnTag2WithTwoWeekInterval.NextDueTimeUtc);

            var expectedNextDueTimeUtcForFourWeeksInterval = TimeService.Now.AddWeeks(FourWeeksInterval);
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
            TimeService.SetConstant(TimeService.Now.AddWeeks(TwoWeeksInterval));
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingBulkPreserveCommand_ShouldSave_WhenOnDueForLastRequirement()
        {
            TimeService.SetConstant(TimeService.Now.AddWeeks(FourWeeksInterval));
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

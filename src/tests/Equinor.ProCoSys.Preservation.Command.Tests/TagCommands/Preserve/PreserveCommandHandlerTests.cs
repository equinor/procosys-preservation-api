using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.TagCommands.Preserve;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.Preserve
{
    [TestClass]
    public class PreserveCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int SupplierModeId = 1;
        private const int OtherModeId = 2;
        private const int TagWithForAllRequirementsId = 7;
        private const int TagInSupplierStepId = 8;
        private const int TagInOtherStepId = 9;
        private const int SupplierStepId = 100;
        private const int OtherStepId = 101;
        private const int TwoWeeksInterval = 2;
        private const int FourWeeksInterval = 4;

        private PreserveCommand _commandForTagWithForAllRequirements;
        private PreserveCommand _commandForTagInSupplierStep;
        private PreserveCommand _commandForTagInOtherStep;
        private Tag _tagWithForAllRequirements;
        private Tag _tagWithSupplierAndOtherRequirementsInSupplierStep;
        private Tag _tagWithSupplierAndOtherRequirementsInOtherStep;
        private TagRequirement _req1ForAllWithTwoWeekInterval;
        private TagRequirement _req2ForAllWithTwoWeekInterval;
        private TagRequirement _req3ForAllWithFourWeekInterval;
        private TagRequirement _reqForSupplierInSupplierStep;
        private TagRequirement _reqForSupplierInOtherStep;
        private TagRequirement _reqForOtherInSupplierStep;
        private TagRequirement _reqForOtherInOtherStep;

        private PreserveCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var supplierMode = new Mode(TestPlant, "SUP", true);
            supplierMode.SetProtectedIdForTesting(SupplierModeId);
            var otherMode = new Mode(TestPlant, "HOOKUP", false);
            otherMode.SetProtectedIdForTesting(OtherModeId);

            var responsible = new Responsible(TestPlant, "C", "D");
            var supplierStep = new Step(TestPlant, "SUP", supplierMode, responsible);
            supplierStep.SetProtectedIdForTesting(SupplierStepId);
            var otherStep = new Step(TestPlant, "HOOKUP", otherMode, responsible);
            otherStep.SetProtectedIdForTesting(OtherStepId);

            var tmp = new Mock<RequirementDefinition>(TestPlant, "ForAll", TwoWeeksInterval, RequirementUsage.ForAll, 1);

            var rd1ForAllTwoWeekInterval = MockRequirementDefinition(TestPlant, "ForAll", TwoWeeksInterval, RequirementUsage.ForAll, 1);
            var rd2ForAllTwoWeekInterval = MockRequirementDefinition(TestPlant, "ForAll", TwoWeeksInterval, RequirementUsage.ForAll, 2);
            var rd3ForAllTwoWeekInterval = MockRequirementDefinition(TestPlant, "ForAll", TwoWeeksInterval, RequirementUsage.ForAll, 3);
            var rdForSupplierTwoWeekInterval = MockRequirementDefinition(TestPlant, "ForSup", TwoWeeksInterval, RequirementUsage.ForSuppliersOnly, 4);
            var rdForOtherTwoWeekInterval = MockRequirementDefinition(TestPlant, "ForOther", TwoWeeksInterval, RequirementUsage.ForOtherThanSuppliers, 5);

            _req1ForAllWithTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, rd1ForAllTwoWeekInterval);
            _req2ForAllWithTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, rd2ForAllTwoWeekInterval);
            _req3ForAllWithFourWeekInterval = new TagRequirement(TestPlant, FourWeeksInterval, rd3ForAllTwoWeekInterval);

            _tagWithForAllRequirements = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "", "", supplierStep, new List<TagRequirement>
            {
                _req1ForAllWithTwoWeekInterval,
                _req2ForAllWithTwoWeekInterval,
                _req3ForAllWithFourWeekInterval
            });

            _reqForSupplierInSupplierStep = new TagRequirement(TestPlant, TwoWeeksInterval, rdForSupplierTwoWeekInterval);
            _reqForOtherInSupplierStep = new TagRequirement(TestPlant, TwoWeeksInterval, rdForOtherTwoWeekInterval);
            _tagWithSupplierAndOtherRequirementsInSupplierStep = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "", "", supplierStep, new List<TagRequirement>
            {
                _reqForSupplierInSupplierStep,
                _reqForOtherInSupplierStep
            });

            _reqForSupplierInOtherStep = new TagRequirement(TestPlant, TwoWeeksInterval, rdForSupplierTwoWeekInterval);
            _reqForOtherInOtherStep = new TagRequirement(TestPlant, TwoWeeksInterval, rdForOtherTwoWeekInterval);
            _tagWithSupplierAndOtherRequirementsInOtherStep = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "", "", otherStep, new List<TagRequirement>
            {
                _reqForSupplierInOtherStep,
                _reqForOtherInOtherStep
            });

            var projectRepoMock = new Mock<IProjectRepository>();
            projectRepoMock.Setup(r => r.GetTagWithPreservationHistoryByTagIdAsync(TagWithForAllRequirementsId))
                .Returns(Task.FromResult(_tagWithForAllRequirements));
            projectRepoMock.Setup(r => r.GetTagWithPreservationHistoryByTagIdAsync(TagInOtherStepId))
                .Returns(Task.FromResult(_tagWithSupplierAndOtherRequirementsInOtherStep));
            projectRepoMock.Setup(r => r.GetTagWithPreservationHistoryByTagIdAsync(TagInSupplierStepId))
                .Returns(Task.FromResult(_tagWithSupplierAndOtherRequirementsInSupplierStep));

            var personRepoMock = new Mock<IPersonRepository>();
            personRepoMock
                .Setup(p => p.GetByOidAsync(It.Is<Guid>(x => x == CurrentUserOid)))
                .Returns(Task.FromResult(new Person(CurrentUserOid, "Test", "User")));
            _commandForTagWithForAllRequirements = new PreserveCommand(TagWithForAllRequirementsId);
            _commandForTagInOtherStep = new PreserveCommand(TagInOtherStepId);
            _commandForTagInSupplierStep = new PreserveCommand(TagInSupplierStepId);

            _tagWithForAllRequirements.StartPreservation();
            _tagWithSupplierAndOtherRequirementsInOtherStep.StartPreservation();
            _tagWithSupplierAndOtherRequirementsInSupplierStep.StartPreservation();

            _dut = new PreserveCommandHandler(
                projectRepoMock.Object,
                personRepoMock.Object,
                UnitOfWorkMock.Object,
                CurrentUserProviderMock.Object);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldPreserveRequirementsOnTag_IsDue()
        {
            var req1WithTwoWeekIntervalInitialPeriod = _req1ForAllWithTwoWeekInterval.ActivePeriod;
            var req2WithTwoWeekIntervalInitialPeriod = _req2ForAllWithTwoWeekInterval.ActivePeriod;
            var req3WithFourWeekIntervalInitialPeriod = _req3ForAllWithFourWeekInterval.ActivePeriod;

            TimeProvider.ElapseWeeks(TwoWeeksInterval);

            await _dut.Handle(_commandForTagWithForAllRequirements, default);

            var expectedNextDueTimeUtc = TimeProvider.UtcNow.AddWeeks(TwoWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeUtc, _req1ForAllWithTwoWeekInterval.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtc, _tagWithForAllRequirements.NextDueTimeUtc);
            Assert.IsNotNull(req1WithTwoWeekIntervalInitialPeriod.PreservationRecord);
            Assert.AreEqual(expectedNextDueTimeUtc, _req2ForAllWithTwoWeekInterval.NextDueTimeUtc);
            Assert.IsNotNull(req2WithTwoWeekIntervalInitialPeriod.PreservationRecord);
            Assert.IsNull(req3WithFourWeekIntervalInitialPeriod.PreservationRecord);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldPreserveRequirementsForSupplier_WhenTagIsInSupplierStep()
        {
            var reqForSupplierInSupplierStepPeriod = _reqForSupplierInSupplierStep.ActivePeriod;
            var reqForOtherInSupplierStepPeriod = _reqForOtherInSupplierStep.ActivePeriod;

            TimeProvider.ElapseWeeks(TwoWeeksInterval);

            await _dut.Handle(_commandForTagInSupplierStep, default);

            Assert.IsNotNull(reqForSupplierInSupplierStepPeriod.PreservationRecord);
            Assert.IsNull(reqForOtherInSupplierStepPeriod.PreservationRecord);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldNotPreserveRequirementsForSupplier_WhenTagIsInOtherStep()
        {
            var reqForOtherInOtherStepPeriod = _reqForOtherInOtherStep.ActivePeriod;
            var reqForSupplierInOtherStepPeriod = _reqForSupplierInOtherStep.ActivePeriod;

            TimeProvider.ElapseWeeks(TwoWeeksInterval);

            await _dut.Handle(_commandForTagInOtherStep, default);

            Assert.IsNotNull(reqForOtherInOtherStepPeriod.PreservationRecord);
            Assert.IsNull(reqForSupplierInOtherStepPeriod.PreservationRecord);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldSkipPreservingRequirementsOnTag_NotDue()
        {
            TimeProvider.ElapseWeeks(TwoWeeksInterval);
            var oldNextDue = _req3ForAllWithFourWeekInterval.NextDueTimeUtc;

            await _dut.Handle(_commandForTagWithForAllRequirements, default);

            Assert.AreEqual(oldNextDue, _req3ForAllWithFourWeekInterval.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldSave_WhenOnDueForFirstRequirement()
        {
            TimeProvider.ElapseWeeks(TwoWeeksInterval);
            await _dut.Handle(_commandForTagWithForAllRequirements, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldSave_WhenOnDueForLastRequirement()
        {
            TimeProvider.ElapseWeeks(FourWeeksInterval);
            await _dut.Handle(_commandForTagWithForAllRequirements, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldNotSave_WhenBeforeDueForAnyRequirement()
        {
            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _dut.Handle(_commandForTagWithForAllRequirements, default)
            );

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Never);
        }

        private RequirementDefinition MockRequirementDefinition(string plant, string title, int defaultIntervalWeeks, RequirementUsage usage, int sortKey)
        {
            var requirementDefinition = new Mock<RequirementDefinition>(plant, title, defaultIntervalWeeks, usage, sortKey);
            requirementDefinition.CallBase = true;
            requirementDefinition.SetupGet(rd => rd.Id).Returns(sortKey);

            return requirementDefinition.Object;
        }
    }
}

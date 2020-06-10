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
        private const int SupplierModeId = 1;
        private const int OtherModeId = 2;
        private const int TagWithForAllRequirementsId = 7;
        private const int TagInSupplierStepId = 8;
        private const int TagInOtherStepId = 9;
        private const int SupplierStepId = 100;
        private const int OtherStepId = 101;
        private const int TwoWeeksInterval = 2;
        private const int FourWeeksInterval = 4;

        private readonly Guid _currentUserOid = new Guid("12345678-1234-1234-1234-123456789123");
        private Mock<IProjectRepository> _projectRepoMock;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Mock<IModeRepository> _modeRepositoryMock;
        private Mock<IPersonRepository> _personRepoMock;
        private Mock<ICurrentUserProvider> _currentUserProvider;
        private PreserveCommand _commandForTagWithForAllRequirements;
        private PreserveCommand _commandForTagInSupplierStep;
        private PreserveCommand _commandForTagInOtherStep;
        private Tag _tagWithForAllRequirements;
        private Tag _tagWithSupplierAndOtherRequirementsInSupplierStep;
        private Tag _tagWithSupplierAndOtherRequirementsInOtherStep;
        private TagRequirement _req1ForAllWithTwoWeekInterval;
        private TagRequirement _req2ForAllWithTwoWeekInterval;
        private TagRequirement _req3ForAllWithFourWeekInterval;
        private TagRequirement _req1ForSupplierWithTwoWeekInterval;
        private TagRequirement _req2ForSupplierWithTwoWeekInterval;
        private TagRequirement _req1ForOtherWithTwoWeekInterval;
        private TagRequirement _req2ForOtherWithTwoWeekInterval;

        private PreserveCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var supplierMode = new Mode(TestPlant, "SUP", true);
            supplierMode.SetProtectedIdForTesting(SupplierModeId);
            var otherMode = new Mode(TestPlant, "HOOKUP", false);
            otherMode.SetProtectedIdForTesting(OtherModeId);

            var responsible = new Responsible(TestPlant, "C", "T");
            var supplierStep = new Step(TestPlant, "SUP", supplierMode, responsible);
            supplierStep.SetProtectedIdForTesting(SupplierStepId);
            var otherStep = new Step(TestPlant, "HOOKUP", otherMode, responsible);
            otherStep.SetProtectedIdForTesting(OtherStepId);

            var rdForAllTwoWeekInterval = new RequirementDefinition(TestPlant, "ForAll", TwoWeeksInterval, RequirementUsage.ForAll, 1);
            var rdForSupplierTwoWeekInterval = new RequirementDefinition(TestPlant, "ForSup", TwoWeeksInterval, RequirementUsage.ForSuppliersOnly, 2);
            var rdForOtherTwoWeekInterval = new RequirementDefinition(TestPlant, "ForOther", TwoWeeksInterval, RequirementUsage.ForOtherThanSuppliers, 3);

            _req1ForAllWithTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, rdForAllTwoWeekInterval);
            _req2ForAllWithTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, rdForAllTwoWeekInterval);
            _req3ForAllWithFourWeekInterval = new TagRequirement(TestPlant, FourWeeksInterval, rdForAllTwoWeekInterval);
            
            _tagWithForAllRequirements = new Tag(TestPlant, TagType.Standard, "", "", supplierStep, new List<TagRequirement>
            {
                _req1ForAllWithTwoWeekInterval, 
                _req2ForAllWithTwoWeekInterval,
                _req3ForAllWithFourWeekInterval
            });

            _req1ForSupplierWithTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, rdForSupplierTwoWeekInterval);
            _req1ForOtherWithTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, rdForOtherTwoWeekInterval);
            _tagWithSupplierAndOtherRequirementsInSupplierStep = new Tag(TestPlant, TagType.Standard, "", "", supplierStep, new List<TagRequirement>
            {
                _req1ForSupplierWithTwoWeekInterval, 
                _req1ForOtherWithTwoWeekInterval
            });

            _req2ForSupplierWithTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, rdForSupplierTwoWeekInterval);
            _req2ForOtherWithTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, rdForOtherTwoWeekInterval);
            _tagWithSupplierAndOtherRequirementsInOtherStep = new Tag(TestPlant, TagType.Standard, "", "", otherStep, new List<TagRequirement>
            {
                _req2ForSupplierWithTwoWeekInterval, 
                _req2ForOtherWithTwoWeekInterval
            });

            _currentUserProvider = new Mock<ICurrentUserProvider>();
            _currentUserProvider
                .Setup(x => x.GetCurrentUserOid())
                .Returns(_currentUserOid);
            _projectRepoMock = new Mock<IProjectRepository>();
            _projectRepoMock.Setup(r => r.GetTagByTagIdAsync(TagWithForAllRequirementsId)).Returns(Task.FromResult(_tagWithForAllRequirements));
            _projectRepoMock.Setup(r => r.GetTagByTagIdAsync(TagInOtherStepId)).Returns(Task.FromResult(_tagWithSupplierAndOtherRequirementsInOtherStep));
            _projectRepoMock.Setup(r => r.GetTagByTagIdAsync(TagInSupplierStepId)).Returns(Task.FromResult(_tagWithSupplierAndOtherRequirementsInSupplierStep));
            
            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journeyRepositoryMock.Setup(j => j.GetStepByStepIdAsync(SupplierStepId)).Returns(Task.FromResult(supplierStep));
            _journeyRepositoryMock.Setup(j => j.GetStepByStepIdAsync(OtherStepId)).Returns(Task.FromResult(otherStep));
            _modeRepositoryMock = new Mock<IModeRepository>();
            _modeRepositoryMock.Setup(m => m.GetByIdAsync(SupplierModeId)).Returns(Task.FromResult(supplierMode));
            _modeRepositoryMock.Setup(m => m.GetByIdAsync(OtherModeId)).Returns(Task.FromResult(otherMode));
            
            _personRepoMock = new Mock<IPersonRepository>();
            _personRepoMock
                .Setup(p => p.GetByOidAsync(It.Is<Guid>(x => x == _currentUserOid)))
                .Returns(Task.FromResult(new Person(_currentUserOid, "Test", "User")));
            _commandForTagWithForAllRequirements = new PreserveCommand(TagWithForAllRequirementsId);
            _commandForTagInOtherStep = new PreserveCommand(TagInOtherStepId);
            _commandForTagInSupplierStep = new PreserveCommand(TagInSupplierStepId);

            _tagWithForAllRequirements.StartPreservation();
            _tagWithSupplierAndOtherRequirementsInOtherStep.StartPreservation();
            _tagWithSupplierAndOtherRequirementsInSupplierStep.StartPreservation();

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
            var req1WithTwoWeekIntervalInitialPeriod = _req1ForAllWithTwoWeekInterval.ActivePeriod;
            var req2WithTwoWeekIntervalInitialPeriod = _req2ForAllWithTwoWeekInterval.ActivePeriod;
            var req3WithFourWeekIntervalInitialPeriod = _req3ForAllWithFourWeekInterval.ActivePeriod;

            _timeProvider.ElapseWeeks(TwoWeeksInterval);

            await _dut.Handle(_commandForTagWithForAllRequirements, default);

            var expectedNextDueTimeUtc = _timeProvider.UtcNow.AddWeeks(TwoWeeksInterval);
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
            _timeProvider.ElapseWeeks(TwoWeeksInterval);

            await _dut.Handle(_commandForTagInSupplierStep, default);

            Assert.IsNull(_req1ForOtherWithTwoWeekInterval.ActivePeriod.PreservationRecord);
            Assert.IsNotNull(_req1ForSupplierWithTwoWeekInterval.ActivePeriod.PreservationRecord);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldNotPreserveRequirementsForSupplier_WhenTagIsInOtherStep()
        {
            _timeProvider.ElapseWeeks(TwoWeeksInterval);

            await _dut.Handle(_commandForTagInOtherStep, default);

            Assert.IsNull(_req1ForOtherWithTwoWeekInterval.ActivePeriod.PreservationRecord);
            Assert.IsNotNull(_req1ForSupplierWithTwoWeekInterval.ActivePeriod.PreservationRecord);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldSkipPreservingRequirementsOnTag_NotDue()
        {
            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            var oldNextDue = _req3ForAllWithFourWeekInterval.NextDueTimeUtc;

            await _dut.Handle(_commandForTagWithForAllRequirements, default);

            Assert.AreEqual(oldNextDue, _req3ForAllWithFourWeekInterval.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldSave_WhenOnDueForFirstRequirement()
        {
            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            await _dut.Handle(_commandForTagWithForAllRequirements, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingPreserveCommand_ShouldSave_WhenOnDueForLastRequirement()
        {
            _timeProvider.ElapseWeeks(FourWeeksInterval);
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
    }
}

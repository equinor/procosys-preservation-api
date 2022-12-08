using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.Domain.Time;
using Equinor.ProCoSys.Preservation.Test.Common;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class TagTests
    {
        #region Setup

        private const string TestPlant = "PlantA";
        private const int TwoWeeksInterval = 2;
        private const int ThreeWeeksInterval = 3;
        private Person _person;
        private Step _supplierStep;
        private Step _otherStep;
        private Step _lastStep;

        private RequirementDefinition _reqDef1NotNeedInput;
        private RequirementDefinition _reqDef2NotNeedInput;
        private RequirementDefinition _reqDef1NeedInput;
        private RequirementDefinition _reqDef2NeedInput;
        private RequirementDefinition _reqDefNotNeedInputForSupplier;
        private RequirementDefinition _reqDefNotNeedInputForOther;
        private TagRequirement _reqNotNeedInputForAllTwoWeekInterval;
        private TagRequirement _reqNotNeedInputForSupplierTwoWeekInterval;
        private TagRequirement _reqNotNeedInputForOtherTwoWeekInterval;
        private TagRequirement _reqNotNeedInputForAllThreeWeekInterval;
        private TagRequirement _reqNeedInputTwoWeekInterval;
        private TagRequirement _reqNeedInputThreeWeekInterval;
        private List<TagRequirement> _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther;
        private List<TagRequirement> _oneReq_NotNeedInputTwoWeekInterval;
        private List<TagRequirement> _oneReq_NeedInputTwoWeekInterval;
        private List<TagRequirement> _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval;
        private List<TagRequirement> _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval;
        private List<TagRequirement> _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval;

        private ManualTimeProvider _timeProvider;
        private DateTime _utcNow;
        private Tag _dutWithOneReqNotNeedInputTwoWeekInterval;
        private Journey _journey;
        private int _reqDefNotNeedInputForSupplierId;
        private int _reqDefNotNeedInputForOtherId;
        private int _nextStepId;
        private Responsible _responsible;
        private Mode _mode;
        private Guid _testGuid;

        [TestInitialize]
        public void Setup()
        {
            _responsible = new Responsible(TestPlant, "RC", "RD");
            _supplierStep = new Step(TestPlant, "SUP", new Mode(TestPlant, "SUP", true), _responsible);
            _nextStepId = 1;
            _supplierStep.SetProtectedIdForTesting(++_nextStepId);
            _mode = new Mode(TestPlant, "O", false);
            _otherStep = new Step(TestPlant, "OTHER1", _mode, _responsible)
            {
                AutoTransferMethod = AutoTransferMethod.OnRfccSign
            };
            _otherStep.SetProtectedIdForTesting(++_nextStepId);

            _person = new Person(Guid.Empty, "Espen", "Askeladd");
            _person.SetProtectedIdForTesting(12);

            _journey = new Journey(TestPlant, "J");
            _journey.AddStep(_supplierStep);
            _journey.AddStep(_otherStep);
            _lastStep = _otherStep;

            var reqDefId = 100;

            _reqDef1NotNeedInput = new RequirementDefinition(TestPlant, "ALL1", 2, RequirementUsage.ForAll, 0);
            _reqDef1NotNeedInput.SetProtectedIdForTesting(++reqDefId);
            _reqDef1NotNeedInput.AddField(new Field(TestPlant, "", FieldType.Info, 0));

            _reqDefNotNeedInputForSupplier = new RequirementDefinition(TestPlant, "SUP", 2, RequirementUsage.ForSuppliersOnly, 0);
            _reqDefNotNeedInputForSupplier.SetProtectedIdForTesting(++reqDefId);
            _reqDefNotNeedInputForSupplierId = _reqDefNotNeedInputForSupplier.Id;
            _reqDefNotNeedInputForOther = new RequirementDefinition(TestPlant, "OTHER", 2, RequirementUsage.ForOtherThanSuppliers, 0);
            _reqDefNotNeedInputForOther.SetProtectedIdForTesting(++reqDefId);
            _reqDefNotNeedInputForOtherId = _reqDefNotNeedInputForOther.Id;

            _reqDef2NotNeedInput = new RequirementDefinition(TestPlant, "All2", 2, RequirementUsage.ForAll, 0);
            _reqDef2NotNeedInput.SetProtectedIdForTesting(++reqDefId);
            _reqDef2NotNeedInput.AddField(new Field(TestPlant, "", FieldType.Info, 0));
            
            _reqDef1NeedInput = new RequirementDefinition(TestPlant, "", 1, RequirementUsage.ForAll, 0);
            _reqDef1NeedInput.SetProtectedIdForTesting(++reqDefId);
            _reqDef1NeedInput.AddField(new Field(TestPlant, "", FieldType.CheckBox, 0));
            
            _reqDef2NeedInput = new RequirementDefinition(TestPlant, "", 1, RequirementUsage.ForAll, 0);
            _reqDef2NeedInput.SetProtectedIdForTesting(++reqDefId);
            _reqDef2NeedInput.AddField(new Field(TestPlant, "", FieldType.CheckBox, 0));

            var reqId = 510;
            _reqNotNeedInputForAllTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, _reqDef1NotNeedInput);
            _reqNotNeedInputForAllTwoWeekInterval.SetProtectedIdForTesting(++reqId);
            _reqNotNeedInputForSupplierTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, _reqDefNotNeedInputForSupplier);
            _reqNotNeedInputForSupplierTwoWeekInterval.SetProtectedIdForTesting(++reqId);
            _reqNotNeedInputForOtherTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, _reqDefNotNeedInputForOther);
            _reqNotNeedInputForOtherTwoWeekInterval.SetProtectedIdForTesting(++reqId);
            _reqNotNeedInputForAllThreeWeekInterval = new TagRequirement(TestPlant, ThreeWeeksInterval, _reqDef2NotNeedInput);
            _reqNotNeedInputForAllThreeWeekInterval.SetProtectedIdForTesting(++reqId);
            _reqNeedInputTwoWeekInterval = new TagRequirement(TestPlant, TwoWeeksInterval, _reqDef1NeedInput);
            _reqNeedInputTwoWeekInterval.SetProtectedIdForTesting(++reqId);
            _reqNeedInputThreeWeekInterval = new TagRequirement(TestPlant, ThreeWeeksInterval, _reqDef2NeedInput);
            _reqNeedInputThreeWeekInterval.SetProtectedIdForTesting(++reqId);

            _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther = new List<TagRequirement>
            {
                _reqNotNeedInputForAllTwoWeekInterval,
                _reqNotNeedInputForAllThreeWeekInterval,
                _reqNotNeedInputForSupplierTwoWeekInterval,
                _reqNotNeedInputForOtherTwoWeekInterval
            };

            _oneReq_NotNeedInputTwoWeekInterval = new List<TagRequirement>
            {
                _reqNotNeedInputForAllTwoWeekInterval
            };

            _oneReq_NeedInputTwoWeekInterval = new List<TagRequirement>
            {
                _reqNeedInputTwoWeekInterval
            };

            _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval = new List<TagRequirement>
            {
                _reqNeedInputTwoWeekInterval, _reqNotNeedInputForAllThreeWeekInterval
            };

            _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval = new List<TagRequirement>
            {
                _reqNotNeedInputForAllTwoWeekInterval, _reqNeedInputThreeWeekInterval
            };

            _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval = new List<TagRequirement>
            {
                _reqNotNeedInputForAllTwoWeekInterval, _reqNotNeedInputForAllThreeWeekInterval
            };

            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _timeProvider = new ManualTimeProvider(_utcNow);
            TimeService.SetProvider(_timeProvider);
            
            _testGuid = Guid.NewGuid();
            _dutWithOneReqNotNeedInputTwoWeekInterval = new Tag(
                TestPlant,
                TagType.Standard,
                _testGuid,
                "TagNoA",
                "DescA", 
                _supplierStep,
                _oneReq_NotNeedInputTwoWeekInterval);
        }
        #endregion

        #region Constructor

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual(TestPlant, _dutWithOneReqNotNeedInputTwoWeekInterval.Plant);
            Assert.AreEqual("TagNoA", _dutWithOneReqNotNeedInputTwoWeekInterval.TagNo);
            Assert.AreEqual("DescA", _dutWithOneReqNotNeedInputTwoWeekInterval.Description);
            Assert.AreEqual(_supplierStep.Id, _dutWithOneReqNotNeedInputTwoWeekInterval.StepId);
            Assert.AreEqual(_testGuid, _dutWithOneReqNotNeedInputTwoWeekInterval.ProCoSysGuid);
            Assert.AreEqual(TagType.Standard, _dutWithOneReqNotNeedInputTwoWeekInterval.TagType);
            Assert.IsNull(_dutWithOneReqNotNeedInputTwoWeekInterval.NextDueTimeUtc);
            var requirements = _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements;
            Assert.AreEqual(1, requirements.Count);
            
            var req = _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.ElementAt(0);
            Assert.IsNull(req.NextDueTimeUtc);
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
        }

        [TestMethod]
        public void Constructor_ShouldAddTagCreatedEvent()
        {
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Last(), typeof(TagCreatedEvent));
        }
        
        [TestMethod]
        public void Constructor_ShouldNotSetReadyToBePreserved_AtAnyTime()
        {
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved());
            
            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved());

            _timeProvider.SetTime(_utcNow);
            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved());
        }
        
        [TestMethod]
        public void Constructor_ShouldNotSetReadyToBeBeRescheduled_AtAnyTime()
        {
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBeRescheduled());
            
            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBeRescheduled());

            _timeProvider.SetTime(_utcNow);
            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBeRescheduled());
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenStepNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(()
                => new Tag(TestPlant, TagType.Standard, _testGuid, "", "", null, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementsNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(()
                => new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, null));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenProCoSysGuidNotGiven_ForStandardTag()
            => Assert.ThrowsException<ArgumentException>(()
                => new Tag(TestPlant, TagType.Standard, null, "", "", _supplierStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenProCoSysGuidIsEmpty_ForStandardTag()
            => Assert.ThrowsException<ArgumentException>(()
                => new Tag(TestPlant, TagType.Standard, Guid.Empty, "", "", _supplierStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenProCoSysGuidGiven_ForPreAreaTag()
            => Assert.ThrowsException<ArgumentException>(()
                => new Tag(TestPlant, TagType.PreArea, _testGuid, "", "", _supplierStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenProCoSysGuidGiven_ForPoAreaTag()
            => Assert.ThrowsException<ArgumentException>(()
                => new Tag(TestPlant, TagType.PoArea, _testGuid, "", "", _supplierStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenProCoSysGuidGiven_ForSiteAreTag()
            => Assert.ThrowsException<ArgumentException>(()
                => new Tag(TestPlant, TagType.SiteArea, _testGuid, "", "", _supplierStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenEmptyListOfRequirementsGiven()
            => Assert.ThrowsException<Exception>(()
                => new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, new List<TagRequirement>()));

        #endregion

        #region SetStep

        [TestMethod]
        public void SetStep_ShouldSetStepIdAndIsSupplierStep()
        {
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsInSupplierStep);
            var newStep = new Mock<Step>();
            newStep.SetupGet(x => x.Id).Returns(4);
            _dutWithOneReqNotNeedInputTwoWeekInterval.SetStep(newStep.Object);

            Assert.AreEqual(newStep.Object.Id, _dutWithOneReqNotNeedInputTwoWeekInterval.StepId);
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsInSupplierStep);
        }

        [TestMethod]
        public void SetStep_ShouldThrowException_WhenStepNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dutWithOneReqNotNeedInputTwoWeekInterval.SetStep(null));

        #endregion

        #region AddRequirement

        [TestMethod]
        public void AddRequirement_ShouldAddRequirement()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddRequirement(_reqNeedInputThreeWeekInterval);

            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.Count);
            Assert.AreEqual(_reqNeedInputThreeWeekInterval, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.Last());
        }

        [TestMethod]
        public void AddRequirement_ShouldStartPreservation_OnAddedRequirement_WhenPreservationStarted()
        {
            // Arrange
            Assert.IsFalse(_reqNeedInputThreeWeekInterval.HasActivePeriod);
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            
            // Act
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddRequirement(_reqNeedInputThreeWeekInterval);

            // Assert
            Assert.IsTrue(_reqNeedInputThreeWeekInterval.HasActivePeriod);
        }

        [TestMethod]
        public void AddRequirement_ShouldNotStartPreservation_OnAddedRequirement_WhenPreservationNotStarted()
        {
            // Arrange
            Assert.IsFalse(_reqNeedInputThreeWeekInterval.HasActivePeriod);
            
            // Act
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddRequirement(_reqNeedInputThreeWeekInterval);

            // Assert
            Assert.IsFalse(_reqNeedInputThreeWeekInterval.HasActivePeriod);
        }

        [TestMethod]
        public void AddRequirement_ShouldThrowException_WhenRequirementNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dutWithOneReqNotNeedInputTwoWeekInterval.AddRequirement(null));

        [TestMethod]
        public void AddRequirement_ShouldThrowException_WhenRequirementWithSameDefinitionExists()
            => Assert.ThrowsException<ArgumentException>(() =>
                _dutWithOneReqNotNeedInputTwoWeekInterval.AddRequirement(_dutWithOneReqNotNeedInputTwoWeekInterval
                    .Requirements.First()));

        [TestMethod]
        public void AddRequirement_ShouldAddRequirementAddedEvent()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddRequirement(_reqNeedInputThreeWeekInterval);

            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Last(), typeof(TagRequirementAddedEvent));
        }

        #endregion

        #region DeleteRequirement

        [TestMethod]
        public void DeleteRequirement_ShouldDeleteRequirement()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            Assert.AreEqual(2, dut.Requirements.Count);
            var requirement = dut.Requirements.Last();
            dut.UpdateRequirement(requirement.Id, true, requirement.IntervalWeeks, "AAAAAAAAABA=");

            // Act
            dut.RemoveRequirement(requirement.Id, "AAAAAAAAABA=");

            // Assert
            Assert.AreEqual(1, dut.Requirements.Count);
        }

        [TestMethod]
        public void DeleteRequirement_ShouldAddRequirementDeletedEvent()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            var requirement = dut.Requirements.Last();
            dut.UpdateRequirement(requirement.Id, true, requirement.IntervalWeeks, "AAAAAAAAABA=");

            // Act
            dut.RemoveRequirement(requirement.Id, "AAAAAAAAABA=");

            Assert.AreEqual(3, dut.DomainEvents.Count);
            Assert.IsInstanceOfType(dut.DomainEvents.Last(), typeof(TagRequirementDeletedEvent));
        }

        [TestMethod]
        public void DeleteRequirement_ShouldThrowException_WhenPreservationStarted()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();
            var requirement = dut.Requirements.Last();
            dut.UpdateRequirement(requirement.Id, true, requirement.IntervalWeeks, "AAAAAAAAABA=");

            // Act and Assert
            Assert.ThrowsException<Exception>(() => dut.RemoveRequirement(requirement.Id, "AAAAAAAAABA="));
        }

        [TestMethod]
        public void DeleteRequirement_ShouldThrowException_WhenNotVoided()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            var requirement = dut.Requirements.Last();

            // Act and Assert
            Assert.ThrowsException<Exception>(() => dut.RemoveRequirement(requirement.Id, "AAAAAAAAABA="));
        }

        #endregion

        #region StartPreservation

        [TestMethod]
        public void StartPreservation_FromNotStarted_ShouldSetStatusActive()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);

            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();

            Assert.AreEqual(PreservationStatus.Active, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
        }

        [TestMethod]
        public void StartPreservation_FromNotStarted_ShouldSetCorrectNextDueDateOnTagAndEachRequirement()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);

            dut.StartPreservation();

            var expectedNextDueTimeFirstUtc = _utcNow.AddWeeks(TwoWeeksInterval);
            var expectedNextDueTimeLaterUtc = _utcNow.AddWeeks(ThreeWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeFirstUtc, dut.Requirements.ElementAt(0).NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeLaterUtc, dut.Requirements.ElementAt(1).NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeFirstUtc, dut.NextDueTimeUtc);
        }

        [TestMethod]
        public void StartPreservation_FromNotStarted_ShouldStartOnEachNonVoidedRequirement()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);
            dut.Requirements.ElementAt(0).IsVoided = true;

            dut.StartPreservation();

            var expectedNextDueTime1 = _utcNow.AddWeeks(dut.Requirements.ElementAt(1).IntervalWeeks);
            var expectedNextDueTime2 = _utcNow.AddWeeks(dut.Requirements.ElementAt(2).IntervalWeeks);
            var expectedNextDueTime3 = _utcNow.AddWeeks(dut.Requirements.ElementAt(3).IntervalWeeks);
            Assert.AreEqual(expectedNextDueTime1, dut.Requirements.ElementAt(1).NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTime2, dut.Requirements.ElementAt(2).NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTime3, dut.Requirements.ElementAt(3).NextDueTimeUtc);
            Assert.IsFalse(dut.Requirements.ElementAt(0).NextDueTimeUtc.HasValue);

            var expectedNextDueTime = _utcNow.AddWeeks(dut.OrderedRequirements().ElementAt(0).IntervalWeeks);
            Assert.AreEqual(expectedNextDueTime, dut.NextDueTimeUtc);
        }

        [TestMethod]
        public void StartPreservation_FromNotStarted_ShouldAddPreservationStartedEvent()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();

            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Last(), typeof(PreservationStartedEvent));
        }

        [TestMethod]
        public void StartPreservation_AfterComplete_ShouldSetStatusActive()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _lastStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);
            dut.StartPreservation();
            dut.CompletePreservation(_journey);
            Assert.AreEqual(PreservationStatus.Completed, dut.Status);

            // Act
            dut.StartPreservation();

            // Assert
            Assert.AreEqual(PreservationStatus.Active, dut.Status);
        }

        [TestMethod]
        public void StartPreservation_AfterComplete_ShouldSetCorrectNextDueDateOnTagAndEachRequirement()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _lastStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);

            dut.StartPreservation();
            dut.CompletePreservation(_journey);

            // Act
            dut.StartPreservation();

            // Assert
            var expectedNextDueTimeFirstUtc = _utcNow.AddWeeks(TwoWeeksInterval);
            var expectedNextDueTimeLaterUtc = _utcNow.AddWeeks(ThreeWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeFirstUtc, dut.Requirements.ElementAt(0).NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeLaterUtc, dut.Requirements.ElementAt(1).NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeFirstUtc, dut.NextDueTimeUtc);
        }

        [TestMethod]
        public void StartPreservation_AfterComplete_ShouldStartOnEachNonVoidedRequirement()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _lastStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);
            dut.Requirements.ElementAt(0).IsVoided = true;

            dut.StartPreservation();
            dut.CompletePreservation(_journey);

            // Act
            dut.StartPreservation();

            // Assert
            var expectedNextDueTime1 = _utcNow.AddWeeks(dut.Requirements.ElementAt(1).IntervalWeeks);
            var expectedNextDueTime2 = _utcNow.AddWeeks(dut.Requirements.ElementAt(2).IntervalWeeks);
            var expectedNextDueTime3 = _utcNow.AddWeeks(dut.Requirements.ElementAt(3).IntervalWeeks);
            Assert.AreEqual(expectedNextDueTime1, dut.Requirements.ElementAt(1).NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTime2, dut.Requirements.ElementAt(2).NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTime3, dut.Requirements.ElementAt(3).NextDueTimeUtc);
            Assert.IsFalse(dut.Requirements.ElementAt(0).NextDueTimeUtc.HasValue);

            var expectedNextDueTime = _utcNow.AddWeeks(dut.OrderedRequirements().ElementAt(0).IntervalWeeks);
            Assert.AreEqual(expectedNextDueTime, dut.NextDueTimeUtc);
        }

        [TestMethod]
        public void StartPreservation_AfterComplete_ShouldAddPreservationStartedEvent()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _lastStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);
            dut.StartPreservation();
            dut.CompletePreservation(_journey);

            // Act
            dut.StartPreservation();

            // Assert
            Assert.AreEqual(4, dut.DomainEvents.Count);
            Assert.IsInstanceOfType(dut.DomainEvents.Last(), typeof(PreservationStartedEvent));
        }

        [TestMethod]
        public void StartPreservation_ShouldThrowException_IfAlreadyStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);

            dut.StartPreservation();

            Assert.ThrowsException<Exception>(() => dut.StartPreservation());
        }

        #endregion

        #region Reschedule

        [TestMethod]
        public void Reschedule_ShouldThrowException_IfNotStarted()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);

            // Act and Assert
            Assert.ThrowsException<Exception>(() => dut.Reschedule(1, RescheduledDirection.Later, "Comment"));
        }

        [TestMethod]
        public void Reschedule_ShouldSetCorrectNextDueDateOnTagAndEachRequirement()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation();
            
            var expectedNextDueTimeFirstUtc = dut.Requirements.ElementAt(0).ActivePeriod.DueTimeUtc.AddWeeks(1);
            var expectedNextDueTimeLaterUtc = dut.Requirements.ElementAt(1).ActivePeriod.DueTimeUtc.AddWeeks(1);

            // Act
            dut.Reschedule(1, RescheduledDirection.Later, "Comment");

            // Assert
            Assert.AreEqual(expectedNextDueTimeFirstUtc, dut.Requirements.ElementAt(0).NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeLaterUtc, dut.Requirements.ElementAt(1).NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeFirstUtc, dut.NextDueTimeUtc);
        }

        [TestMethod]
        public void Reschedule_ShouldAddRescheduledEvent()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            _dutWithOneReqNotNeedInputTwoWeekInterval.Reschedule(1, RescheduledDirection.Later, "Comment");

            Assert.AreEqual(3, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Last(), typeof(RescheduledEvent));
        }

        #endregion

        #region IsReadyToBePreserved

        [TestMethod]
        public void IsReadyToBePreserved_ShouldBeFalse_BeforePeriod()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();

            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved());
        }
        
        [TestMethod]
        public void IsReadyToBePreserved_ShouldBeTrue_InPeriod_WhenNotNeedInput()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved());
        }
        
        [TestMethod]
        public void IsReadyToBePreserved_ShouldBeTrue_OnOverdue_WhenNotNeedInput()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval + TwoWeeksInterval);
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved());
        }

        [TestMethod]
        public void IsReadyToBePreserved_ShouldBeFalse_InPeriod_WhenNeedInput()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NeedInputTwoWeekInterval);
            Assert.AreEqual(PreservationStatus.NotStarted, dut.Status);

            dut.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval + TwoWeeksInterval);
            Assert.IsFalse(dut.IsReadyToBePreserved());
        }

        #endregion

        #region IsReadyToBeRescheduled
        
        [TestMethod]
        public void IsReadyToBeRescheduled_ShouldBeFalse_WhenNotStarted()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
            
            // Act
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBeRescheduled());
        }

        [TestMethod]
        public void IsReadyToBeRescheduled_ShouldBeTrue_WhenStarted()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();

            // Act
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBeRescheduled());
        }

        [TestMethod]
        public void IsReadyToBeRescheduled_ShouldBeFalse_WhenCompleted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _otherStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();
            dut.CompletePreservation(_journey);
            Assert.AreEqual(PreservationStatus.Completed, dut.Status);
            
            // Act
            Assert.IsFalse(dut.IsReadyToBeRescheduled());
        }

        #endregion

        #region IsReadyToBeEdited

        [TestMethod]
        public void IsReadyToBeEdited_ShouldBeTrue_WhenNotStarted()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);

            // Act
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBeEdited());
        }

        [TestMethod]
        public void IsReadyToBeEdited_ShouldBeTrue_WhenStarted()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();

            // Act
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBeEdited());
        }

        [TestMethod]
        public void IsReadyToBeEdited_ShouldBeFalse_WhenCompleted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _otherStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();
            dut.CompletePreservation(_journey);
            Assert.AreEqual(PreservationStatus.Completed, dut.Status);

            // Act
            Assert.IsFalse(dut.IsReadyToBeEdited());
        }

        #endregion

        #region Preserve

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservingBeforeTime()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
                   
            Assert.ThrowsException<Exception>(() 
                => _dutWithOneReqNotNeedInputTwoWeekInterval.Preserve(_person));
        }

        [TestMethod]
        public void Preserve_ShouldPreserve_WhenPreservingOnDue()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            _dutWithOneReqNotNeedInputTwoWeekInterval.Preserve(_person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
        }

        [TestMethod]
        public void Preserve_ShouldPreserve_WhenPreservingOverdue()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            _dutWithOneReqNotNeedInputTwoWeekInterval.Preserve(_person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
        }
        
        [TestMethod]
        public void Preserve_ShouldPreserveOnce_WhenPreservingMultipleTimesAtSameTime()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            Assert.ThrowsException<Exception>(() 
                => _dutWithOneReqNotNeedInputTwoWeekInterval.Preserve(_person));
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenRequirementNeedsInput()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NeedInputTwoWeekInterval);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.ThrowsException<Exception>(() => dut.Preserve(_person));
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenFirstUpcomingRequirementNeedsInput()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.ThrowsException<Exception>(() => dut.Preserve(_person));
        }
        
        [TestMethod]
        public void Preserve_ShouldThrowException_WhenFirstUpcomingRequirementNeedInput_BecauseFirstInListIsVoided()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation();
            dut.Requirements.ElementAt(0).IsVoided = true;

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            Assert.ThrowsException<Exception>(() => dut.Preserve(_person));
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservedByNotGiven()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.ThrowsException<ArgumentNullException>(() => dut.Preserve(null));
        }
        
        [TestMethod]
        public void Preserve_ShouldPreserveDueRequirementsOnly()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation();

            var req1 = dut.Requirements.ElementAt(0);
            var req2 = dut.Requirements.ElementAt(1);
            Assert.AreEqual(1, req1.PreservationPeriods.Count);
            Assert.AreEqual(1, req2.PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            dut.Preserve(_person);
            Assert.AreEqual(2, req1.PreservationPeriods.Count);
            Assert.AreEqual(1, req2.PreservationPeriods.Count);
        }
        
        [TestMethod]
        public void Preserve_ShouldChangeNextDueTime()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();
            
            var requirement = dut.OrderedRequirements().First();
            var oldNextTime = requirement.NextDueTimeUtc;
            Assert.AreEqual(_reqNotNeedInputForAllTwoWeekInterval, requirement);
            Assert.AreEqual(requirement.NextDueTimeUtc, dut.NextDueTimeUtc);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            dut.Preserve(_person);

            requirement = dut.OrderedRequirements().First();
            Assert.AreEqual(_reqNotNeedInputForAllThreeWeekInterval, requirement);
            Assert.AreEqual(requirement.NextDueTimeUtc, dut.NextDueTimeUtc);
            Assert.AreNotEqual(oldNextTime, requirement.NextDueTimeUtc);
        }

        #endregion

        #region Preserve Requirement
        
        [TestMethod]
        public void PreserveRequirement_ShouldThrowException_WhenRequirementNeedsInput()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NeedInputTwoWeekInterval);
            dut.StartPreservation();
            _timeProvider.ElapseWeeks(TwoWeeksInterval);

            Assert.ThrowsException<Exception>(() => dut.Preserve(_person, dut.Requirements.Single().Id));
        }

        [TestMethod]
        public void PreserveRequirement_ShouldThrowException_WhenPreservedByNotGiven()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();
            _timeProvider.ElapseWeeks(TwoWeeksInterval);

            Assert.ThrowsException<ArgumentNullException>(() => dut.Preserve(null, dut.Requirements.Single().Id));
        }
        
        [TestMethod]
        public void PreserveRequirement_ShouldPreserve()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            var requirement = dut.Requirements.Single();
            Assert.AreEqual(1, requirement.PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            dut.Preserve(_person, dut.Requirements.Single().Id);
            Assert.AreEqual(2, requirement.PreservationPeriods.Count);
        }

        [TestMethod]
        public void PreserveRequirement_ShouldChangeNextDueTime()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();
            
            var requirement = dut.Requirements.Single();
            var oldNextTime = requirement.NextDueTimeUtc;
            Assert.AreEqual(requirement.NextDueTimeUtc, dut.NextDueTimeUtc);
            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            dut.Preserve(_person, requirement.Id);

            Assert.AreEqual(requirement.NextDueTimeUtc, dut.NextDueTimeUtc);
            Assert.AreNotEqual(oldNextTime, requirement.NextDueTimeUtc);
        }

        [TestMethod]
        public void PreserveRequirement_ShouldAddRequirementPreservedEvent()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            var requirement = dut.Requirements.Single();
            Assert.AreEqual(1, requirement.PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            dut.Preserve(_person, dut.Requirements.Single().Id);

            Assert.AreEqual(3, dut.DomainEvents.Count);
            Assert.IsInstanceOfType(dut.DomainEvents.Last(), typeof(TagRequirementPreservedEvent));
        }

        #endregion

        #region BulkPreserve

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenRequirementNeedInput()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NeedInputTwoWeekInterval);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.ThrowsException<Exception>(() => dut.BulkPreserve(_person));
        }

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenFirstUpcomingRequirementNeedInput()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.ThrowsException<Exception>(() => dut.BulkPreserve(_person));
        }

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenFirstUpcomingRequirementNeedInput_BecauseFirstInListIsVoided()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation();
            dut.Requirements.ElementAt(0).IsVoided = true;

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            Assert.ThrowsException<Exception>(() => dut.BulkPreserve(_person));
        }

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenPreservedByNotGiven()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.ThrowsException<ArgumentNullException>(() => dut.BulkPreserve(null));
        }

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenPreservingBeforeTime()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            
            Assert.ThrowsException<Exception>(() 
                => _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_person));
        }

        [TestMethod]
        public void BulkPreserve_ShouldPreserve_WhenPreservingOnDue()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
        }

        [TestMethod]
        public void BulkPreserve_ShouldPreserve_WhenPreservingOverdue()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
        }
        
        [TestMethod]
        public void BulkPreserve_ShouldPreserveOnce_WhenPreservingMultipleTimesAtSameTime()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            Assert.ThrowsException<Exception>(() 
                => _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_person));
        }
        
        [TestMethod]
        public void BulkPreserve_ShouldPreserveDueRequirementsOnly()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation();

            var req1 = dut.Requirements.ElementAt(0);
            var req2 = dut.Requirements.ElementAt(1);
            Assert.AreEqual(1, req1.PreservationPeriods.Count);
            Assert.AreEqual(1, req2.PreservationPeriods.Count);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            dut.BulkPreserve(_person);
            Assert.AreEqual(2, req1.PreservationPeriods.Count);
            Assert.AreEqual(1, req2.PreservationPeriods.Count);
        }

        [TestMethod]
        public void BulkPreserve_ShouldChangeNextDueTime()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();
            
            var requirement = dut.OrderedRequirements().First();
            var oldNextTime = requirement.NextDueTimeUtc;
            Assert.AreEqual(_reqNotNeedInputForAllTwoWeekInterval, requirement);
            Assert.AreEqual(requirement.NextDueTimeUtc, dut.NextDueTimeUtc);
            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            dut.BulkPreserve(_person);

            requirement = dut.OrderedRequirements().First();
            Assert.AreEqual(_reqNotNeedInputForAllThreeWeekInterval, requirement);
            Assert.AreEqual(requirement.NextDueTimeUtc, dut.NextDueTimeUtc);
            Assert.AreNotEqual(oldNextTime, requirement.NextDueTimeUtc);
        }
        
        #endregion

        #region GetUpComingRequirements

        [TestMethod]
        public void GetUpComingRequirements_ShouldReturnNoneRequirements_BeforePreservationStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            Assert.AreEqual(0, dut.GetUpComingRequirements().Count());
        }
        
        [TestMethod]
        public void GetUpComingRequirements_ShouldReturnNoneRequirements_BeforeDue()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);

            dut.StartPreservation();
            Assert.AreEqual(0, dut.GetUpComingRequirements().Count());
        }
        
        [TestMethod]
        public void GetUpComingRequirements_ShouldReturnTwoReadyRequirements_WhenBothReadyAndDue()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            Assert.AreEqual(2, dut.GetUpComingRequirements().Count());
        }
        
        [TestMethod]
        public void GetUpComingRequirements_ShouldReturnReadyRequirements_WhenOverdue()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            Assert.AreEqual(1, dut.GetUpComingRequirements().Count());
        }
        
        [TestMethod]
        public void GetUpComingRequirements_ShouldReturnReadyRequirements_WhenDue()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation();

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            Assert.AreEqual(1, dut.GetUpComingRequirements().Count());
        }
        
        [TestMethod]
        public void GetUpComingRequirements_ShouldNotReturnVoidedRequirements()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();
            dut.Requirements.ElementAt(0).IsVoided = true;

            _timeProvider.ElapseWeeks(ThreeWeeksInterval);
            Assert.AreEqual(1, dut.GetUpComingRequirements().Count());
        }
        
        #endregion

        #region OrderedRequirements

        [TestMethod]
        public void OrderedRequirements_ShouldReturnAllRequirements_BeforeAndAfterPreservationStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);

            Assert.AreEqual(2, dut.OrderedRequirements().Count());
        
            dut.StartPreservation();
            
            Assert.AreEqual(2, dut.OrderedRequirements().Count());
        }
        
        [TestMethod]
        public void OrderedRequirements_ShouldNotReturnOrderedVoidedRequirements()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation();
            dut.Requirements.ElementAt(0).IsVoided = true;

            Assert.AreEqual(1, dut.OrderedRequirements().Count());
        }

        [TestMethod]
        public void OrderedRequirements_ShouldReturnOrderedVoidedRequirements()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.Requirements.ElementAt(0).IsVoided = true;

            Assert.AreEqual(2, dut.OrderedRequirements(true).Count());
        }

        [TestMethod]
        public void OrderedRequirements_ShouldChange_WhenPreserve()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();
            Assert.AreEqual(_reqNotNeedInputForAllTwoWeekInterval, dut.OrderedRequirements().First());

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            dut.Preserve(_person);

            Assert.AreEqual(_reqNotNeedInputForAllThreeWeekInterval, dut.OrderedRequirements().First());
        }

        [TestMethod]
        public void OrderedRequirements_ShouldChange_WhenBulkPreserve()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();
            Assert.AreEqual(_reqNotNeedInputForAllTwoWeekInterval, dut.OrderedRequirements().First());

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            dut.BulkPreserve(_person);

            Assert.AreEqual(_reqNotNeedInputForAllThreeWeekInterval, dut.OrderedRequirements().First());
        }
        
        [TestMethod]
        public void OrderedRequirements_ShouldNotReturnSupplierRequirements_InOtherStep()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _otherStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);

            var orderedRequirements = dut.OrderedRequirements().ToList();
            Assert.AreEqual(3, orderedRequirements.Count);
            Assert.IsNull(orderedRequirements.FirstOrDefault(r => r.RequirementDefinitionId == _reqDefNotNeedInputForSupplierId));
        }
        
        [TestMethod]
        public void OrderedRequirements_ShouldNotReturnOtherRequirements_InSupplierStep()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);

            var orderedRequirements = dut.OrderedRequirements().ToList();
            Assert.AreEqual(3, orderedRequirements.Count);
            Assert.IsNull(orderedRequirements.FirstOrDefault(r => r.RequirementDefinitionId == _reqDefNotNeedInputForOtherId));
        }

        #endregion

        #region RequirementsDueToCurrentStep
        
        [TestMethod]
        public void RequirementsDueToCurrentStep_ShouldNotReturnVoidedRequirements()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.Requirements.ElementAt(0).IsVoided = true;

            Assert.AreEqual(1, dut.RequirementsDueToCurrentStep().Count());
        }

        [TestMethod]
        public void RequirementsDueToCurrentStep_ShouldReturnVoidedRequirements()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.Requirements.ElementAt(0).IsVoided = true; 

            Assert.AreEqual(2, dut.RequirementsDueToCurrentStep(true).Count());
        }

        [TestMethod]
        public void RequirementsDueToCurrentStep_ShouldReturnAllRequirements()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);

            Assert.AreEqual(4, dut.RequirementsDueToCurrentStep(includeAllUsages: true).Count());
        }

        [TestMethod]
        public void RequirementsDueToCurrentStep_ShouldReturnRequirementsAccordingToStep_WhenInSupplierStep()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);

            var requirements = dut.RequirementsDueToCurrentStep().ToList();
            Assert.AreEqual(3, requirements.Count);
            Assert.IsFalse(requirements.Any(r => r.Usage == RequirementUsage.ForOtherThanSuppliers));
        }

        [TestMethod]
        public void RequirementsDueToCurrentStep_ShouldReturnRequirementsAccordingToStep_WhenInOtherStep()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _otherStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);

            var requirements = dut.RequirementsDueToCurrentStep().ToList();
            Assert.AreEqual(3, requirements.Count);
            Assert.IsFalse(requirements.Any(r => r.Usage == RequirementUsage.ForSuppliersOnly));
        }

        #endregion

        #region UpdateStep

        [TestMethod]
        public void UpdateStep_ShouldChangeStep()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.UpdateStep(_otherStep);

            Assert.AreEqual(_otherStep.Id, dut.StepId);
        }
                
        [TestMethod]
        public void UpdateStep_ShouldThrowException_WhenStepIsNull()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);

            Assert.ThrowsException<ArgumentNullException>(() => dut.UpdateStep(null));
        }
                
        [TestMethod]
        public void UpdateStep_ShouldThrowException_WhenTagIsPoAreaAndStepIsNotSupplierStep()
        {
            var dut = new Tag(TestPlant, TagType.PoArea, null, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);

            Assert.ThrowsException<Exception>(() => dut.UpdateStep(_otherStep));
        }

        [TestMethod]
        public void UpdateStep_ShouldAddStepChangedEvent_WhenStepChanged()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.UpdateStep(_otherStep);

            Assert.AreEqual(2, dut.DomainEvents.Count);
            Assert.IsInstanceOfType(dut.DomainEvents.Last(), typeof(StepChangedEvent));
        }

        [TestMethod]
        public void UpdateStep_ShouldNotAddAnyEvent_WhenStepNotChanged()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            Assert.AreEqual(1, dut.DomainEvents.Count);
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Last(), typeof(TagCreatedEvent));
            
            dut.UpdateStep(_supplierStep);

            Assert.AreEqual(1, dut.DomainEvents.Count);
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Last(), typeof(TagCreatedEvent));
        }

        #endregion

        #region Transfer

       [TestMethod]
        public void Transfer_ShouldTransferToNextStep()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();
            dut.Transfer(_journey);

            Assert.AreEqual(_otherStep.Id, dut.StepId);
        }
                
        [TestMethod]
        public void Transfer_ShouldThrowException_WhenJourneyIsNull()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.ThrowsException<ArgumentNullException>(() => dut.Transfer(null));
        }
                
        [TestMethod]
        public void Transfer_ShouldThrowException_WhenTagIsSiteArea()
        {
            var dut = new Tag(TestPlant, TagType.SiteArea, null, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.ThrowsException<Exception>(() => dut.Transfer(_journey));
        }
                
        [TestMethod]
        public void Transfer_ShouldThrowException_WhenTagIsPoArea()
        {
            var dut = new Tag(TestPlant, TagType.PoArea, null, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.ThrowsException<Exception>(() => dut.Transfer(_journey));
        }

        [TestMethod]
        public void Transfer_ShouldAddTransferredManuallyEvent()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();
            dut.Transfer(_journey);

            Assert.AreEqual(3, dut.DomainEvents.Count);
            Assert.IsInstanceOfType(dut.DomainEvents.Last(), typeof(TransferredManuallyEvent));
        }

        #endregion

        #region AutoTransfer

        [TestMethod]
        public void AutoTransfer_ShouldTransferToNextStep_WhenTransferMethodMatch()
        {
            // Arrange
            var otherStep2 = new Step(TestPlant, "OTHER2", _mode, _responsible);
            otherStep2.SetProtectedIdForTesting(++_nextStepId);
            _journey.AddStep(otherStep2);
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _otherStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();
            
            // Act
            dut.AutoTransfer(_journey, AutoTransferMethod.OnRfccSign);

            // Assert
            Assert.AreEqual(otherStep2.Id, dut.StepId);
        }

        [TestMethod]
        public void AutoTransfer_ShouldThrowException_WhenTransferMethodMismatch()
        {
            // Arrange
            var otherStep2 = new Step(TestPlant, "OTHER2", _mode, _responsible);
            otherStep2.SetProtectedIdForTesting(++_nextStepId);
            _journey.AddStep(otherStep2);
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _otherStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            // Act and assert
            Assert.ThrowsException<Exception>(() => dut.AutoTransfer(_journey, AutoTransferMethod.OnRfocSign));
        }
                
        [TestMethod]
        public void AutoTransfer_ShouldThrowException_WhenJourneyIsNull()
        {
            // Arrange
            var otherStep2 = new Step(TestPlant, "OTHER2", _mode, _responsible);
            otherStep2.SetProtectedIdForTesting(++_nextStepId);
            _journey.AddStep(otherStep2);
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _otherStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            // Act and assert
            Assert.ThrowsException<ArgumentNullException>(() => dut.AutoTransfer(null, AutoTransferMethod.OnRfccSign));
        }
                
        [TestMethod]
        public void AutoTransfer_ShouldThrowException_WhenTagIsSiteArea()
        {
            // Arrange
            var otherStep2 = new Step(TestPlant, "OTHER2", _mode, _responsible);
            otherStep2.SetProtectedIdForTesting(++_nextStepId);
            _journey.AddStep(otherStep2);
            var dut = new Tag(TestPlant, TagType.SiteArea, null, "", "", _otherStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            // Act and assert
            Assert.ThrowsException<Exception>(() => dut.AutoTransfer(_journey, AutoTransferMethod.OnRfccSign));
        }
                
        [TestMethod]
        public void AutoTransfer_ShouldThrowException_WhenTagIsPoArea()
        {
            // Arrange
            var otherStep2 = new Step(TestPlant, "OTHER2", _mode, _responsible);
            otherStep2.SetProtectedIdForTesting(++_nextStepId);
            _journey.AddStep(otherStep2);
            var dut = new Tag(TestPlant, TagType.PoArea, null, "", "", _otherStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            // Act and assert
            Assert.ThrowsException<Exception>(() => dut.AutoTransfer(_journey, AutoTransferMethod.OnRfccSign));
        }

        [TestMethod]
        public void AutoTransfer_ShouldAddTransferredAutomaticallyEvent()
        {
            // Arrange
            var otherStep2 = new Step(TestPlant, "OTHER2", _mode, _responsible);
            otherStep2.SetProtectedIdForTesting(++_nextStepId);
            _journey.AddStep(otherStep2);
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _otherStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            // Act
            dut.AutoTransfer(_journey, AutoTransferMethod.OnRfccSign);

            // Assert
            Assert.AreEqual(3, dut.DomainEvents.Count);
            Assert.IsInstanceOfType(dut.DomainEvents.Last(), typeof(TransferredAutomaticallyEvent));
        }

        #endregion

        #region CompletePreservation

        [TestMethod]
        public void CompletePreservation_ShouldSetStatusToCompleted_WhenInLastStepAndIsStandard()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _otherStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            dut.CompletePreservation(_journey);

            Assert.AreEqual(PreservationStatus.Completed, dut.Status);
        }

        [TestMethod]
        public void CompletePreservation_ShouldThrowException_WhenJourneyIsNull()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.ThrowsException<ArgumentNullException>(() => dut.CompletePreservation(null));
        }

        [TestMethod]
        public void CompletePreservation_ShouldSetStatusToCompleted_WhenNotInLastStepAndIsArea()
        {
            var dut = new Tag(TestPlant, TagType.SiteArea, null, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            dut.CompletePreservation(_journey);

            Assert.AreEqual(PreservationStatus.Completed, dut.Status);
        }

        [TestMethod]
        public void CompletePreservation_ShouldAddPreservationCompletedEvent()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _otherStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            dut.CompletePreservation(_journey);

            Assert.AreEqual(3, dut.DomainEvents.Count);
            Assert.IsInstanceOfType(dut.DomainEvents.Last(), typeof(PreservationCompletedEvent));
        }

        #endregion

        #region IsReadyToBeTransferred

        [TestMethod]
        public void IsReadyToBeTransferred_ShouldBeTrue_BeforePreservationStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);

            Assert.IsTrue(dut.IsReadyToBeTransferred(_journey));
        }
        
        [TestMethod]
        public void IsReadyToBeTransferred_ShouldBeTrue_WhenPreservationActive()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.IsTrue(dut.IsReadyToBeTransferred(_journey));
        }
        
        [TestMethod]
        public void IsReadyToBeTransferred_ShouldBeFalse_WhenPreservationCompleted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _otherStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            dut.CompletePreservation(_journey);

            Assert.IsFalse(dut.IsReadyToBeTransferred(_journey));
        }
        
        [TestMethod]
        public void IsReadyToBeTransferred_ShouldBeFalse_WhenCurrentStepIsLastStepInJourney()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _otherStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.IsFalse(dut.IsReadyToBeTransferred(_journey));
        }
                
        [TestMethod]
        public void IsReadyToBeTransferred_ShouldThrowException_WhenJourneyIsNull()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.ThrowsException<ArgumentNullException>(() => dut.IsReadyToBeTransferred(null));
        }

        #endregion
        
        #region IsReadyToBeCompleted

        [TestMethod]
        public void IsReadyToBeCompleted_ShouldBeFalse_BeforePreservationStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _otherStep, _oneReq_NotNeedInputTwoWeekInterval);

            Assert.IsFalse(dut.IsReadyToBeCompleted(_journey));
        }

        [TestMethod]
        public void IsReadyToBeCompleted_ShouldBeTrue_AfterPreservationStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _otherStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.IsTrue(dut.IsReadyToBeCompleted(_journey));
        }

        [TestMethod]
        public void IsReadyToBeCompleted_ShouldBeFalse_WhenCurrentStepIsLastStepInJourney()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.IsFalse(dut.IsReadyToBeCompleted(_journey));
        }

        [TestMethod]
        public void IsReadyToBeCompleted_ShouldThrowException_WhenJourneyIsNull()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.ThrowsException<ArgumentNullException>(() => dut.IsReadyToBeCompleted(null));
        }

        #endregion

        #region IsReadyToBeStarted

        [TestMethod]
        public void IsReadyToBeStarted_ShouldBeTrue_BeforePreservationStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);

            Assert.IsTrue(dut.IsReadyToBeStarted());
        }
        
        [TestMethod]
        public void IsReadyToBeStarted_ShouldBeFalse_AfterPreservationStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();

            Assert.IsFalse(dut.IsReadyToBeStarted());
        }

        [TestMethod]
        public void IsReadyToBeStarted_ShouldBeTrue_AfterPreservationCompleted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _otherStep, _oneReq_NotNeedInputTwoWeekInterval);
            dut.StartPreservation();
            dut.CompletePreservation(_journey);

            Assert.IsTrue(dut.IsReadyToBeStarted());
        }

        #endregion

        #region IsReadyToBeDuplicated

        [TestMethod]
        public void IsReadyToBeDuplicated_ShouldBeTrue_ForSiteAreaTag()
        {
            var dut = new Tag(TestPlant, TagType.SiteArea, null, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);

            Assert.IsTrue(dut.IsReadyToBeDuplicated());
        }
        
        [TestMethod]
        public void IsReadyToBeDuplicated_ShouldBeTrue_ForPreAreaTag()
        {
            var dut = new Tag(TestPlant, TagType.PreArea, null, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);

            Assert.IsTrue(dut.IsReadyToBeDuplicated());
        }

        [TestMethod]
        public void IsReadyToBeDuplicated_ShouldBeFalse_ForStandardTag()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);

            Assert.IsFalse(dut.IsReadyToBeDuplicated());
        }
        
        [TestMethod]
        public void IsReadyToBeDuplicated_ShouldBeFalse_ForPoAreaTag()
        {
            var dut = new Tag(TestPlant, TagType.PoArea, null, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);

            Assert.IsFalse(dut.IsReadyToBeDuplicated());
        }

        #endregion
        
        #region AddAction

        [TestMethod]
        public void AddAction_ShouldAddAction()
        {
            var action = new Action(TestPlant, "", "", null);
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddAction(action);

            Assert.AreEqual(action, _dutWithOneReqNotNeedInputTwoWeekInterval.Actions.First());
        }

        [TestMethod]
        public void AddAction_ShouldThrowException_WhenActionNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dutWithOneReqNotNeedInputTwoWeekInterval.AddAction(null));

        [TestMethod]
        public void AddAction_ShouldAddActionAddedEvent()
        {
            var action = new Action(TestPlant, "", "", null);
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddAction(action);

            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Last(), typeof(ActionAddedEvent));
        }

        #endregion

        #region CloseAction

        [TestMethod]
        public void CloseAction_ShouldCloseAction()
        {
            var action = new Action(TestPlant, "", "", null);
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddAction(action);
            var closedAction = _dutWithOneReqNotNeedInputTwoWeekInterval.CloseAction(action.Id, _person, DateTime.UtcNow, "AAAAAAAAABA=");

            Assert.IsTrue(closedAction.IsClosed);
        }

        [TestMethod]
        public void CloseAction_ShouldSetClosedBy()
        {
            var action = new Action(TestPlant, "", "", null);
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddAction(action);
            var closedAction = _dutWithOneReqNotNeedInputTwoWeekInterval.CloseAction(action.Id, _person, DateTime.UtcNow, "AAAAAAAAABA=");

            Assert.AreEqual(_person.Id, closedAction.ClosedById);
        }

        [TestMethod]
        public void CloseAction_ShouldThrowException_WhenActionIdIsInvalid()
            => Assert.ThrowsException<InvalidOperationException>(() => _dutWithOneReqNotNeedInputTwoWeekInterval.CloseAction(0, _person, DateTime.UtcNow, "AAAAAAAAABA="));

        [TestMethod]
        public void CloseAction_ShouldAddActionClosedEvent()
        {
            var action = new Action(TestPlant, "", "", null);
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddAction(action);
            _dutWithOneReqNotNeedInputTwoWeekInterval.CloseAction(action.Id, _person, DateTime.UtcNow, "AAAAAAAAABA=");

            Assert.AreEqual(3, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Last(), typeof(ActionClosedEvent));
        }

        [TestMethod]
        public void CloseAction_ShouldReturnClosedAction()
        {
            var action = new Action(TestPlant, "", "", null);
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddAction(action);
            var closedAction = _dutWithOneReqNotNeedInputTwoWeekInterval.CloseAction(action.Id, _person, DateTime.UtcNow, "AAAAAAAAABA=");

            Assert.AreEqual(action, closedAction);
        }

        #endregion

        #region SetArea

        [TestMethod]
        public void SetArea_ShouldSetAreaProperties()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.SetArea("AC", "AD");

            Assert.AreEqual("AC", _dutWithOneReqNotNeedInputTwoWeekInterval.AreaCode);
            Assert.AreEqual("AD", _dutWithOneReqNotNeedInputTwoWeekInterval.AreaDescription);
        }

        #endregion

        #region SetDiscipline

        [TestMethod]
        public void SetDiscipline_ShouldSetDisciplineProperties()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.SetDiscipline("DC", "DD");

            Assert.AreEqual("DC", _dutWithOneReqNotNeedInputTwoWeekInterval.DisciplineCode);
            Assert.AreEqual("DD", _dutWithOneReqNotNeedInputTwoWeekInterval.DisciplineDescription);
        }

        #endregion
        
        #region AddAttachment

        [TestMethod]
        public void AddAttachment_ShouldAddAttachment()
        {
            var attachment = new TagAttachment(TestPlant, Guid.Empty, "A.txt");
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddAttachment(attachment);

            Assert.AreEqual(attachment, _dutWithOneReqNotNeedInputTwoWeekInterval.Attachments.First());
        }

        [TestMethod]
        public void AddAttachment_ShouldThrowException_WhenAttachmentNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dutWithOneReqNotNeedInputTwoWeekInterval.AddAttachment(null));

        #endregion
        
        #region RemoveAttachment

        [TestMethod]
        public void RemoveAttachment_ShouldRemoveAttachment()
        {
            var attachment = new TagAttachment(TestPlant, Guid.Empty, "A.txt");
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddAttachment(attachment);
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Attachments.Count);

            // Act
            _dutWithOneReqNotNeedInputTwoWeekInterval.RemoveAttachment(attachment);

            Assert.AreEqual(0, _dutWithOneReqNotNeedInputTwoWeekInterval.Attachments.Count);
        }

        [TestMethod]
        public void RemoveAttachment_ShouldThrowException_WhenAttachmentNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dutWithOneReqNotNeedInputTwoWeekInterval.RemoveAttachment(null));

        #endregion
        
        #region GetAttachmentByFileName

        [TestMethod]
        public void GetAttachmentByFileName_ShouldGetAttachmentWhenExists()
        {
            // Arrange
            var fileName = "FileA";
            var attachment = new TagAttachment(TestPlant, Guid.Empty, fileName);
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddAttachment(attachment);

            // Act
            var result = _dutWithOneReqNotNeedInputTwoWeekInterval.GetAttachmentByFileName(fileName);

            // Arrange
            Assert.AreEqual(attachment, result);
        }

        [TestMethod]
        public void GetAttachmentByFileName_ShouldGetAttachmentWhenExists_RegardlessOfCasing()
        {
            // Arrange
            var fileName = "FileA";
            var attachment = new TagAttachment(TestPlant, Guid.Empty, fileName);
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddAttachment(attachment);

            // Act
            var result = _dutWithOneReqNotNeedInputTwoWeekInterval.GetAttachmentByFileName(fileName.ToUpper());

            // Arrange
            Assert.AreEqual(attachment, result);
        }

        [TestMethod]
        public void GetAttachmentByFileName_ShouldReturnNullWhenNotFound()
        {
            // Act
            var result = _dutWithOneReqNotNeedInputTwoWeekInterval.GetAttachmentByFileName("FileA");

            // Arrange
            Assert.IsNull(result);
        }

        #endregion
        
        #region FollowsAJourney

        [TestMethod]
        public void FollowsAJourney_ShouldGetBeTrueForStandardTag()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);

            // Act and Arrange
            Assert.IsTrue(dut.FollowsAJourney);
        }

        [TestMethod]
        public void FollowsAJourney_ShouldGetBeFalseForSiteAreaTag()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.SiteArea, null, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);

            // Act and Arrange
            Assert.IsFalse(dut.FollowsAJourney);
        }

        [TestMethod]
        public void FollowsAJourney_ShouldGetBeTrueForPreAreaTag()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.PreArea, null, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);

            // Act and Arrange
            Assert.IsTrue(dut.FollowsAJourney);
        }

        [TestMethod]
        public void FollowsAJourney_ShouldGetBeFalseForPoAreaTag()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.PoArea, null, "", "", _supplierStep, _oneReq_NotNeedInputTwoWeekInterval);

            // Act and Arrange
            Assert.IsFalse(dut.FollowsAJourney);
        }

        #endregion

        #region Void

        [TestMethod]
        public void Void_ShouldAddTagVoidedEvent_WhenNotAlreadyVoided()
        {
            // Arrange
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided);

            // Act
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided = true;

            // Assert
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Last(), typeof(TagVoidedEvent));
        }

        [TestMethod]
        public void Void_ShouldNotAddAnotherTagVoidedEvent_WhenAlreadyVoided()
        {
            // Arrange
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided = true;
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);

            // Act
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided = true;

            // Assert
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
        }

        #endregion

        #region Unvoid

        [TestMethod]
        public void Unvoid_ShouldAddTagUnvoidedEvent_WhenVoided()
        {
            // Arrange
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided = true;
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided);
            
            // Act
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided = false;

            // Assert
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided);
            Assert.AreEqual(3, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Last(), typeof(TagUnvoidedEvent));
        }

        [TestMethod]
        public void Unvoid_ShouldNotAddAnotherTagUnvoidedEvent_WhenAlreadyUnvoided()
        {
            // Arrange
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided = true;
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided = false;
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided);
            Assert.AreEqual(3, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
            
            // Act
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided = false;

            // Assert
            Assert.AreEqual(3, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
        }

        #endregion

        #region UpdateRequirement

        [TestMethod]
        public void UpdateRequirement_ShouldUpdateRequirement()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            var requirement = dut.Requirements.First();

            Assert.IsFalse(requirement.IsVoided);
            Assert.AreEqual(requirement.IntervalWeeks, 2);

            // Act
            dut.UpdateRequirement(requirement.Id, true, 1, "AAAAAAAAABA=");

            // Assert
            Assert.IsTrue(requirement.IsVoided);
            Assert.AreEqual(requirement.IntervalWeeks, 1);
        }

        [TestMethod]
        public void UpdateRequirement_Void_ShouldVoidRequirement()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            var requirement = dut.Requirements.First();
            
            // Act
            dut.UpdateRequirement(requirement.Id, true, requirement.IntervalWeeks, "AAAAAAAAABA=");

            // Assert
            Assert.IsTrue(requirement.IsVoided);
        }

        [TestMethod]
        public void UpdateRequirement_Void_ShouldAddRequirementVoidedEvent()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            var requirement = dut.Requirements.First();
            
            // Act
            dut.UpdateRequirement(requirement.Id, true, requirement.IntervalWeeks, "AAAAAAAAABA=");

            // Assert
            Assert.AreEqual(2, dut.DomainEvents.Count);
            Assert.IsInstanceOfType(dut.DomainEvents.Last(), typeof(TagRequirementVoidedEvent));
        }

        [TestMethod]
        public void UpdateRequirement_Unvoid_ShouldUnvoidRequirement()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            var requirement = dut.Requirements.First();
            dut.UpdateRequirement(requirement.Id, true, requirement.IntervalWeeks, "AAAAAAAAABA=");
            Assert.IsTrue(requirement.IsVoided);

            // Act
            dut.UpdateRequirement(requirement.Id, false, requirement.IntervalWeeks, "AAAAAAAAABA=");

            // Assert
            Assert.IsFalse(requirement.IsVoided);
        }

        [TestMethod]
        public void UpdateRequirement_Unvoid_ShouldAddRequirementUnvoidedEvent()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            var requirement = dut.Requirements.First();
            dut.UpdateRequirement(requirement.Id, true, requirement.IntervalWeeks, "AAAAAAAAABA=");
            
            // Act
            dut.UpdateRequirement(requirement.Id, false, requirement.IntervalWeeks, "AAAAAAAAABA=");

            // Assert
            Assert.AreEqual(3, dut.DomainEvents.Count);
            Assert.IsInstanceOfType(dut.DomainEvents.Last(), typeof(TagRequirementUnvoidedEvent));
        }

        [TestMethod]
        public void UpdateRequirement_Unvoid_ShouldStartPreservation_OnUnvoidedRequirement_WhenPreservationStarted()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            var requirement = dut.Requirements.First();
            dut.UpdateRequirement(requirement.Id, true, requirement.IntervalWeeks, "AAAAAAAAABA=");
            dut.StartPreservation();
            Assert.IsFalse(requirement.HasActivePeriod);
            Assert.IsTrue(requirement.IsVoided);

            // Act
            dut.UpdateRequirement(requirement.Id, false, requirement.IntervalWeeks, "AAAAAAAAABA=");

            // Assert
            Assert.IsTrue(requirement.HasActivePeriod);
            Assert.IsFalse(requirement.IsVoided);
        }

        [TestMethod]
        public void UpdateRequirement_Unvoid_ShouldNotStartPreservation_OnUnvoidedRequirement_WhenPreservationNotStarted()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            var requirement = dut.Requirements.First();
            dut.UpdateRequirement(requirement.Id, true, requirement.IntervalWeeks, "AAAAAAAAABA=");
            Assert.IsFalse(requirement.HasActivePeriod);
            
            // Act
            dut.UpdateRequirement(requirement.Id, false, requirement.IntervalWeeks, "AAAAAAAAABA=");

            // Assert
            Assert.IsFalse(requirement.HasActivePeriod);
            Assert.IsFalse(requirement.IsVoided);
        }
                        
        [TestMethod]
        public void UpdateRequirement_Void_ShouldChangeNextDueTime_WhenPreservationStarted()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();
            
            var twoWeekRequirement = dut.OrderedRequirements().First();
            var threeWeekRequirement = dut.OrderedRequirements().Last();
            Assert.AreEqual(twoWeekRequirement.NextDueTimeUtc, dut.NextDueTimeUtc);

            // Act
            dut.UpdateRequirement(twoWeekRequirement.Id, true, twoWeekRequirement.IntervalWeeks, "AAAAAAAAABA=");

            // Assert
            Assert.AreEqual(threeWeekRequirement.NextDueTimeUtc, dut.NextDueTimeUtc);
        }
                        
        [TestMethod]
        public void UpdateRequirement_UnVoid_ShouldChangeNextDueTime_WhenPreservationStarted()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation();
            
            var twoWeekRequirement = dut.OrderedRequirements().First();
            var threeWeekRequirement = dut.OrderedRequirements().Last();
            Assert.AreEqual(twoWeekRequirement.NextDueTimeUtc, dut.NextDueTimeUtc);
            dut.UpdateRequirement(twoWeekRequirement.Id, true, twoWeekRequirement.IntervalWeeks, "AAAAAAAAABA=");
            Assert.AreEqual(threeWeekRequirement.NextDueTimeUtc, dut.NextDueTimeUtc);
        
            // Act
            dut.UpdateRequirement(twoWeekRequirement.Id, false, twoWeekRequirement.IntervalWeeks, "AAAAAAAAABA=");

            // Assert
            Assert.AreEqual(twoWeekRequirement.NextDueTimeUtc, dut.NextDueTimeUtc);
        }
                        
        [TestMethod]
        public void UpdateRequirement_Void_ShouldKeepDueTimeNull_BeforePreservationStarted()
        {
            // Arrange
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            
            var twoWeekRequirement = dut.OrderedRequirements().First();
            Assert.IsFalse(dut.NextDueTimeUtc.HasValue);

            // Act
            dut.UpdateRequirement(twoWeekRequirement.Id, true, twoWeekRequirement.IntervalWeeks, "AAAAAAAAABA=");

            // Assert
            Assert.IsFalse(dut.NextDueTimeUtc.HasValue);
        }

        #endregion

        #region ChangeInterval

        [TestMethod]
        public void ChangeInterval_ShouldChangeInterval()
        {
            var requirement = _oneReq_NotNeedInputTwoWeekInterval.First();
            Assert.AreEqual(2, requirement.IntervalWeeks);

            _dutWithOneReqNotNeedInputTwoWeekInterval.ChangeInterval(requirement.Id, 1);

            Assert.AreEqual(requirement.IntervalWeeks, 1);
        }

        [TestMethod]
        public void ChangeInterval_ShouldAddIntervalChangedEvent_WhenIntervalChanged()
        {
            var requirement = _oneReq_NotNeedInputTwoWeekInterval.First();
            Assert.AreEqual(2, requirement.IntervalWeeks);

            _dutWithOneReqNotNeedInputTwoWeekInterval.ChangeInterval(requirement.Id, 3);

            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Last(), typeof(IntervalChangedEvent));
        }

        [TestMethod]
        public void ChangeInterval_ShouldNotAddIntervalChangedEvent_WhenIntervalNotChanged()
        {
            var requirement = _oneReq_NotNeedInputTwoWeekInterval.First();

            _dutWithOneReqNotNeedInputTwoWeekInterval.ChangeInterval(requirement.Id, requirement.IntervalWeeks);

            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Last(), typeof(TagCreatedEvent));
        }

        #endregion

        #region IsAreaTag

        [TestMethod]
        public void IsAreaTag_ShouldReturnFalse_ForStandardTag()
            => Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsAreaTag());

        [TestMethod]
        public void IsAreaTag_ShouldReturnTrue_ForPoTag()
        {
            var dut = new Tag(
                TestPlant,
                TagType.PoArea,
                null,
                "TagNoA",
                "DescA", 
                _supplierStep,
                _oneReq_NotNeedInputTwoWeekInterval);
            Assert.IsTrue(dut.IsAreaTag());
        }

        [TestMethod]
        public void IsAreaTag_ShouldReturnTrue_ForPreTag()
        {
            var dut = new Tag(
                TestPlant,
                TagType.PreArea,
                null,
                "TagNoA",
                "DescA", 
                _supplierStep,
                _oneReq_NotNeedInputTwoWeekInterval);
            Assert.IsTrue(dut.IsAreaTag());
        }

        [TestMethod]
        public void IsAreaTag_ShouldReturnTrue_ForSiteTag()
        {
            var dut = new Tag(
                TestPlant,
                TagType.SiteArea,
                null,
                "TagNoA",
                "DescA", 
                _supplierStep,
                _oneReq_NotNeedInputTwoWeekInterval);
            Assert.IsTrue(dut.IsAreaTag());
        }

        #endregion

        #region Rename
        [TestMethod]
        public void Rename_ShouldUpdateTagNo()
        {
            var newTagNo = "A-Brand-new-tag-no";
            Assert.AreNotEqual(newTagNo, _dutWithOneReqNotNeedInputTwoWeekInterval.TagNo);

            _dutWithOneReqNotNeedInputTwoWeekInterval.Rename(newTagNo);

            Assert.AreEqual(newTagNo, _dutWithOneReqNotNeedInputTwoWeekInterval.TagNo);
        }
        #endregion

        #region UndoStartPreservation

        [TestMethod]
        public void UndoStartPreservation_ShouldSetStatusNotStarted()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            Assert.AreEqual(PreservationStatus.Active, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);

            _dutWithOneReqNotNeedInputTwoWeekInterval.UndoStartPreservation();

            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
        }

        [TestMethod]
        public void UndoStartPreservation_ShouldClearDueDateOnTagAndEachRequirement()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);
            dut.StartPreservation();

            dut.UndoStartPreservation();

            Assert.IsFalse(dut.Requirements.ElementAt(0).NextDueTimeUtc.HasValue);
            Assert.IsFalse(dut.Requirements.ElementAt(1).NextDueTimeUtc.HasValue);
            Assert.IsFalse(dut.NextDueTimeUtc.HasValue);
        }

        [TestMethod]
        public void UndoStartPreservation_ShouldSetStatusNotStartedOnVoidedRequirements()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);
            dut.StartPreservation();
            dut.Requirements.ElementAt(0).IsVoided = true;

            dut.UndoStartPreservation();

            Assert.IsFalse(dut.Requirements.ElementAt(0).NextDueTimeUtc.HasValue);
        }

        [TestMethod]
        public void UndoStartPreservation_ShouldThrowException_IfNotStarted()
        {
            var dut = new Tag(TestPlant, TagType.Standard, _testGuid, "", "", _supplierStep, _fourReqs_NoneNeedInput_DifferentIntervals_OneForSupplier_OneForOther);

            Assert.ThrowsException<Exception>(() => dut.UndoStartPreservation());
        }

        [TestMethod]
        public void UndoStartPreservation_ShouldAddUndoPreservationStartedEvent()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation();
            
            _dutWithOneReqNotNeedInputTwoWeekInterval.UndoStartPreservation();

            Assert.AreEqual(3, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Last(), typeof(UndoPreservationStartedEvent));
        }

        #endregion

        #region VoidInSource

        [TestMethod]
        public void VoidInSource_ShouldAddEvents_WhenNotAlreadyVoidedInSource()
        {
            // Arrange
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource);

            // Act
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource = true;

            // Assert
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource);
            Assert.AreEqual(3, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.ElementAt(1), typeof(TagVoidedInSourceEvent));
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.ElementAt(2), typeof(TagVoidedEvent));
        }

        [TestMethod]
        public void VoidInSource_ShouldNotAddAnotherTagVoidedInSourceEvent_WhenAlreadyVoidedInSource()
        {
            // Arrange
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource = true;
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource);
            Assert.AreEqual(3, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);

            // Act
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource = true;

            // Assert
            Assert.AreEqual(3, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
        }

        [TestMethod]
        public void VoidInSource_ShouldVoidInPreservation()
        {
            // Arrange
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource);
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided);

            // Act
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource = true;

            // Assert
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource);
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided);
        }

        #endregion

        #region UnvoidInSource

        [TestMethod]
        public void UnvoidInSource_ShouldAddEvents_WhenVoidedInSource()
        {
            // Arrange
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource = true;
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource);

            // Act
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource = false;

            // Assert
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource);
            Assert.AreEqual(5, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.ElementAt(3), typeof(TagUnvoidedInSourceEvent));
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.ElementAt(4), typeof(TagUnvoidedEvent));
        }

        [TestMethod]
        public void UnvoidInSource_ShouldNotAddAnotherTagUnvoidedInSourceEvent_WhenAlreadyUnvoidedInSource()
        {
            // Arrange
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource = true;
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource = false;
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource);
            Assert.AreEqual(5, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);

            // Act
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource = false;

            // Assert
            Assert.AreEqual(5, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
        }

        [TestMethod]
        public void UnvoidInSource_ShouldUnvoidInPreservation()
        {
            // Arrange
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource = true;
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource);
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided);

            // Act
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource = false;

            // Assert
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource);
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided);
        }

        #endregion

        #region DeleteInSource

        [TestMethod]
        public void DeleteInSource_ShouldAddEvents_WhenNotDeletedInSource()
        {
            // Arrange
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsDeletedInSource);

            // Act
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsDeletedInSource = true;

            // Assert
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsDeletedInSource);
            Assert.AreEqual(4, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.ElementAt(1), typeof(TagDeletedInSourceEvent));
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.ElementAt(2), typeof(TagVoidedInSourceEvent));
            Assert.IsInstanceOfType(_dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.ElementAt(3), typeof(TagVoidedEvent));
        }

        [TestMethod]
        public void DeleteInSource_ShouldVoidInPreservation()
        {
            // Arrange
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsDeletedInSource);
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided);

            // Act
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsDeletedInSource = true;

            // Assert
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoided);
        }

        [TestMethod]
        public void DeleteInSource_ShouldSetIsVoidedInSourceInPreservation()
        {
            // Arrange
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsDeletedInSource);
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource);

            // Act
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsDeletedInSource = true;

            // Assert
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsVoidedInSource);
        }

        [TestMethod]
        public void DeleteInSource_ShouldNotAddAnotherTagDeletedInSourceEvent_WhenAlreadyDeletedInSource()
        {
            // Arrange
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsDeletedInSource = true;
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsDeletedInSource);
            Assert.AreEqual(4, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);

            // Act
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsDeletedInSource = true;

            // Assert
            Assert.AreEqual(4, _dutWithOneReqNotNeedInputTwoWeekInterval.DomainEvents.Count);
        }

        [TestMethod]
        public void UnDeleteInSource_ShouldThrowException()
        {
            // Arrange
            _dutWithOneReqNotNeedInputTwoWeekInterval.IsDeletedInSource = true;
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsDeletedInSource);

            // Act and Assert
            Assert.ThrowsException<Exception>(() => _dutWithOneReqNotNeedInputTwoWeekInterval.IsDeletedInSource = false);
        }

        #endregion
    }
}

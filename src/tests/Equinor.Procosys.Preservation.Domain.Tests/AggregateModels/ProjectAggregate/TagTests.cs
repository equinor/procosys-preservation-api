using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class TagTests
    {
        private const int TwoWeeksInterval = 2;
        private const int ThreeWeeksInterval = 3;
        private Mock<Step> _stepMock;
        private Person _person;
        
        private RequirementDefinition _reqDef1NotNeedInput;
        private RequirementDefinition _reqDef2NotNeedInput;
        private RequirementDefinition _reqDef1NeedInput;
        private RequirementDefinition _reqDef2NeedInput;
        private Requirement _reqNotNeedInputTwoWeekInterval;
        private Requirement _reqNotNeedInputThreeWeekInterval;
        private Requirement _reqNeedInputTwoWeekInterval;
        private Requirement _reqNeedInputThreeWeekInterval;
        private List<Requirement> _twoReqs_NoneNeedInput_DifferentIntervals;
        private List<Requirement> _oneReq_NotNeedInputTwoWeekInterval;
        private List<Requirement> _oneReq_NeedInputTwoWeekInterval;
        private List<Requirement> _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval;
        private List<Requirement> _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval;
        private List<Requirement> _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval;
        
        private DateTime _utcNow;
        private DateTime _dueTimeForTwoWeeksInterval;
        private DateTime _dueTimeForThreeWeeksInterval;
        private Tag _dutWithOneReqNotNeedInputTwoWeekInterval;

        [TestInitialize]
        public void Setup()
        {
            _stepMock = new Mock<Step>();
            _stepMock.SetupGet(x => x.Id).Returns(3);

            _person = new Mock<Person>().Object;

            _reqDef1NotNeedInput = new RequirementDefinition("", "", 2, 0);
            _reqDef1NotNeedInput.AddField(new Field("", "", FieldType.Info, 0));
            _reqDef2NotNeedInput = new RequirementDefinition("", "", 2, 0);
            _reqDef2NotNeedInput.AddField(new Field("", "", FieldType.Info, 0));
            _reqDef1NeedInput = new RequirementDefinition("", "", 1, 0);
            _reqDef1NeedInput.AddField(new Field("", "", FieldType.CheckBox, 0));
            _reqDef2NeedInput = new RequirementDefinition("", "", 1, 0);
            _reqDef2NeedInput.AddField(new Field("", "", FieldType.CheckBox, 0));
            
            _reqNotNeedInputTwoWeekInterval = new Requirement("", TwoWeeksInterval, _reqDef1NotNeedInput);
            _reqNotNeedInputThreeWeekInterval = new Requirement("", ThreeWeeksInterval, _reqDef2NotNeedInput);
            _reqNeedInputTwoWeekInterval = new Requirement("", TwoWeeksInterval, _reqDef1NeedInput);
            _reqNeedInputThreeWeekInterval = new Requirement("", ThreeWeeksInterval, _reqDef2NeedInput);

            _twoReqs_NoneNeedInput_DifferentIntervals = new List<Requirement>
            {
                _reqNotNeedInputTwoWeekInterval, _reqNotNeedInputThreeWeekInterval
            };

            _oneReq_NotNeedInputTwoWeekInterval = new List<Requirement>
            {
                _reqNotNeedInputTwoWeekInterval
            };

            _oneReq_NeedInputTwoWeekInterval = new List<Requirement>
            {
                _reqNeedInputTwoWeekInterval
            };

            _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval = new List<Requirement>
            {
                _reqNeedInputTwoWeekInterval, _reqNotNeedInputThreeWeekInterval
            };

            _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval = new List<Requirement>
            {
                _reqNotNeedInputTwoWeekInterval, _reqNeedInputThreeWeekInterval
            };

            _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval = new List<Requirement>
            {
                _reqNotNeedInputTwoWeekInterval, _reqNotNeedInputThreeWeekInterval
            };

            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _dueTimeForTwoWeeksInterval = _utcNow.AddWeeks(TwoWeeksInterval);
            _dueTimeForThreeWeeksInterval = _utcNow.AddWeeks(ThreeWeeksInterval);

            _dutWithOneReqNotNeedInputTwoWeekInterval = new Tag(
                "SchemaA",
                TagType.Standard,
                "TagNoA",
                "DescA", 
                "AreaCodeA", 
                "CalloffA", 
                "DisciplineA", 
                "McPkgA", 
                "CommPkgA", 
                "PurchaseOrderA", 
                "RemarkA", 
                "TagFunctionCodeA", 
                _stepMock.Object,
                _oneReq_NotNeedInputTwoWeekInterval);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual("SchemaA", _dutWithOneReqNotNeedInputTwoWeekInterval.Schema);
            Assert.AreEqual("TagNoA", _dutWithOneReqNotNeedInputTwoWeekInterval.TagNo);
            Assert.AreEqual("DescA", _dutWithOneReqNotNeedInputTwoWeekInterval.Description);
            Assert.AreEqual("AreaCodeA", _dutWithOneReqNotNeedInputTwoWeekInterval.AreaCode);
            Assert.AreEqual("CalloffA", _dutWithOneReqNotNeedInputTwoWeekInterval.Calloff);
            Assert.AreEqual("DisciplineA", _dutWithOneReqNotNeedInputTwoWeekInterval.DisciplineCode);
            Assert.AreEqual("McPkgA", _dutWithOneReqNotNeedInputTwoWeekInterval.McPkgNo);
            Assert.AreEqual("PurchaseOrderA", _dutWithOneReqNotNeedInputTwoWeekInterval.PurchaseOrderNo);
            Assert.AreEqual("RemarkA", _dutWithOneReqNotNeedInputTwoWeekInterval.Remark);
            Assert.AreEqual("TagFunctionCodeA", _dutWithOneReqNotNeedInputTwoWeekInterval.TagFunctionCode);
            Assert.AreEqual(_stepMock.Object.Id, _dutWithOneReqNotNeedInputTwoWeekInterval.StepId);
            Assert.AreEqual(TagType.Standard, _dutWithOneReqNotNeedInputTwoWeekInterval.TagType);
            var requirements = _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements;
            Assert.AreEqual(1, requirements.Count);
            
            var req = _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.ElementAt(0);
            Assert.IsNull(req.NextDueTimeUtc);
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
        }
        
        [TestMethod]
        public void Constructor_ShouldNotSetReadyToBePreserved_AtAnyTime()
        {
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved(_utcNow));
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved(_utcNow.AddWeeks(TwoWeeksInterval)));
            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved(_utcNow.AddWeeks(ThreeWeeksInterval)));
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenStepNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(()
                => new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", null, _twoReqs_NoneNeedInput_DifferentIntervals));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementsNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(()
                => new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, null));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenEmptyListOfRequirementsGiven()
            => Assert.ThrowsException<Exception>(()
                => new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, new List<Requirement>()));

        [TestMethod]
        public void SetStep_ShouldSetStepId()
        {
            var newStep = new Mock<Step>();
            newStep.SetupGet(x => x.Id).Returns(4);
            _dutWithOneReqNotNeedInputTwoWeekInterval.SetStep(newStep.Object);

            Assert.AreEqual(newStep.Object.Id, _dutWithOneReqNotNeedInputTwoWeekInterval.StepId);
        }

        [TestMethod]
        public void SetStep_ShouldThrowException_WhenStepNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dutWithOneReqNotNeedInputTwoWeekInterval.SetStep(null));

        [TestMethod]
        public void AddRequirement_ShouldThrowException_WhenRequirementNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dutWithOneReqNotNeedInputTwoWeekInterval.AddRequirement(null));

        [TestMethod]
        public void StartPreservation_ShouldSetStatusActive()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);

            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation(_utcNow);

            Assert.AreEqual(PreservationStatus.Active, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
        }

        [TestMethod]
        public void StartPreservation_ShouldSetCorrectNextDueDateOnEachRequirement()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_NoneNeedInput_DifferentIntervals);

            dut.StartPreservation(_utcNow);

            var expectedNextDueTimeFirstUtc = _utcNow.AddWeeks(TwoWeeksInterval);
            var expectedNextDueTimeLaterUtc = _utcNow.AddWeeks(ThreeWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeFirstUtc, dut.Requirements.ElementAt(0).NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeLaterUtc, dut.Requirements.ElementAt(1).NextDueTimeUtc);
        }
        
        [TestMethod]
        public void IsReadyToBePreserved_ShouldBeFalse_BeforePeriod()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation(_utcNow);

            Assert.IsFalse(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved(_utcNow));
        }
        
        [TestMethod]
        public void IsReadyToBePreserved_ShouldBeTrue_InPeriod_WhenNotNeedInput()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation(_utcNow);
            
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved(_dueTimeForTwoWeeksInterval));
        }
        
        [TestMethod]
        public void IsReadyToBePreserved_ShouldBeTrue_OnOverdue_WhenNotNeedInput()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation(_utcNow);

            var overDue = _utcNow.AddWeeks(TwoWeeksInterval + TwoWeeksInterval);
            Assert.IsTrue(_dutWithOneReqNotNeedInputTwoWeekInterval.IsReadyToBePreserved(overDue));
        }

        [TestMethod]
        public void IsReadyToBePreserved_ShouldBeFalse_InPeriod_WhenNeedInput()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _oneReq_NeedInputTwoWeekInterval);
            Assert.AreEqual(PreservationStatus.NotStarted, dut.Status);

            dut.StartPreservation(_utcNow);

            Assert.IsFalse(dut.IsReadyToBePreserved(_dueTimeForTwoWeeksInterval));
        }

        [TestMethod]
        public void FirstUpcomingRequirement_ShouldNotGiveRequirement_BeforeDue()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);

            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation(_utcNow);

            Assert.IsNull(_dutWithOneReqNotNeedInputTwoWeekInterval.FirstUpcomingRequirement(_utcNow));
        }

        [TestMethod]
        public void FirstUpcomingRequirement_ShouldGiveRequirement_OnDue()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);

            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation(_utcNow);

            var firstUpcomingRequirement = _dutWithOneReqNotNeedInputTwoWeekInterval.FirstUpcomingRequirement(_dueTimeForTwoWeeksInterval);
            Assert.AreEqual(_reqNotNeedInputTwoWeekInterval, firstUpcomingRequirement);
        }

        [TestMethod]
        public void FirstUpcomingRequirement_ShouldGiveRequirement_OverDue()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInputTwoWeekInterval.Status);

            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation(_utcNow);

            var firstUpcomingRequirement = _dutWithOneReqNotNeedInputTwoWeekInterval.FirstUpcomingRequirement(_dueTimeForTwoWeeksInterval);
            Assert.AreEqual(_reqNotNeedInputTwoWeekInterval, firstUpcomingRequirement);
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservingBeforeTime()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation(_utcNow);
                   
            Assert.ThrowsException<Exception>(() 
                => _dutWithOneReqNotNeedInputTwoWeekInterval.Preserve(_utcNow, _person));
     
        }

        [TestMethod]
        public void Preserve_ShouldPreserve_WhenPreservingOnDue()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation(_utcNow);
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
            
            _dutWithOneReqNotNeedInputTwoWeekInterval.Preserve(_dueTimeForTwoWeeksInterval, _person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
        }

        [TestMethod]
        public void Preserve_ShouldPreserve_WhenPreservingOverDue()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation(_utcNow);
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
            
            _dutWithOneReqNotNeedInputTwoWeekInterval.Preserve(_dueTimeForThreeWeeksInterval, _person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
        }
        
        [TestMethod]
        public void Preserve_ShouldPreserveOnce_WhenPreservingMultipleTimesAtSameTime()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation(_utcNow);
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
            
            _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_dueTimeForTwoWeeksInterval, _person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            Assert.ThrowsException<Exception>(() 
                => _dutWithOneReqNotNeedInputTwoWeekInterval.Preserve(_dueTimeForTwoWeeksInterval, _person));
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenRequirementNeedsInput()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _oneReq_NeedInputTwoWeekInterval);
            dut.StartPreservation(_utcNow);

            Assert.ThrowsException<Exception>(() => dut.Preserve(_dueTimeForTwoWeeksInterval, _person));
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenFirstUpcomingRequirementNeedsInput()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation(_utcNow);

            Assert.ThrowsException<Exception>(() => dut.Preserve(_dueTimeForTwoWeeksInterval, _person));
        }
        
        [TestMethod]
        public void Preserve_ShouldThrowException_WhenFirstUpcomingRequirementNeedInput_BecauseFirstInListIsVoided()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation(_utcNow);
            dut.Requirements.ElementAt(0).Void();

            Assert.ThrowsException<Exception>(() => dut.Preserve(_dueTimeForThreeWeeksInterval, _person));
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservedByNotGiven()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_NoneNeedInput_DifferentIntervals);
            dut.StartPreservation(_utcNow);

            Assert.ThrowsException<ArgumentNullException>(() => dut.Preserve(_dueTimeForTwoWeeksInterval, null));
        }
        
        [TestMethod]
        public void Preserve_ShouldChangeUpComingRequirement()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation(_utcNow);
            Assert.AreEqual(_reqNotNeedInputTwoWeekInterval, dut.FirstUpcomingRequirement(_dueTimeForThreeWeeksInterval));

            dut.Preserve(_dueTimeForTwoWeeksInterval, _person);

            Assert.AreEqual(_reqNotNeedInputThreeWeekInterval, dut.FirstUpcomingRequirement(_dueTimeForThreeWeeksInterval));
        }
        
        [TestMethod]
        public void Preserve_ShouldPreserveDueRequirementsOnly()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation(_utcNow);

            var req1 = dut.Requirements.ElementAt(0);
            var req2 = dut.Requirements.ElementAt(1);
            Assert.AreEqual(1, req1.PreservationPeriods.Count);
            Assert.AreEqual(1, req2.PreservationPeriods.Count);

            dut.Preserve(_dueTimeForTwoWeeksInterval, _person);
            Assert.AreEqual(2, req1.PreservationPeriods.Count);
            Assert.AreEqual(1, req2.PreservationPeriods.Count);
        }

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenRequirementNeedInput()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _oneReq_NeedInputTwoWeekInterval);
            dut.StartPreservation(_utcNow);

            Assert.ThrowsException<Exception>(() => dut.BulkPreserve(_dueTimeForTwoWeeksInterval, _person));
        }

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenFirstUpcomingRequirementNeedInput()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation(_utcNow);

            Assert.ThrowsException<Exception>(() => dut.BulkPreserve(_dueTimeForTwoWeeksInterval, _person));
        }

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenFirstUpcomingRequirementNeedInput_BecauseFirstInListIsVoided()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation(_utcNow);
            dut.Requirements.ElementAt(0).Void();

            Assert.ThrowsException<Exception>(() => dut.BulkPreserve(_dueTimeForThreeWeeksInterval, _person));
        }

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenPreservedByNotGiven()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_NoneNeedInput_DifferentIntervals);
            dut.StartPreservation(_utcNow);

            Assert.ThrowsException<ArgumentNullException>(() => dut.BulkPreserve(_dueTimeForTwoWeeksInterval, null));
        }

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenPreservingBeforeTime()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation(_utcNow);
            
            Assert.ThrowsException<Exception>(() 
                => _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_utcNow, _person));
        }

        [TestMethod]
        public void BulkPreserve_ShouldPreserve_WhenPreservingOnDue()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation(_utcNow);
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
            
            _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_dueTimeForTwoWeeksInterval, _person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
        }

        [TestMethod]
        public void BulkPreserve_ShouldPreserve_WhenPreservingOverDue()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation(_utcNow);
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
            
            _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_dueTimeForThreeWeeksInterval, _person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
        }
        
        [TestMethod]
        public void BulkPreserve_ShouldPreserveOnce_WhenPreservingMultipleTimesAtSameTime()
        {
            _dutWithOneReqNotNeedInputTwoWeekInterval.StartPreservation(_utcNow);
            Assert.AreEqual(1, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);
            
            _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_dueTimeForTwoWeeksInterval, _person);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInputTwoWeekInterval.Requirements.First().PreservationPeriods.Count);

            Assert.ThrowsException<Exception>(() 
                => _dutWithOneReqNotNeedInputTwoWeekInterval.BulkPreserve(_dueTimeForTwoWeeksInterval, _person));
        }
        
        [TestMethod]
        public void BulkPreserve_ShouldPreserveDueRequirementsOnly()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation(_utcNow);

            var req1 = dut.Requirements.ElementAt(0);
            var req2 = dut.Requirements.ElementAt(1);
            Assert.AreEqual(1, req1.PreservationPeriods.Count);
            Assert.AreEqual(1, req2.PreservationPeriods.Count);

            dut.BulkPreserve(_dueTimeForTwoWeeksInterval, _person);
            Assert.AreEqual(2, req1.PreservationPeriods.Count);
            Assert.AreEqual(1, req2.PreservationPeriods.Count);
        }
        
        [TestMethod]
        public void BulkPreserve_ShouldChangeUpComingRequirement()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation(_utcNow);
            Assert.AreEqual(_reqNotNeedInputTwoWeekInterval, dut.FirstUpcomingRequirement(_dueTimeForTwoWeeksInterval));

            dut.BulkPreserve(_dueTimeForTwoWeeksInterval, _person);

            Assert.AreEqual(_reqNotNeedInputThreeWeekInterval, dut.FirstUpcomingRequirement(_dueTimeForThreeWeeksInterval));
        }
        
        [TestMethod]
        public void GetUpComingRequirements_ShouldReturnNoneRequirements_BeforePreservationStarted()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);

            Assert.AreEqual(0, dut.GetUpComingRequirements(_dueTimeForThreeWeeksInterval).Count());
        }
        
        [TestMethod]
        public void GetUpComingRequirements_ShouldReturnNoneRequirements_BeforeDue()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);

            dut.StartPreservation(_utcNow);
            Assert.AreEqual(0, dut.GetUpComingRequirements(_utcNow).Count());
        }
        
        [TestMethod]
        public void GetUpComingRequirements_ShouldReturnTwoReadyRequirements_WhenBothReadyAndDue()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation(_utcNow);

            Assert.AreEqual(2, dut.GetUpComingRequirements(_dueTimeForThreeWeeksInterval).Count());
        }
        
        [TestMethod]
        public void GetUpComingRequirements_ShouldReturnReadyRequirements_WhenOverDue()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation(_utcNow);

            Assert.AreEqual(1, dut.GetUpComingRequirements(_dueTimeForThreeWeeksInterval).Count());
        }
        
        [TestMethod]
        public void GetUpComingRequirements_ShouldReturnReadyRequirements_WhenDue()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation(_utcNow);

            Assert.AreEqual(1, dut.GetUpComingRequirements(_dueTimeForThreeWeeksInterval).Count());
        }
        
        [TestMethod]
        public void GetUpComingRequirements_ShouldNotReturnVoidedRequirements()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNotNeedInputThreeWeekInterval);
            dut.StartPreservation(_utcNow);
            dut.Requirements.ElementAt(0).Void();

            Assert.AreEqual(1, dut.GetUpComingRequirements(_dueTimeForThreeWeeksInterval).Count());
        }
        
        [TestMethod]
        public void OrderedRequirements_ShouldReturnAllRequirements_BeforeAndAfterPreservationStarted()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);

            Assert.AreEqual(2, dut.OrderedRequirements().Count());
        
            dut.StartPreservation(_utcNow);
            
            Assert.AreEqual(2, dut.OrderedRequirements().Count());
        }
        
        [TestMethod]
        public void OrderedRequirements_ShouldNotReturnVoidedRequirements()
        {
            var dut = new Tag("", TagType.Standard, "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputThreeWeekInterval);
            dut.StartPreservation(_utcNow);
            dut.Requirements.ElementAt(0).Void();

            Assert.AreEqual(1, dut.OrderedRequirements().Count());
        }
        
        [TestMethod]
        public void AddAction_ShouldAddAction()
        {
            var action = new Action("", "", "", _utcNow, _person, null);
            _dutWithOneReqNotNeedInputTwoWeekInterval.AddAction(action);

            Assert.AreEqual(action, _dutWithOneReqNotNeedInputTwoWeekInterval.Actions.First());
        }

        [TestMethod]
        public void AddAction_ShouldThrowException_WhenActionNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dutWithOneReqNotNeedInputTwoWeekInterval.AddAction(null));    }
}

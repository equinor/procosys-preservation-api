using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class TagTests
    {
        private const int TwoWeeksInterval = 2;
        private const int FourWeeksInterval = 4;
        private Mock<Step> _stepMock;
        
        private RequirementDefinition _reqDef1NotNeedInput;
        private RequirementDefinition _reqDef2NotNeedInput;
        private RequirementDefinition _reqDef1NeedInput;
        private RequirementDefinition _reqDef2NeedInput;
        private Requirement _reqNotNeedInputTwoWeekInterval;
        private Requirement _reqNotNeedInputFourWeekInterval;
        private Requirement _reqNeedInputTwoWeekInterval;
        private Requirement _reqNeedInputFourWeekInterval;
        private List<Requirement> _twoReqs_NoneNeedInput_DifferentIntervals;
        private List<Requirement> _oneReq_NotNeedInputTwoWeekInterval;
        private List<Requirement> _oneReq_NeedInputTwoWeekInterval;
        private List<Requirement> _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputFourWeekInterval;
        private List<Requirement> _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputFourWeekInterval;
        
        private DateTime _utcNow;
        private DateTime _dueTimeForTwoWeeksInterval;
        private DateTime _dueTimeForFourWeeksInterval;
        private Tag _dutWithOneReqNotNeedInput;

        [TestInitialize]
        public void Setup()
        {
            _stepMock = new Mock<Step>();
            _stepMock.SetupGet(x => x.Id).Returns(3);

            _reqDef1NotNeedInput = new RequirementDefinition("", "", 2, 0);
            _reqDef1NotNeedInput.AddField(new Field("", "", FieldType.Info, 0));
            _reqDef2NotNeedInput = new RequirementDefinition("", "", 2, 0);
            _reqDef2NotNeedInput.AddField(new Field("", "", FieldType.Info, 0));
            _reqDef1NeedInput = new RequirementDefinition("", "", 1, 0);
            _reqDef1NeedInput.AddField(new Field("", "", FieldType.CheckBox, 0));
            _reqDef2NeedInput = new RequirementDefinition("", "", 1, 0);
            _reqDef2NeedInput.AddField(new Field("", "", FieldType.CheckBox, 0));
            
            _reqNotNeedInputTwoWeekInterval = new Requirement("", TwoWeeksInterval, _reqDef1NotNeedInput);
            _reqNotNeedInputFourWeekInterval = new Requirement("", FourWeeksInterval, _reqDef2NotNeedInput);
            _reqNeedInputTwoWeekInterval = new Requirement("", TwoWeeksInterval, _reqDef1NeedInput);
            _reqNeedInputFourWeekInterval = new Requirement("", FourWeeksInterval, _reqDef2NeedInput);

            _twoReqs_NoneNeedInput_DifferentIntervals = new List<Requirement>
            {
                _reqNotNeedInputTwoWeekInterval, _reqNotNeedInputFourWeekInterval
            };

            _oneReq_NotNeedInputTwoWeekInterval = new List<Requirement>
            {
                _reqNotNeedInputTwoWeekInterval
            };

            _oneReq_NeedInputTwoWeekInterval = new List<Requirement>
            {
                _reqNeedInputTwoWeekInterval
            };

            _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputFourWeekInterval = new List<Requirement>
            {
                _reqNeedInputTwoWeekInterval, _reqNotNeedInputFourWeekInterval
            };

            _twoReqs_FirstNotNeedInputTwoWeekInterval_SecondNeedInputFourWeekInterval = new List<Requirement>
            {
                _reqNotNeedInputTwoWeekInterval, _reqNeedInputFourWeekInterval
            };

            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _dueTimeForTwoWeeksInterval = _utcNow.AddWeeks(TwoWeeksInterval);
            _dueTimeForFourWeeksInterval = _utcNow.AddWeeks(FourWeeksInterval);

            _dutWithOneReqNotNeedInput = new Tag("SchemaA",
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
            Assert.AreEqual("SchemaA", _dutWithOneReqNotNeedInput.Schema);
            Assert.AreEqual("TagNoA", _dutWithOneReqNotNeedInput.TagNo);
            Assert.AreEqual("DescA", _dutWithOneReqNotNeedInput.Description);
            Assert.AreEqual("AreaCodeA", _dutWithOneReqNotNeedInput.AreaCode);
            Assert.AreEqual("CalloffA", _dutWithOneReqNotNeedInput.Calloff);
            Assert.AreEqual("DisciplineA", _dutWithOneReqNotNeedInput.DisciplineCode);
            Assert.AreEqual("McPkgA", _dutWithOneReqNotNeedInput.McPkgNo);
            Assert.AreEqual("PurchaseOrderA", _dutWithOneReqNotNeedInput.PurchaseOrderNo);
            Assert.AreEqual("RemarkA", _dutWithOneReqNotNeedInput.Remark);
            Assert.AreEqual("TagFunctionCodeA", _dutWithOneReqNotNeedInput.TagFunctionCode);
            Assert.AreEqual(_stepMock.Object.Id, _dutWithOneReqNotNeedInput.StepId);
            var requirements = _dutWithOneReqNotNeedInput.Requirements;
            Assert.AreEqual(1, requirements.Count);
            
            var req = _dutWithOneReqNotNeedInput.Requirements.ElementAt(0);
            Assert.IsNull(req.NextDueTimeUtc);
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInput.Status);
        }
        
        [TestMethod]
        public void Constructor_ShouldNotSetReadyToBePreserved()
        {
            Assert.IsFalse(_dutWithOneReqNotNeedInput.ReadyToBePreserved);
        }
        
        [TestMethod]
        public void Constructor_ShouldNotSetReadyToBeBulkPreserved()
        {
            Assert.IsFalse(_dutWithOneReqNotNeedInput.IsReadyToBeBulkPreserved(_dueTimeForTwoWeeksInterval));
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenStepNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(()
                => new Tag("", "", "", "", "", "", "", "", "", "", "", null, _twoReqs_NoneNeedInput_DifferentIntervals));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementsNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(()
                => new Tag("", "", "", "", "", "", "", "", "", "", "", _stepMock.Object, null));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenEmptyListOfRequirementsGiven()
            => Assert.ThrowsException<Exception>(()
                => new Tag("", "", "", "", "", "", "", "", "", "", "", _stepMock.Object, new List<Requirement>()));

        [TestMethod]
        public void SetStep_ShouldSetStepId()
        {
            var newStep = new Mock<Step>();
            newStep.SetupGet(x => x.Id).Returns(4);
            _dutWithOneReqNotNeedInput.SetStep(newStep.Object);

            Assert.AreEqual(newStep.Object.Id, _dutWithOneReqNotNeedInput.StepId);
        }

        [TestMethod]
        public void SetStep_ShouldThrowException_WhenStepNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dutWithOneReqNotNeedInput.SetStep(null));

        [TestMethod]
        public void AddRequirement_ShouldThrowException_WhenRequirementNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dutWithOneReqNotNeedInput.AddRequirement(null));

        [TestMethod]
        public void StartPreservation_ShouldSetStatusActive()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInput.Status);

            _dutWithOneReqNotNeedInput.StartPreservation(_utcNow);

            Assert.AreEqual(PreservationStatus.Active, _dutWithOneReqNotNeedInput.Status);
        }

        [TestMethod]
        public void StartPreservation_ShouldSetCorrectNextDueDateOnEachRequirement()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_NoneNeedInput_DifferentIntervals);

            dut.StartPreservation(_utcNow);

            var expectedNextDueTimeFirstUtc = _utcNow.AddWeeks(TwoWeeksInterval);
            var expectedNextDueTimeLaterUtc = _utcNow.AddWeeks(FourWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeFirstUtc, dut.Requirements.ElementAt(0).NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeLaterUtc, dut.Requirements.ElementAt(1).NextDueTimeUtc);
        }
        
        [TestMethod]
        public void StartPreservation_ShouldSetReadyToBePreserved_WhenNoRequirementNeedInput()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInput.Status);

            _dutWithOneReqNotNeedInput.StartPreservation(_utcNow);

            Assert.IsTrue(_dutWithOneReqNotNeedInput.ReadyToBePreserved);
        }
        
        [TestMethod]
        public void IsReadyToBeBulkPreserved_ShouldBeFalse_BeforePeriod()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInput.Status);
            _dutWithOneReqNotNeedInput.StartPreservation(_utcNow);

            Assert.IsFalse(_dutWithOneReqNotNeedInput.IsReadyToBeBulkPreserved(_utcNow));
        }
        
        [TestMethod]
        public void IsReadyToBeBulkPreserved_ShouldBeTrue_InPeriod_WhenNotNeedInput()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInput.Status);
            _dutWithOneReqNotNeedInput.StartPreservation(_utcNow);
            
            Assert.IsTrue(_dutWithOneReqNotNeedInput.IsReadyToBeBulkPreserved(_dueTimeForTwoWeeksInterval));
        }
        
        [TestMethod]
        public void IsReadyToBeBulkPreserved_ShouldBeTrue_OnOverdue_WhenNotNeedInput()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInput.Status);
            _dutWithOneReqNotNeedInput.StartPreservation(_utcNow);

            var overDue = _utcNow.AddWeeks(TwoWeeksInterval + TwoWeeksInterval);
            Assert.IsTrue(_dutWithOneReqNotNeedInput.IsReadyToBeBulkPreserved(overDue));
        }

        [TestMethod]
        public void StartPreservation_ShouldNotSetReadyToBePreserved_WhenRequirementNeedInput()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _oneReq_NeedInputTwoWeekInterval);
            Assert.AreEqual(PreservationStatus.NotStarted, dut.Status);

            dut.StartPreservation(_utcNow);

            Assert.IsFalse(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void FirstUpcomingRequirement_ShouldGiveRequirement_WhenPreservationStarted()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dutWithOneReqNotNeedInput.Status);

            _dutWithOneReqNotNeedInput.StartPreservation(_utcNow);
            var firstUpcomingRequirement = _dutWithOneReqNotNeedInput.FirstUpcomingRequirement;

            Assert.IsNotNull(firstUpcomingRequirement);
        }

        [TestMethod]
        public void Preserve_ShouldPreserve_WhenPreservingBeforeTime()
        {
            _dutWithOneReqNotNeedInput.StartPreservation(_utcNow);
            Assert.AreEqual(1, _dutWithOneReqNotNeedInput.FirstUpcomingRequirement.PreservationPeriods.Count);
            Assert.IsTrue(_dutWithOneReqNotNeedInput.ReadyToBePreserved);
            
            _dutWithOneReqNotNeedInput.Preserve(_utcNow, new Mock<Person>().Object);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInput.FirstUpcomingRequirement.PreservationPeriods.Count);
        }

        [TestMethod]
        public void Preserve_ShouldPreserve_WhenPreservingOnDue()
        {
            _dutWithOneReqNotNeedInput.StartPreservation(_utcNow);
            Assert.AreEqual(1, _dutWithOneReqNotNeedInput.FirstUpcomingRequirement.PreservationPeriods.Count);
            Assert.IsTrue(_dutWithOneReqNotNeedInput.ReadyToBePreserved);
            
            _dutWithOneReqNotNeedInput.Preserve(_dueTimeForTwoWeeksInterval, new Mock<Person>().Object);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInput.FirstUpcomingRequirement.PreservationPeriods.Count);
        }

        [TestMethod]
        public void Preserve_ShouldPreserve_WhenPreservingOverDue()
        {
            _dutWithOneReqNotNeedInput.StartPreservation(_utcNow);
            Assert.AreEqual(1, _dutWithOneReqNotNeedInput.FirstUpcomingRequirement.PreservationPeriods.Count);
            Assert.IsTrue(_dutWithOneReqNotNeedInput.ReadyToBePreserved);
            
            _dutWithOneReqNotNeedInput.Preserve(_dueTimeForFourWeeksInterval, new Mock<Person>().Object);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInput.FirstUpcomingRequirement.PreservationPeriods.Count);
        }
        
        [TestMethod]
        public void Preserve_ShouldPreserve_WhenPreservingMultipleTimesAnyTime()
        {
            _dutWithOneReqNotNeedInput.StartPreservation(_utcNow);
            Assert.AreEqual(1, _dutWithOneReqNotNeedInput.FirstUpcomingRequirement.PreservationPeriods.Count);
            Assert.IsTrue(_dutWithOneReqNotNeedInput.ReadyToBePreserved);
            
            _dutWithOneReqNotNeedInput.Preserve(_utcNow, new Mock<Person>().Object);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInput.FirstUpcomingRequirement.PreservationPeriods.Count);
            _dutWithOneReqNotNeedInput.Preserve(_utcNow, new Mock<Person>().Object);
            Assert.AreEqual(3, _dutWithOneReqNotNeedInput.FirstUpcomingRequirement.PreservationPeriods.Count);
            _dutWithOneReqNotNeedInput.Preserve(_utcNow, new Mock<Person>().Object);
            Assert.AreEqual(4, _dutWithOneReqNotNeedInput.FirstUpcomingRequirement.PreservationPeriods.Count);
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenRequirementNeedInput()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _oneReq_NeedInputTwoWeekInterval);
            dut.StartPreservation(_utcNow);
            Assert.IsFalse(dut.ReadyToBePreserved);

            Assert.ThrowsException<Exception>(() => dut.Preserve(_dueTimeForTwoWeeksInterval, new Mock<Person>().Object));
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenFirstUpcomingRequirementNeedInput()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputFourWeekInterval);
            dut.StartPreservation(_utcNow);
            Assert.IsFalse(dut.ReadyToBePreserved);

            Assert.ThrowsException<Exception>(() => dut.Preserve(_dueTimeForTwoWeeksInterval, new Mock<Person>().Object));
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservedByNotGiven()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_NoneNeedInput_DifferentIntervals);
            dut.StartPreservation(_utcNow);
            Assert.IsTrue(dut.ReadyToBePreserved);

            Assert.ThrowsException<ArgumentNullException>(() => dut.Preserve(_dueTimeForTwoWeeksInterval, null));
        }

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenRequirementNeedInput()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _oneReq_NeedInputTwoWeekInterval);
            dut.StartPreservation(_utcNow);
            Assert.IsFalse(dut.IsReadyToBeBulkPreserved(_dueTimeForTwoWeeksInterval));

            Assert.ThrowsException<Exception>(() => dut.BulkPreserve(_dueTimeForTwoWeeksInterval, new Mock<Person>().Object));
        }

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenFirstUpcomingRequirementNeedInput()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_FirstNeedInputTwoWeekInterval_SecondNotNeedInputFourWeekInterval);
            dut.StartPreservation(_utcNow);
            Assert.IsFalse(dut.IsReadyToBeBulkPreserved(_dueTimeForTwoWeeksInterval));

            Assert.ThrowsException<Exception>(() => dut.BulkPreserve(_utcNow, new Mock<Person>().Object));
        }

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenPreservedByNotGiven()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", "", _stepMock.Object, _twoReqs_NoneNeedInput_DifferentIntervals);
            dut.StartPreservation(_utcNow);
            Assert.IsTrue(dut.IsReadyToBeBulkPreserved(_dueTimeForTwoWeeksInterval));

            Assert.ThrowsException<ArgumentNullException>(() => dut.BulkPreserve(_dueTimeForTwoWeeksInterval, null));
        }

        [TestMethod]
        public void BulkPreserve_ShouldThrowException_WhenPreservingBeforeTime()
        {
            _dutWithOneReqNotNeedInput.StartPreservation(_utcNow);
            Assert.IsFalse(_dutWithOneReqNotNeedInput.IsReadyToBeBulkPreserved(_utcNow));
            
            Assert.ThrowsException<Exception>(() 
                => _dutWithOneReqNotNeedInput.BulkPreserve(_utcNow, new Mock<Person>().Object));
        }

        [TestMethod]
        public void BulkPreserve_ShouldPreserve_WhenPreservingOnDue()
        {
            _dutWithOneReqNotNeedInput.StartPreservation(_utcNow);
            Assert.AreEqual(1, _dutWithOneReqNotNeedInput.FirstUpcomingRequirement.PreservationPeriods.Count);
            Assert.IsTrue(_dutWithOneReqNotNeedInput.IsReadyToBeBulkPreserved(_dueTimeForTwoWeeksInterval));
            
            _dutWithOneReqNotNeedInput.BulkPreserve(_dueTimeForTwoWeeksInterval, new Mock<Person>().Object);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInput.FirstUpcomingRequirement.PreservationPeriods.Count);
        }

        [TestMethod]
        public void BulkPreserve_ShouldPreserve_WhenPreservingOverDue()
        {
            _dutWithOneReqNotNeedInput.StartPreservation(_utcNow);
            Assert.AreEqual(1, _dutWithOneReqNotNeedInput.FirstUpcomingRequirement.PreservationPeriods.Count);
            Assert.IsTrue(_dutWithOneReqNotNeedInput.IsReadyToBeBulkPreserved(_dueTimeForTwoWeeksInterval));
            
            _dutWithOneReqNotNeedInput.BulkPreserve(_dueTimeForFourWeeksInterval, new Mock<Person>().Object);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInput.FirstUpcomingRequirement.PreservationPeriods.Count);
        }
        
        [TestMethod]
        public void BulkPreserve_ShoulPreserveOnce_WhenPreservingMultipleTimesAtSameTime()
        {
            _dutWithOneReqNotNeedInput.StartPreservation(_utcNow);
            Assert.AreEqual(1, _dutWithOneReqNotNeedInput.FirstUpcomingRequirement.PreservationPeriods.Count);
            Assert.IsTrue(_dutWithOneReqNotNeedInput.IsReadyToBeBulkPreserved(_dueTimeForTwoWeeksInterval));
            
            _dutWithOneReqNotNeedInput.BulkPreserve(_dueTimeForTwoWeeksInterval, new Mock<Person>().Object);
            Assert.AreEqual(2, _dutWithOneReqNotNeedInput.FirstUpcomingRequirement.PreservationPeriods.Count);

            Assert.ThrowsException<Exception>(() 
                => _dutWithOneReqNotNeedInput.BulkPreserve(_dueTimeForTwoWeeksInterval, new Mock<Person>().Object));
        }
    }
}

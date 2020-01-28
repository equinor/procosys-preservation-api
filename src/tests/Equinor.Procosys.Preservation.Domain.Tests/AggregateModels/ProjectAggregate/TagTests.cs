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
        private const int IntervalWeeksFirst = 2;
        private const int IntervalWeeksLater = 4;
        private Mock<Step> _stepMock;
        
        private RequirementDefinition _reqDefFirstNotNeedInput;
        private RequirementDefinition _reqDefLaterNotNeedInput;
        private RequirementDefinition _reqDefNeedInput;
        private Requirement _reqFirstNotNeedInput;
        private Requirement _reqLaterNotNeedInput;
        private Requirement _reqNeedInput;
        private List<Requirement> _reqsNotNeedInput;
        private List<Requirement> _reqsNeedInput;
        
        private DateTime _utcNow;
        private Tag _dut;

        [TestInitialize]
        public void Setup()
        {
            _stepMock = new Mock<Step>();
            _stepMock.SetupGet(x => x.Id).Returns(3);

            _reqDefFirstNotNeedInput = new RequirementDefinition("", "", 2, 0);
            _reqDefFirstNotNeedInput.AddField(new Field("", "", FieldType.Info, 0));
            _reqDefLaterNotNeedInput = new RequirementDefinition("", "", 2, 0);
            _reqDefLaterNotNeedInput.AddField(new Field("", "", FieldType.Info, 0));
            _reqDefNeedInput = new RequirementDefinition("", "", 1, 0);
            _reqDefNeedInput.AddField(new Field("", "", FieldType.CheckBox, 0));
            
            _reqFirstNotNeedInput = new Requirement("", IntervalWeeksFirst, _reqDefFirstNotNeedInput);
            _reqLaterNotNeedInput = new Requirement("", IntervalWeeksLater, _reqDefLaterNotNeedInput);
            _reqNeedInput = new Requirement("", 8, _reqDefNeedInput);

            _reqsNotNeedInput = new List<Requirement>
            {
                _reqFirstNotNeedInput, _reqLaterNotNeedInput
            };

            _reqsNeedInput = new List<Requirement>
            {
                _reqNeedInput
            };

            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);

            _dut = new Tag("SchemaA",
                "TagNoA",
                "DescA", 
                "AreaCodeA", 
                "CalloffA", 
                "DisciplineA", 
                "McPkgA", 
                "CommPkgA", 
                "PurchaseOrderA", 
                "TagFunctionCodeA", 
                _stepMock.Object,
                _reqsNotNeedInput);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual("SchemaA", _dut.Schema);
            Assert.AreEqual("TagNoA", _dut.TagNo);
            Assert.AreEqual("DescA", _dut.Description);
            Assert.AreEqual("AreaCodeA", _dut.AreaCode);
            Assert.AreEqual("CalloffA", _dut.Calloff);
            Assert.AreEqual("DisciplineA", _dut.DisciplineCode);
            Assert.AreEqual("McPkgA", _dut.McPkgNo);
            Assert.AreEqual("PurchaseOrderA", _dut.PurchaseOrderNo);
            Assert.AreEqual("TagFunctionCodeA", _dut.TagFunctionCode);
            Assert.AreEqual(_stepMock.Object.Id, _dut.StepId);
            var requirements = _dut.Requirements;
            Assert.AreEqual(2, requirements.Count);
            var firstReq = requirements.ElementAt(0);
            var laterReq = requirements.ElementAt(1);
            Assert.IsNull(firstReq.NextDueTimeUtc);
            Assert.IsNull(laterReq.NextDueTimeUtc);
            Assert.AreEqual(PreservationStatus.NotStarted, _dut.Status);
        }
        
        [TestMethod]
        public void Constructor_ShouldNotMakeTagReadyToBePreserved()
        {
            Assert.IsFalse(_dut.ReadyToBePreserved);
            var requirements = _dut.Requirements;
            var firstReq = requirements.ElementAt(0);
            var laterReq = requirements.ElementAt(1);
            Assert.IsFalse(firstReq.HasPeriodReadyToBePreserved);
            Assert.IsFalse(laterReq.HasPeriodReadyToBePreserved);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenStepNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(()
                => new Tag("", "", "", "", "", "", "", "", "", "", null, _reqsNotNeedInput));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementsNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(()
                => new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, null));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenEmptyListOfRequirementsGiven()
            => Assert.ThrowsException<Exception>(()
                => new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, new List<Requirement>()));

        [TestMethod]
        public void SetStep_ShouldSetStepId()
        {
            var newStep = new Mock<Step>();
            newStep.SetupGet(x => x.Id).Returns(4);
            _dut.SetStep(newStep.Object);

            Assert.AreEqual(newStep.Object.Id, _dut.StepId);
        }

        [TestMethod]
        public void SetStep_ShouldThrowException_WhenStepNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dut.SetStep(null));

        [TestMethod]
        public void AddRequirement_ShouldThrowException_WhenRequirementNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dut.AddRequirement(null));

        [TestMethod]
        public void StartPreservation_ShouldSetStatusActive()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dut.Status);

            _dut.StartPreservation(_utcNow);

            Assert.AreEqual(PreservationStatus.Active, _dut.Status);
        }

        [TestMethod]
        public void StartPreservation_ShouldShouldSetCorrectNextDueDateOnEachRequirement()
        {
            _dut.StartPreservation(_utcNow);

            var expectedNextDueTimeFirstUtc = _utcNow.AddWeeks(IntervalWeeksFirst);
            var expectedNextDueTimeLaterUtc = _utcNow.AddWeeks(IntervalWeeksLater);
            Assert.AreEqual(expectedNextDueTimeFirstUtc, _dut.Requirements.ElementAt(0).NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeLaterUtc, _dut.Requirements.ElementAt(1).NextDueTimeUtc);
        }
        
        [TestMethod]
        public void StartPreservation_ShouldMakeTagReadyToBePreserved_WhenNoRequirementNeedInput()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dut.Status);

            _dut.StartPreservation(_utcNow);

            Assert.IsTrue(_dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void StartPreservation_ShouldNotMakeTagReadyToBePreserved_WhenRequirementNeedInput()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, _reqsNeedInput);
            Assert.AreEqual(PreservationStatus.NotStarted, dut.Status);

            dut.StartPreservation(_utcNow);

            Assert.IsFalse(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void FirstUpcomingRequirement_ShouldNotGiveRequirement_WhenPreservationNotStarted()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dut.Status);

            var firstUpcomingRequirement = _dut.FirstUpcomingRequirement;

            Assert.IsNull(firstUpcomingRequirement);
        }

        [TestMethod]
        public void FirstUpcomingRequirement_ShouldGiveRequirement_WhenPreservationStarted()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dut.Status);

            _dut.StartPreservation(_utcNow);
            var firstUpcomingRequirement = _dut.FirstUpcomingRequirement;

            Assert.IsNotNull(firstUpcomingRequirement);
        }

        [TestMethod]
        public void FirstUpcomingRequirement_ShouldGiveCorrectRequirement_WhenDifferentInterval()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dut.Status);

            _dut.StartPreservation(_utcNow);
            var firstUpcomingRequirement = _dut.FirstUpcomingRequirement;

            Assert.AreEqual(firstUpcomingRequirement, _dut.Requirements.ElementAt(0));
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenRequirementNeedInput()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, _reqsNeedInput);
            dut.StartPreservation(_utcNow);
            Assert.IsFalse(dut.ReadyToBePreserved);

            Assert.ThrowsException<Exception>(() => dut.Preserve(_utcNow, new Mock<Person>().Object, false));
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservedByNotGiven()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, _reqsNotNeedInput);
            dut.StartPreservation(_utcNow);

            Assert.ThrowsException<ArgumentNullException>(() => dut.Preserve(_utcNow, null, false));
        }
    }
}

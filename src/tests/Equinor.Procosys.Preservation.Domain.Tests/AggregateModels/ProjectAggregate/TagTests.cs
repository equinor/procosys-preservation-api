using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class TagTests
    {
        private const int ReqFirstId = 7;
        private const int ReqLaterId = 17;
        private const int IntervalWeeksFirst = 2;
        private const int IntervalWeeksLater = 4;
        private Mock<Step> _stepMock;
        private Mock<Requirement> _reqFirstMock;
        private Mock<Requirement> _reqLaterMock;
        private List<Requirement> _requirements;
        private DateTime _utcNow;
        private Tag _dut;

        [TestInitialize]
        public void Setup()
        {
            _stepMock = new Mock<Step>();
            _stepMock.SetupGet(x => x.Id).Returns(3);
            _reqFirstMock = new Mock<Requirement>("", IntervalWeeksFirst, new Mock<RequirementDefinition>().Object);
            _reqFirstMock.SetupGet(x => x.Id).Returns(ReqFirstId);
            _reqLaterMock = new Mock<Requirement>("", IntervalWeeksLater, new Mock<RequirementDefinition>().Object);
            _reqLaterMock.SetupGet(x => x.Id).Returns(ReqLaterId);

            _requirements = new List<Requirement>
            {
                _reqFirstMock.Object, _reqLaterMock.Object
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
                _requirements);
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
            Assert.AreEqual(_reqFirstMock.Object.Id, firstReq.Id);
            Assert.AreEqual(_reqLaterMock.Object.Id, laterReq.Id);
            Assert.IsNull(firstReq.NextDueTimeUtc);
            Assert.IsNull(laterReq.NextDueTimeUtc);
            Assert.AreEqual(PreservationStatus.NotStarted, _dut.Status);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenStepNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(()
                => new Tag("", "", "", "", "", "", "", "", "", "", null, _requirements));

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
        public void FirstUpcommingRequirement_ShouldNotGiveRequirement_WhenPreservationNotStarted()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dut.Status);

            var firstUpcommingRequirement = _dut.FirstUpcommingRequirement;

            Assert.IsNull(firstUpcommingRequirement);
        }

        [TestMethod]
        public void FirstUpcommingRequirement_ShouldGiveRequirement_WhenPreservationStarted()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dut.Status);

            _dut.StartPreservation(_utcNow);
            var firstUpcommingRequirement = _dut.FirstUpcommingRequirement;

            Assert.IsNotNull(firstUpcommingRequirement);
        }

        [TestMethod]
        public void FirstUpcommingRequirement_ShouldGiveCorrectRequirement_WhenDifferentInterval()
        {
            Assert.AreEqual(PreservationStatus.NotStarted, _dut.Status);

            _dut.StartPreservation(_utcNow);
            var firstUpcommingRequirement = _dut.FirstUpcommingRequirement;

            Assert.AreEqual(firstUpcommingRequirement, _dut.Requirements.ElementAt(0));
        }
    }
}

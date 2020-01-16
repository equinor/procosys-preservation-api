using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.TagAggregate
{
    [TestClass]
    public class PreservationRecordTests
    {
        private DateTime _utcNow;
        private Mock<Requirement> _reqMock;
        private int _preservedById = 99;
        private Mock<Person> _preservedByMock;

        [TestInitialize]
        public void Setup()
        {
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _reqMock = new Mock<Requirement>();
            _preservedByMock = new Mock<Person>();
            _preservedByMock.SetupGet(p => p.Id).Returns(_preservedById);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            _reqMock.SetupGet(r => r.Id).Returns(3);

            var dut = new PreservationRecord("SchemaA", _reqMock.Object, _utcNow);

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual(_reqMock.Object.Id, dut.RequirementId);
            Assert.IsFalse(dut.BulkPreserved.HasValue);
            Assert.IsFalse(dut.PreservedAtUtc.HasValue);
            Assert.IsFalse(dut.PreservedBy.HasValue);
        }
        [TestMethod]

        public void Constructor_ShouldSetCorrectNextDueDateBasedOnRequirement()
        {
            var rdMock = new Mock<RequirementDefinition>();
            var intervalWeeks = 8;
            var r = new Requirement("SchemaA", intervalWeeks, rdMock.Object);

            var dut = new PreservationRecord("SchemaA", r, _utcNow);

            var expectedNextDueTimeUtc = _utcNow.AddDays(7 * intervalWeeks);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.NextDueTimeUtc);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new PreservationRecord("SchemaA", null, _utcNow)
            );

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenCurrentTimeNotGivenInUtc()
        {
            var now = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Local);
            Assert.ThrowsException<ArgumentException>(() =>
                new PreservationRecord("SchemaA", _reqMock.Object, now)
            );
        }

        [TestMethod]
        public void WhenPreserve_ShouldSetPreservedNowByPerson()
        {
            var dut = new PreservationRecord("SchemaA", _reqMock.Object, _utcNow);

            dut.Preserve(_preservedByMock.Object, "", _utcNow);

            Assert.IsTrue(dut.PreservedBy.HasValue);
            Assert.AreEqual(_preservedById, dut.PreservedBy.Value);
            Assert.IsTrue(dut.PreservedAtUtc.HasValue);
            Assert.AreEqual(_utcNow, dut.PreservedAtUtc.Value);
        }

        [TestMethod]
        public void WhenPreserve_ShouldNotSetBulkPreservedButWithComment()
        {
            var dut = new PreservationRecord("SchemaA", _reqMock.Object, _utcNow);

            dut.Preserve(_preservedByMock.Object, "Comment", _utcNow);

            Assert.AreEqual("Comment", dut.Comment);
            Assert.IsTrue(dut.BulkPreserved.HasValue);
            Assert.IsFalse(dut.BulkPreserved.Value);
        }

        [TestMethod]
        public void WhenBulkPreserve_ShouldSetPreservedNowByPerson()
        {
            var dut = new PreservationRecord("SchemaA", _reqMock.Object, _utcNow);

            dut.BulkPreserve(_preservedByMock.Object, _utcNow);

            Assert.IsTrue(dut.PreservedBy.HasValue);
            Assert.AreEqual(_preservedById, dut.PreservedBy.Value);
            Assert.IsTrue(dut.PreservedAtUtc.HasValue);
            Assert.AreEqual(_utcNow, dut.PreservedAtUtc.Value);
        }

        [TestMethod]
        public void WhenBulkPreserve_ShouldSetBulkPreservedAndNullComment()
        {
            var dut = new PreservationRecord("SchemaA", _reqMock.Object, _utcNow);

            dut.BulkPreserve(_preservedByMock.Object, _utcNow);

            Assert.IsNull(dut.Comment);
            Assert.IsTrue(dut.BulkPreserved.HasValue);
            Assert.IsTrue(dut.BulkPreserved.Value);
        }

        [TestMethod]
        public void WhenPreserve_ShouldThrowException_WhenPreservedByNotGivenInUtc()
        {
            var dut = new PreservationRecord("SchemaA", _reqMock.Object, _utcNow);
            Assert.ThrowsException<ArgumentNullException>(() => dut.Preserve(null, "", _utcNow));
        }

        [TestMethod]
        public void WhenPreserve_ShouldThrowException_WhenCurrentTimeNotGivenInUtc()
        {
            var now = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Local);
            
            var dut = new PreservationRecord("SchemaA", _reqMock.Object, _utcNow);
            Assert.ThrowsException<ArgumentException>(() => dut.Preserve(_preservedByMock.Object, "", now));
        }

        [TestMethod]
        public void WhenBulkPreserve_ShouldThrowException_WhenPreservedByNotGivenInUtc()
        {
            var dut = new PreservationRecord("SchemaA", _reqMock.Object, _utcNow);
            Assert.ThrowsException<ArgumentNullException>(() => dut.BulkPreserve(null, _utcNow));
        }

        [TestMethod]
        public void WhenBulkPreserve_ShouldThrowException_WhenCurrentTimeNotGivenInUtc()
        {
            var now = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Local);
            
            var dut = new PreservationRecord("SchemaA", _reqMock.Object, _utcNow);
            Assert.ThrowsException<ArgumentException>(() => dut.BulkPreserve(_preservedByMock.Object, now));
        }
    }
}

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
        private Mock<ITimeService> _tsMock;
        private DateTime _utcNow;
        private Mock<Requirement> _reqMock;
        private int _preservedById = 99;
        private Mock<Person> _preservedByMock;

        [TestInitialize]
        public void Setup()
        {
            _utcNow = DateTime.UtcNow;
            _tsMock = new Mock<ITimeService>();
            _tsMock.Setup(s => s.GetCurrentTimeUtc()).Returns(_utcNow);
            _reqMock = new Mock<Requirement>();
            _preservedByMock = new Mock<Person>();
            _preservedByMock.SetupGet(p => p.Id).Returns(_preservedById);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            _reqMock.SetupGet(r => r.Id).Returns(3);

            var dut = new PreservationRecord("SchemaA", _reqMock.Object, _tsMock.Object);

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

            var dut = new PreservationRecord("SchemaA", r, _tsMock.Object);

            var expectedNextDueTimeUtc = _utcNow.AddDays(7 * intervalWeeks);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.NextDueTimeUtc);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new PreservationRecord("SchemaA", null, _tsMock.Object)
            );

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenTimeServiceNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new PreservationRecord("SchemaA", _reqMock.Object, null)
            );

        [TestMethod]
        public void WhenPreserve_ShouldSetPreservedNowByPerson()
        {
            var dut = new PreservationRecord("SchemaA", _reqMock.Object, _tsMock.Object);

            dut.Preserve(_preservedByMock.Object, "", _tsMock.Object);

            Assert.IsTrue(dut.PreservedBy.HasValue);
            Assert.AreEqual(_preservedById, dut.PreservedBy.Value);
            Assert.IsTrue(dut.PreservedAtUtc.HasValue);
            Assert.AreEqual(_utcNow, dut.PreservedAtUtc.Value);
        }

        [TestMethod]
        public void WhenPreserve_ShouldNotSetBulkPreservedButWithComment()
        {
            var dut = new PreservationRecord("SchemaA", _reqMock.Object, _tsMock.Object);

            dut.Preserve(_preservedByMock.Object, "Comment", _tsMock.Object);

            Assert.AreEqual("Comment", dut.Comment);
            Assert.IsTrue(dut.BulkPreserved.HasValue);
            Assert.IsFalse(dut.BulkPreserved.Value);
        }

        [TestMethod]
        public void WhenBulkPreserve_ShouldSetPreservedNowByPerson()
        {
            var dut = new PreservationRecord("SchemaA", _reqMock.Object, _tsMock.Object);

            dut.BulkPreserve(_preservedByMock.Object, _tsMock.Object);

            Assert.IsTrue(dut.PreservedBy.HasValue);
            Assert.AreEqual(_preservedById, dut.PreservedBy.Value);
            Assert.IsTrue(dut.PreservedAtUtc.HasValue);
            Assert.AreEqual(_utcNow, dut.PreservedAtUtc.Value);
        }

        [TestMethod]
        public void WhenBulkPreserve_ShouldSetBulkPreservedAndNullComment()
        {
            var dut = new PreservationRecord("SchemaA", _reqMock.Object, _tsMock.Object);

            dut.BulkPreserve(_preservedByMock.Object, _tsMock.Object);

            Assert.IsNull(dut.Comment);
            Assert.IsTrue(dut.BulkPreserved.HasValue);
            Assert.IsTrue(dut.BulkPreserved.Value);
        }
    }
}

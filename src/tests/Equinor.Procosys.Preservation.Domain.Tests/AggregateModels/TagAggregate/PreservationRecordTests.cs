using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
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

            var dut = new PreservationRecord("SchemaA", _reqMock.Object, _utcNow, _preservedByMock.Object, true, "Comment");

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual(_reqMock.Object.Id, dut.RequirementId);
            Assert.AreEqual(_utcNow, dut.PreservedAtUtc);
            Assert.AreEqual(_preservedById, dut.PreservedByPersonId);
            Assert.IsTrue(dut.BulkPreserved);
            Assert.AreEqual("Comment", dut.Comment);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new PreservationRecord("SchemaA", null, _utcNow, _preservedByMock.Object, true, "Comment")
            );

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenPreservedByNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new PreservationRecord("SchemaA", _reqMock.Object, _utcNow, null, true, "Comment")
            );

    }
}

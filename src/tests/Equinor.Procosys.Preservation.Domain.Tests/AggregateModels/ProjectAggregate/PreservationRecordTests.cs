using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class PreservationRecordTests
    {
        private ManualTimeProvider _timeProvider;
        private int _preservedById = 99;
        private Mock<Person> _preservedByMock;

        [TestInitialize]
        public void Setup()
        {
            _timeProvider = new ManualTimeProvider();
            _timeProvider.UtcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _preservedByMock = new Mock<Person>();
            _preservedByMock.SetupGet(p => p.Id).Returns(_preservedById);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new PreservationRecord("SchemaA", _preservedByMock.Object, true);

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual(_timeProvider.UtcNow, dut.PreservedAtUtc);
            Assert.AreEqual(_preservedById, dut.PreservedById);
            Assert.IsTrue(dut.BulkPreserved);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenPreservedByNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new PreservationRecord("SchemaA", null, true)
            );

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenDateNotUtc()
            => Assert.ThrowsException<ArgumentException>(() =>
                new PreservationRecord("SchemaA", _preservedByMock.Object, true)
            );
    }
}

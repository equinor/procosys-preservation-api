using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class PreservationPeriodTests
    {
        private const int PreservedById = 31;
        private DateTime _utcNow;
        private Mock<Person> _preservedByMock;

        [TestInitialize]
        public void Setup()
        {
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _preservedByMock = new Mock<Person>();
            _preservedByMock.SetupGet(p => p.Id).Returns(PreservedById);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.ReadyToBePreserved);

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual(_utcNow, dut.DueTimeUtc);
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.Status);
            Assert.IsNull(dut.PreservationRecord);
        }

        [TestMethod]
        public void Constructor_ShouldAllowStatusNeedUserInput()
        {
            var dut = new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.NeedUserInput);

            Assert.AreEqual(PreservationPeriodStatus.NeedUserInput, dut.Status);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenStatusIsPreserved()
            => Assert.ThrowsException<ArgumentException>(() =>
                new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.Preserved)
            );

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenStatusIsStopped()
            => Assert.ThrowsException<ArgumentException>(() =>
                new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.Stopped)
            );

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenDateNotUtc()
            => Assert.ThrowsException<ArgumentException>(() =>
                new PreservationPeriod("SchemaA", DateTime.Now, PreservationPeriodStatus.NeedUserInput)
            );

        [TestMethod]
        public void Preserve_ShouldPreserve_WhenReadyToBePreserved()
        {
            var dut = new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.ReadyToBePreserved);
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.Status);
            Assert.IsNull(dut.PreservationRecord);

            // act
            var preservationTime = _utcNow.AddDays(12);
            dut.Preserve(preservationTime, _preservedByMock.Object, true);

            // assert
            var record = dut.PreservationRecord;
            Assert.IsNotNull(record);
            Assert.AreEqual(PreservationPeriodStatus.Preserved, dut.Status);
            Assert.AreEqual(PreservedById, record.PreservedByPersonId);
            Assert.AreEqual(preservationTime, record.PreservedAtUtc);
            Assert.IsTrue(record.BulkPreserved);
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreserveTwice()
        {
            var dut = new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.ReadyToBePreserved);
            Assert.IsNull(dut.PreservationRecord);
            var preservationTime = _utcNow.AddDays(12);
            dut.Preserve(preservationTime, _preservedByMock.Object, true);

            // act
            preservationTime = preservationTime.AddDays(1);
            Assert.ThrowsException<Exception>(() =>
                dut.Preserve(preservationTime, _preservedByMock.Object, true)
            );
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenNeedUserInput()
        {
            var dut = new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.NeedUserInput);
            Assert.AreEqual(PreservationPeriodStatus.NeedUserInput, dut.Status);

            // act
            Assert.ThrowsException<Exception>(() =>
                dut.Preserve(_utcNow, _preservedByMock.Object, true)
            );
        }
    }
}

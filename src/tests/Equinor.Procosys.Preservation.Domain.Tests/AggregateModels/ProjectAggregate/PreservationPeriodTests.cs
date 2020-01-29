using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
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
        public void Constructor_ShouldAllowStatusNeedsUserInput()
        {
            var dut = new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.NeedsUserInput);

            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.Status);
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
                new PreservationPeriod("SchemaA", DateTime.Now, PreservationPeriodStatus.NeedsUserInput)
            );
        
        [TestMethod]
        public void AddFieldValue_ShouldThrowException_WhenFieldValueNotGiven()
        {
            var dut = new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.ReadyToBePreserved);

            Assert.ThrowsException<ArgumentNullException>(() =>
                dut.AddFieldValue(null));
            Assert.AreEqual(0, dut.FieldValues.Count);
        }

        [TestMethod]
        public void AddFieldValue_ShouldAddFieldValueToFieldValuesList()
        {
            var dut = new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.ReadyToBePreserved);
            var fvMock = new Mock<FieldValue>();

            dut.AddFieldValue(fvMock.Object);

            Assert.AreEqual(1, dut.FieldValues.Count);
            Assert.IsTrue(dut.FieldValues.Contains(fvMock.Object));
        }
                
        [TestMethod]
        public void AddFieldValue_ShouldThrowException_AfterPreserved()
        {
            var dut = new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.ReadyToBePreserved);
            dut.Preserve(_utcNow, _preservedByMock.Object, true);

            Assert.ThrowsException<Exception>(() => dut.AddFieldValue(new Mock<FieldValue>().Object));
            Assert.AreEqual(0, dut.FieldValues.Count);
        }
        
        [TestMethod]
        public void RemoveFieldValue_ShouldRemoveFieldValueFromFieldValuesList()
        {
            var dut = new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.ReadyToBePreserved);
            var fMock = new Mock<Field>();
            fMock.SetupGet(f => f.Id).Returns(41);
            var fv = new FieldValue("", fMock.Object);
            dut.AddFieldValue(fv);
            Assert.AreEqual(1, dut.FieldValues.Count);

            dut.RemoveAnyOldFieldValueWithFieldId(41);

            Assert.AreEqual(0, dut.FieldValues.Count);
        }
        
        [TestMethod]
        public void RemoveFieldValue_ShouldDoNothing_WhenNoFieldValueForFieldIdExists()
        {
            var dut = new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.ReadyToBePreserved);
            dut.AddFieldValue(new Mock<FieldValue>().Object);
            Assert.AreEqual(1, dut.FieldValues.Count);

            dut.RemoveAnyOldFieldValueWithFieldId(141);

            Assert.AreEqual(1, dut.FieldValues.Count);
        }
        
        [TestMethod]
        public void RemoveFieldValue_ShouldThrowException_AfterPreserved()
        {
            var dut = new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.ReadyToBePreserved);
            dut.Preserve(_utcNow, _preservedByMock.Object, true);

            Assert.ThrowsException<Exception>(() => dut.RemoveAnyOldFieldValueWithFieldId(141));
        }

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
            Assert.ThrowsException<Exception>(() => dut.Preserve(preservationTime, _preservedByMock.Object, true));
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenNeedsUserInput()
        {
            var dut = new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.NeedsUserInput);
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.Status);

            // act
            Assert.ThrowsException<Exception>(() => dut.Preserve(_utcNow, _preservedByMock.Object, true));
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenDateNotUtc()
        {
            var dut = new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.ReadyToBePreserved);
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.Status);

            // act
            Assert.ThrowsException<ArgumentException>(() =>
                dut.Preserve(DateTime.Now, _preservedByMock.Object, true)
            );
        }
    }
}

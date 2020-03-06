using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class PreservationPeriodTests
    {
        private const string TestPlant = "PlantA";
        private Field _checkBoxField;
        private Field _infoField;
        private const int PreservedById = 31;
        private ManualTimeProvider _timeProvider;
        private Mock<Person> _preservedByMock;

        [TestInitialize]
        public void Setup()
        {
            _checkBoxField = new Field(TestPlant, "", FieldType.CheckBox, 0);
            _infoField = new Field(TestPlant, "", FieldType.Info, 0);

            _timeProvider = new ManualTimeProvider(new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc));
            TimeService.SetProvider(_timeProvider);
            _preservedByMock = new Mock<Person>();
            _preservedByMock.SetupGet(p => p.Id).Returns(PreservedById);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new PreservationPeriod(TestPlant, _timeProvider.UtcNow, PreservationPeriodStatus.ReadyToBePreserved);

            Assert.AreEqual(TestPlant, dut.Schema);
            Assert.AreEqual(_timeProvider.UtcNow, dut.DueTimeUtc);
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.Status);
            Assert.IsNull(dut.PreservationRecord);
        }

        [TestMethod]
        public void Constructor_ShouldAllowStatusNeedsUserInput()
        {
            var dut = new PreservationPeriod(TestPlant, _timeProvider.UtcNow, PreservationPeriodStatus.NeedsUserInput);

            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.Status);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenStatusIsPreserved()
            => Assert.ThrowsException<ArgumentException>(() =>
                new PreservationPeriod(TestPlant, _timeProvider.UtcNow, PreservationPeriodStatus.Preserved)
            );

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenStatusIsStopped()
            => Assert.ThrowsException<ArgumentException>(() =>
                new PreservationPeriod(TestPlant, _timeProvider.UtcNow, PreservationPeriodStatus.Stopped)
            );

        [TestMethod]
        public void SetComment_ShouldSetComment()
        {
            var dut = new PreservationPeriod(TestPlant, _timeProvider.UtcNow, PreservationPeriodStatus.ReadyToBePreserved);

            dut.SetComment("Comment");

            Assert.AreEqual("Comment", dut.Comment);
        }

        [TestMethod]
        public void SetComment_ShouldThrowException_AfterPreserved()
        {
            var dut = new PreservationPeriod(TestPlant, _timeProvider.UtcNow, PreservationPeriodStatus.ReadyToBePreserved);
            dut.SetComment("Comment");
            Assert.AreEqual("Comment", dut.Comment);

            dut.Preserve(_preservedByMock.Object, true);

            Assert.ThrowsException<Exception>(() => dut.SetComment("X"));
            Assert.AreEqual("Comment", dut.Comment);
        }

        [TestMethod]
        public void Preserve_ShouldPreserve_WhenReadyToBePreserved()
        {
            var dut = new PreservationPeriod(TestPlant, _timeProvider.UtcNow, PreservationPeriodStatus.ReadyToBePreserved);
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.Status);
            Assert.IsNull(dut.PreservationRecord);

            // act
            _timeProvider.Elapse(TimeSpan.FromDays(12));
            dut.Preserve(_preservedByMock.Object, true);

            // assert
            var record = dut.PreservationRecord;
            Assert.IsNotNull(record);
            Assert.AreEqual(PreservationPeriodStatus.Preserved, dut.Status);
            Assert.AreEqual(PreservedById, record.PreservedById);
            Assert.AreEqual(_timeProvider.UtcNow, record.PreservedAtUtc);
            Assert.IsTrue(record.BulkPreserved);
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreserveTwice()
        {
            var dut = new PreservationPeriod(TestPlant, _timeProvider.UtcNow, PreservationPeriodStatus.ReadyToBePreserved);
            Assert.IsNull(dut.PreservationRecord);
            var preservationTime = _timeProvider.UtcNow.AddDays(12);
            dut.Preserve(_preservedByMock.Object, true);

            // act
            preservationTime = preservationTime.AddDays(1);
            Assert.ThrowsException<Exception>(() => dut.Preserve(_preservedByMock.Object, true));
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenNeedsUserInput()
        {
            var dut = new PreservationPeriod(TestPlant, _timeProvider.UtcNow, PreservationPeriodStatus.NeedsUserInput);
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.Status);

            // act
            Assert.ThrowsException<Exception>(() => dut.Preserve(_preservedByMock.Object, true));
        }

        [TestMethod]
        public void RecordValueForField_ShouldAddFieldValueToFieldValuesList_ForCheckBoxField()
        {
            var dut = new PreservationPeriod(TestPlant, _timeProvider.UtcNow, PreservationPeriodStatus.ReadyToBePreserved);

            dut.RecordValueForField(_checkBoxField, "true");

            Assert.AreEqual(1, dut.FieldValues.Count);
        }

        [TestMethod]
        public void RecordValueForField_ShouldThrowException_ForInfoField()
        {
            var dut = new PreservationPeriod(TestPlant, _timeProvider.UtcNow, PreservationPeriodStatus.ReadyToBePreserved);

            Assert.ThrowsException<Exception>(() => dut.RecordValueForField(_infoField, "abc"));
            Assert.AreEqual(0, dut.FieldValues.Count);
        }

        [TestMethod]
        public void RecordValueForField_ShouldThrowException_AfterPreserved()
        {
            var dut = new PreservationPeriod(TestPlant, _timeProvider.UtcNow, PreservationPeriodStatus.ReadyToBePreserved);
            dut.Preserve(_preservedByMock.Object, true);

            Assert.ThrowsException<Exception>(() => dut.RecordValueForField(_checkBoxField, "true"));
            Assert.AreEqual(0, dut.FieldValues.Count);
        }


        [TestMethod]
        public void GetFieldValue_ShouldGetACheckBoxCheckedValue_AfterRecordingCheckBoxValue()
        {
            var dut = new PreservationPeriod(TestPlant, _timeProvider.UtcNow, PreservationPeriodStatus.ReadyToBePreserved);
            var fMock = new Mock<Field>(TestPlant, "", FieldType.CheckBox, 0, null, null);
            fMock.SetupGet(f => f.Id).Returns(12);
            fMock.SetupGet(f => f.Schema).Returns(TestPlant);
            var field = fMock.Object;

            dut.RecordValueForField(field, "true");

            var fieldValue = dut.GetFieldValue(12);

            Assert.IsNotNull(fieldValue);
            Assert.IsInstanceOfType(fieldValue, typeof(CheckBoxChecked));
        }

        [TestMethod]
        public void GetFieldValue_ShouldGetANumberValue_AfterRecordingNumber()
        {
            var dut = new PreservationPeriod(TestPlant, _timeProvider.UtcNow, PreservationPeriodStatus.ReadyToBePreserved);
            var fMock = new Mock<Field>("", "", FieldType.Number, 0, "mm", true);
            fMock.SetupGet(f => f.Id).Returns(12);
            fMock.SetupGet(f => f.Schema).Returns(TestPlant);
            var field = fMock.Object;

            dut.RecordValueForField(field, "NA");

            var fieldValue = dut.GetFieldValue(12);

            Assert.IsNotNull(fieldValue);
            Assert.IsInstanceOfType(fieldValue, typeof(NumberValue));
        }

        [TestMethod]
        public void GetFieldValue_ShouldReturnNull_ForUnknownField()
        {
            var dut = new PreservationPeriod(TestPlant, _timeProvider.UtcNow, PreservationPeriodStatus.ReadyToBePreserved);

            Assert.IsNull(dut.GetFieldValue(12));
        }
    }
}

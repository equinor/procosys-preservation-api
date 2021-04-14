using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.Domain.Time;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
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
        private DateTime _dueUtc;

        [TestInitialize]
        public void Setup()
        {
            _checkBoxField = new Field(TestPlant, "", FieldType.CheckBox, 0);
            _infoField = new Field(TestPlant, "", FieldType.Info, 0);

            var utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _timeProvider = new ManualTimeProvider(utcNow);
            TimeService.SetProvider(_timeProvider);
            _dueUtc = utcNow;

            _preservedByMock = new Mock<Person>();
            _preservedByMock.SetupGet(p => p.Id).Returns(PreservedById);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.ReadyToBePreserved);

            Assert.AreEqual(TestPlant, dut.Plant);
            Assert.AreEqual(_timeProvider.UtcNow, dut.DueTimeUtc);
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.Status);
            Assert.IsNull(dut.PreservationRecord);
        }

        [TestMethod]
        public void Constructor_ShouldAllowStatusNeedsUserInput()
        {
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.NeedsUserInput);

            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.Status);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenStatusIsPreserved()
            => Assert.ThrowsException<ArgumentException>(() =>
                new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.Preserved)
            );

        [TestMethod]
        public void SetComment_ShouldSetComment()
        {
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.ReadyToBePreserved);

            dut.SetComment("Comment");

            Assert.AreEqual("Comment", dut.Comment);
        }

        [TestMethod]
        public void SetComment_ShouldThrowException_AfterPreserved()
        {
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.ReadyToBePreserved);
            dut.SetComment("Comment");
            Assert.AreEqual("Comment", dut.Comment);

            dut.Preserve(_preservedByMock.Object, true);

            Assert.ThrowsException<Exception>(() => dut.SetComment("X"));
            Assert.AreEqual("Comment", dut.Comment);
        }

        [TestMethod]
        public void Preserve_ShouldPreserve_WhenReadyToBePreserved()
        {
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.ReadyToBePreserved);
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
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.ReadyToBePreserved);
            Assert.IsNull(dut.PreservationRecord);
            dut.Preserve(_preservedByMock.Object, true);

            // act
            Assert.ThrowsException<Exception>(() => dut.Preserve(_preservedByMock.Object, true));
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenNeedsUserInput()
        {
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.NeedsUserInput);
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.Status);

            // act
            Assert.ThrowsException<Exception>(() => dut.Preserve(_preservedByMock.Object, true));
        }

        [TestMethod]
        public void RecordCheckBoxValueForField_ShouldAddFieldValueToFieldValuesList_ForCheckBoxField()
        {
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.ReadyToBePreserved);

            dut.RecordCheckBoxValueForField(_checkBoxField, true);

            Assert.AreEqual(1, dut.FieldValues.Count);
        }

        [TestMethod]
        public void RecordCheckBoxValueForField_ShouldThrowException_ForInfoField()
        {
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.ReadyToBePreserved);

            Assert.ThrowsException<Exception>(() => dut.RecordCheckBoxValueForField(_infoField, true));
            Assert.AreEqual(0, dut.FieldValues.Count);
        }

        [TestMethod]
        public void GetAlreadyRecordedAttachmentValueForField_ShouldThrowException_ForInfoField()
        {
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.ReadyToBePreserved);

            Assert.ThrowsException<Exception>(() => dut.GetAlreadyRecordedAttachmentValueForField(_infoField));
        }

        [TestMethod]
        public void RecordAttachmentValueForField_ShouldThrowException_ForInfoField()
        {
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.ReadyToBePreserved);

            Assert.ThrowsException<Exception>(() => dut.RecordAttachmentValueForField(_infoField, null));
        }

        [TestMethod]
        public void RecordCheckBoxValueForField_ShouldThrowException_AfterPreserved()
        {
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.ReadyToBePreserved);
            dut.Preserve(_preservedByMock.Object, true);

            Assert.ThrowsException<Exception>(() => dut.RecordCheckBoxValueForField(_checkBoxField, true));
            Assert.AreEqual(0, dut.FieldValues.Count);
        }

        [TestMethod]
        public void GetFieldValue_ShouldGetACheckBoxCheckedValue_AfterRecordingCheckBoxValue()
        {
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.ReadyToBePreserved);
            var fMock = new Mock<Field>(TestPlant, "", FieldType.CheckBox, 0, null, null);
            fMock.SetupGet(f => f.Id).Returns(12);
            fMock.SetupGet(f => f.Plant).Returns(TestPlant);
            var field = fMock.Object;

            dut.RecordCheckBoxValueForField(field, true);

            var fieldValue = dut.GetFieldValue(12);

            Assert.IsNotNull(fieldValue);
            Assert.IsInstanceOfType(fieldValue, typeof(CheckBoxChecked));
        }

        [TestMethod]
        public void GetFieldValue_ShouldGetANumberValue_AfterRecordingNumber()
        {
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.ReadyToBePreserved);
            var fMock = new Mock<Field>("", "", FieldType.Number, 0, "mm", true);
            fMock.SetupGet(f => f.Id).Returns(12);
            fMock.SetupGet(f => f.Plant).Returns(TestPlant);
            var field = fMock.Object;

            dut.RecordNumberValueForField(field, 1);

            var fieldValue = dut.GetFieldValue(12);

            Assert.IsNotNull(fieldValue);
            Assert.IsInstanceOfType(fieldValue, typeof(NumberValue));
        }

        [TestMethod]
        public void GetFieldValue_ShouldReturnNull_ForUnknownField()
        {
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.ReadyToBePreserved);

            Assert.IsNull(dut.GetFieldValue(12));
        }

        [TestMethod]
        public void SetNewDueTimeUtc_ShouldUpdateDueTime()
        {
            // Arrange
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.ReadyToBePreserved);
            Assert.AreEqual(_dueUtc, dut.DueTimeUtc);

            // Act
            dut.SetNewDueTimeUtc(2);

            var expectedNextDueTimeUtc = _timeProvider.UtcNow.AddWeeks(2);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.DueTimeUtc);
        }

        [TestMethod]
        public void Reschedule_Earlier_ShouldUpdateDueTimeToEarlier()
        {
            // Arrange
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.ReadyToBePreserved);
            Assert.AreEqual(_dueUtc, dut.DueTimeUtc);
            var expectedNextDueTimeUtc = dut.DueTimeUtc.AddWeeks(-2);

            // Act
            dut.Reschedule(2, RescheduledDirection.Earlier);

            Assert.AreEqual(expectedNextDueTimeUtc, dut.DueTimeUtc);
        }

        [TestMethod]
        public void Reschedule_Later_ShouldUpdateDueTimeToLater()
        {
            // Arrange
            var dut = new PreservationPeriod(TestPlant, _dueUtc, PreservationPeriodStatus.ReadyToBePreserved);
            Assert.AreEqual(_dueUtc, dut.DueTimeUtc);
            var expectedNextDueTimeUtc = dut.DueTimeUtc.AddWeeks(4);

            // Act
            dut.Reschedule(4, RescheduledDirection.Later);

            Assert.AreEqual(expectedNextDueTimeUtc, dut.DueTimeUtc);
        }
    }
}

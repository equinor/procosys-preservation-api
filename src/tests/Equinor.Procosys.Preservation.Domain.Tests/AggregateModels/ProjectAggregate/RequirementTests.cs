using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class RequirementTests
    {
        #region Setup
        private const int InfoFieldId = 5;
        private const int CheckBoxFieldId = 11;
        private const int NumberFieldId = 12;
        private const int TwoWeeksInterval = 2;
        private Mock<Field> _infoFieldMock;
        private Mock<Field> _checkBoxFieldMock;
        private Mock<Field> _numberFieldMock;
        private Mock<RequirementDefinition> _reqDefWithInfoFieldMock;
        private Mock<RequirementDefinition> _reqDefWithCheckBoxFieldMock;
        private Mock<RequirementDefinition> _reqDefWithNumberFieldMock;
        private Mock<RequirementDefinition> _reqDefWithNumberAndCheckBoxFieldMock;
        private DateTime _utcNow;

        [TestInitialize]
        public void Setup()
        {
            _infoFieldMock = new Mock<Field>("", "", FieldType.Info, 0, null, null);
            _infoFieldMock.SetupGet(f => f.Id).Returns(InfoFieldId);

            _checkBoxFieldMock = new Mock<Field>("", "", FieldType.CheckBox, 0, null, null);
            _checkBoxFieldMock.SetupGet(f => f.Id).Returns(CheckBoxFieldId);

            _numberFieldMock = new Mock<Field>("", "", FieldType.Number, 0, "mm", true);
            _numberFieldMock.SetupGet(f => f.Id).Returns(NumberFieldId);
            
            _reqDefWithInfoFieldMock = new Mock<RequirementDefinition>();
            _reqDefWithInfoFieldMock.Object.AddField(_infoFieldMock.Object);
            
            _reqDefWithCheckBoxFieldMock = new Mock<RequirementDefinition>();
            _reqDefWithCheckBoxFieldMock.Object.AddField(_checkBoxFieldMock.Object);
            
            _reqDefWithNumberFieldMock = new Mock<RequirementDefinition>();
            _reqDefWithNumberFieldMock.Object.AddField(_numberFieldMock.Object);
            
            _reqDefWithNumberAndCheckBoxFieldMock = new Mock<RequirementDefinition>();
            _reqDefWithNumberAndCheckBoxFieldMock.Object.AddField(_numberFieldMock.Object);
            _reqDefWithNumberAndCheckBoxFieldMock.Object.AddField(_checkBoxFieldMock.Object);

            _reqDefWithInfoFieldMock.SetupGet(rd => rd.Id).Returns(1);
            _reqDefWithCheckBoxFieldMock.SetupGet(rd => rd.Id).Returns(2);
            _reqDefWithNumberFieldMock.SetupGet(rd => rd.Id).Returns(3);
            _reqDefWithNumberAndCheckBoxFieldMock.SetupGet(rd => rd.Id).Returns(4);
            
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
        }

        #endregion

        #region Constructor

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual(_reqDefWithCheckBoxFieldMock.Object.Id, dut.RequirementDefinitionId);
            Assert.IsFalse(dut.IsVoided);
            Assert.IsFalse(dut.ReadyToBePreserved);
            Assert.IsFalse(dut.IsReadyAndDueToBePreserved(_utcNow.AddWeeks(TwoWeeksInterval)));
        }

        [TestMethod]
        public void Constructor_ShouldNotSetActivePeriod()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            Assert.IsFalse(dut.HasActivePeriod);
            Assert.IsNull(dut.ActivePeriod);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementDefinitionNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new Requirement("SchemaA", 4, null)
            );

        #endregion

        #region StartPreservation

        [TestMethod]
        public void StartPreservation_ShouldSetCorrectNextDueDate()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            dut.StartPreservation(_utcNow);

            var expectedNextDueTimeUtc = _utcNow.AddWeeks(TwoWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.NextDueTimeUtc);
        }

        [TestMethod]
        public void StartPreservation_ShouldSetActivePeriod()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            dut.StartPreservation(_utcNow);

            Assert.IsTrue(dut.HasActivePeriod);
            Assert.IsNotNull(dut.ActivePeriod);
        }

        [TestMethod]
        public void StartPreservation_ShouldNotSetReadyToBePreserved_WhenFieldNeedsInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            dut.StartPreservation(_utcNow);

            Assert.IsFalse(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void StartPreservation_ShouldSetReadyToBePreserved_WhenFieldNotNeedInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);

            dut.StartPreservation(_utcNow);

            Assert.IsTrue(dut.ReadyToBePreserved);
        }

        #endregion

        #region IsReadyAndDueToBePreserved
        
        [TestMethod]
        public void IsReadyAndDueToBePreserved_ShouldBeFalse_BeforePeriod()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation(_utcNow);
            Assert.IsTrue(dut.ReadyToBePreserved);

            var sameDayAsStart = _utcNow;

            Assert.IsFalse(dut.IsReadyAndDueToBePreserved(sameDayAsStart));
            Assert.AreEqual(2, dut.GetNextDueInWeeks(sameDayAsStart));
        }

        [TestMethod]
        public void IsReadyAndDueToBePreserved_ShouldBeTrue_InPeriod_WhenNotNeedInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation(_utcNow);
            Assert.IsTrue(dut.ReadyToBePreserved);

            var spotOnTime = _utcNow.AddWeeks(TwoWeeksInterval);

            Assert.IsTrue(dut.IsReadyAndDueToBePreserved(spotOnTime));
            Assert.AreEqual(0, dut.GetNextDueInWeeks(spotOnTime));
        }

        [TestMethod]
        public void IsReadyAndDueToBePreserved_ShouldBeFalse_InPeriod_WhenNeedInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            dut.StartPreservation(_utcNow);
            Assert.IsFalse(dut.ReadyToBePreserved);

            var spotOnTime = _utcNow.AddWeeks(TwoWeeksInterval);

            Assert.IsFalse(dut.IsReadyAndDueToBePreserved(spotOnTime));
            Assert.AreEqual(0, dut.GetNextDueInWeeks(spotOnTime));
        }

        [TestMethod]
        public void IsReadyAndDueToBePreserved_ShouldBeTrue_OnOverdue_WhenNotNeedInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation(_utcNow);
            Assert.IsTrue(dut.ReadyToBePreserved);

            var twoWeekOverdue = _utcNow.AddWeeks(TwoWeeksInterval + TwoWeeksInterval);

            Assert.IsTrue(dut.IsReadyAndDueToBePreserved(twoWeekOverdue));
            Assert.AreEqual(-2, dut.GetNextDueInWeeks(twoWeekOverdue));
        }

        [TestMethod]
        public void IsReadyAndDueToBePreserved_ShouldBeFalse_OnOverdue_WhenNeedInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            dut.StartPreservation(_utcNow);
            Assert.IsFalse(dut.ReadyToBePreserved);

            var twoWeekOverdue = _utcNow.AddWeeks(TwoWeeksInterval+TwoWeeksInterval);

            Assert.IsFalse(dut.IsReadyAndDueToBePreserved(twoWeekOverdue));
            Assert.AreEqual(-2, dut.GetNextDueInWeeks(twoWeekOverdue));
        }

        #endregion

        #region StartPreservation

        [TestMethod]
        public void StartPreservation_ShouldNotSetReadyToBeBulkPreserved_EvenWhenFieldNotNeedInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);

            dut.StartPreservation(_utcNow);

            Assert.IsFalse(dut.IsReadyAndDueToBePreserved(_utcNow.AddDays(2)));
        }

        [TestMethod]
        public void StartPreservation_ShouldThrowException_WhenPreservationAlreadyActive()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            dut.StartPreservation(_utcNow);

            Assert.ThrowsException<Exception>(() => dut.StartPreservation(_utcNow)
            );
        }

        [TestMethod]
        public void StartPreservation_ShouldAddNewPreservationPeriodWithCorrectDueDate()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            dut.StartPreservation(_utcNow);

            var expectedNextDueTimeUtc = _utcNow.AddWeeks(TwoWeeksInterval);
            Assert.AreEqual(1, dut.PreservationPeriods.Count);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.PreservationPeriods.First().DueTimeUtc);
        }

        [TestMethod]
        public void StartPreservation_ShouldAddNewPreservationPeriodWithoutPreservationRecord()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            dut.StartPreservation(_utcNow);

            Assert.IsNull(dut.PreservationPeriods.First().PreservationRecord);
        }

        [TestMethod]
        public void StartPreservation_ShouldAddNewPreservationPeriodWithStatusNeedsUserInput_WhenReqDefNeedsUserInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            dut.StartPreservation(_utcNow);

            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.PreservationPeriods.First().Status);
        }

        [TestMethod]
        public void StartPreservation_ShouldAddNewPreservationPeriodWithStatusReadyToBePreserved_WhenReqDefNotNeedsUserInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);

            dut.StartPreservation(_utcNow);

            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.PreservationPeriods.First().Status);
        }
        
        #endregion

        #region Preserve

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservationNotStarted()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);

            Assert.ThrowsException<Exception>(() =>
                dut.Preserve(_utcNow, new Mock<Person>().Object, false)
            );
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservationPeriodNeedsInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            dut.StartPreservation(_utcNow);

            Assert.ThrowsException<Exception>(() =>
                dut.Preserve(_utcNow, new Mock<Person>().Object, false)
            );
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservedByNoGiven()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation(_utcNow);

            Assert.ThrowsException<ArgumentNullException>(() =>
                dut.Preserve(_utcNow, null, false)
            );
        }
                
        [TestMethod]
        public void Preserve_ShouldUpdateCorrectNextDueDate()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);

            dut.StartPreservation(_utcNow);

            var preservedTime = _utcNow.AddDays(5);
            dut.Preserve(preservedTime, new Mock<Person>().Object, false);
            
            var expectedNextDueTimeUtc = preservedTime.AddWeeks(TwoWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.NextDueTimeUtc);
        }

        [TestMethod]
        public void Preserve_ShouldSetStatusPreserveOnReadyPreservationPeriod()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.Preserve(_utcNow, new Mock<Person>().Object, false);

            Assert.AreEqual(PreservationPeriodStatus.Preserved, dut.PreservationPeriods.First().Status);
        }

        [TestMethod]
        public void Preserve_ShouldCreatePreservationRecordOnReadyPreservationPeriod()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation(_utcNow);

            var personMock = new Mock<Person>();
            personMock.SetupGet(p => p.Id).Returns(51);
            dut.Preserve(_utcNow, personMock.Object, false);

            var preservationRecord = dut.PreservationPeriods.First().PreservationRecord;
            Assert.IsNotNull(preservationRecord);
            Assert.AreEqual(51, preservationRecord.PreservedByPersonId);
            Assert.AreEqual(_utcNow, preservationRecord.PreservedAtUtc);
        }

        [TestMethod]
        public void Preserve_ShouldCreatePreservationRecordWithBulk_WhenBulkPreserve()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation(_utcNow);
            dut.Preserve(_utcNow, new Mock<Person>().Object, true);

            Assert.IsTrue(dut.PreservationPeriods.First().PreservationRecord.BulkPreserved);
        }

        [TestMethod]
        public void Preserve_ShouldCreatePreservationRecordWithoutBulk_WhenNotBulkPreserve()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation(_utcNow);
            dut.Preserve(_utcNow, new Mock<Person>().Object, false);

            Assert.IsFalse(dut.PreservationPeriods.First().PreservationRecord.BulkPreserved);
        }

        [TestMethod]
        public void Preserve_ShouldAddNewPreservationPeriodToPreservationPeriodsList()
        {
            var intervalWeeks = 2;
            var dut = new Requirement("SchemaA", intervalWeeks, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation(_utcNow);

            var preservedTime = _utcNow.AddDays(5);
            dut.Preserve(preservedTime, new Mock<Person>().Object, false);
            
            var expectedNextDueTimeUtc = preservedTime.AddWeeks(intervalWeeks);
            Assert.AreEqual(2, dut.PreservationPeriods.Count);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.PreservationPeriods.Last().DueTimeUtc);
        }

        [TestMethod]
        public void Preserve_ShouldAddNewPreservationPeriodEachTime()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation(_utcNow);

            var preserveCount = 15;
            for (var i = 0; i < preserveCount; i++)
            {
                dut.Preserve(_utcNow, new Mock<Person>().Object, false);
            }
            
            Assert.AreEqual(preserveCount+1, dut.PreservationPeriods.Count);
        }

        #endregion

        #region RecordValuesForActivePeriod

        [TestMethod]
        public void RecordValuesForActivePeriod_ShouldThrowException_WhenPreservationNotStarted()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);

            Assert.ThrowsException<Exception>(() =>
                dut.RecordValuesForActivePeriod(null, null, _reqDefWithCheckBoxFieldMock.Object)
            );
        }

        [TestMethod]
        public void RecordValuesForActivePeriod_ShouldThrowException_WhenReqDefNotGiven()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation(_utcNow);

            Assert.ThrowsException<ArgumentNullException>(() =>
                dut.RecordValuesForActivePeriod(null, null, null)
            );
        }

        [TestMethod]
        public void RecordValuesForActivePeriod_ShouldThrowException_WhenRecordingOnWrongDefinition()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithNumberFieldMock.Object);
            dut.StartPreservation(_utcNow);

            Assert.ThrowsException<Exception>(() =>
                dut.RecordValuesForActivePeriod(null, null, _reqDefWithCheckBoxFieldMock.Object)
            );
        }

        [TestMethod]
        public void RecordValuesForActivePeriod_ShouldThrowException_WhenFieldIsInfo()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation(_utcNow);

            Assert.ThrowsException<Exception>(() =>
                dut.RecordValuesForActivePeriod(
                    new Dictionary<int, string>{ {InfoFieldId, "x"}},
                    null,
                    _reqDefWithInfoFieldMock.Object)
            );
        }
        
        [TestMethod]
        public void RecordValuesForActivePeriod_WithComment_ShouldUpdateCommentOnActivePeriod()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.RecordValuesForActivePeriod(null, "Abc", _reqDefWithCheckBoxFieldMock.Object);
            Assert.AreEqual("Abc", dut.ActivePeriod.Comment);

            dut.RecordValuesForActivePeriod(null, null, _reqDefWithCheckBoxFieldMock.Object);
            Assert.IsNull(dut.ActivePeriod.Comment);
        }
        
        [TestMethod]
        public void RecordValuesForActivePeriod_WithCheckBoxChecked_ShouldCreateNewCheckBoxChecked_WhenValueIsTrue()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "true"}
                }, 
                null,
                _reqDefWithCheckBoxFieldMock.Object);

            // Assert
            var fieldValues = dut.ActivePeriod.FieldValues;
            Assert.AreEqual(1, fieldValues.Count);
            var fv = fieldValues.First();
            Assert.IsInstanceOfType(fv, typeof(CheckBoxChecked));
            Assert.AreEqual(CheckBoxFieldId, fv.FieldId);
        }
        
        [TestMethod]
        public void RecordValuesForActivePeriod_WithCheckBoxUnchecked_ShouldDoNothing_WhenNoValueExistsInAdvance()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "false"}
                }, 
                null,
                _reqDefWithCheckBoxFieldMock.Object);

            // Assert
            Assert.AreEqual(0, dut.ActivePeriod.FieldValues.Count);
        }

        
        [TestMethod]
        public void RecordValuesForActivePeriod_WithNaAsNumber_ShouldCreateNumberValueWithNullValue()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithNumberFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {NumberFieldId, "n/a"}
                }, 
                null,
                _reqDefWithNumberFieldMock.Object);

            // Assert
            var fieldValues = dut.ActivePeriod.FieldValues;
            Assert.AreEqual(1, fieldValues.Count);
            var fv = fieldValues.First();
            Assert.IsInstanceOfType(fv, typeof(NumberValue));
            Assert.AreEqual(NumberFieldId, fv.FieldId);
            Assert.IsNull(((NumberValue)fv).Value);
        }

        [TestMethod]
        public void RecordValuesForActivePeriod_WithNumber_ShouldCreateNumberValueWithCorrectValue()
        {
            var number = 1282.91;
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithNumberFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {NumberFieldId, number.ToString("F2")}
                }, 
                null,
                _reqDefWithNumberFieldMock.Object);

            // Assert
            var fieldValues = dut.ActivePeriod.FieldValues;
            Assert.AreEqual(1, fieldValues.Count);
            var fv = fieldValues.First();
            Assert.IsInstanceOfType(fv, typeof(NumberValue));
            Assert.AreEqual(NumberFieldId, fv.FieldId);
            var numberValue = (NumberValue)fv;
            Assert.IsTrue( numberValue.Value.HasValue);
            Assert.AreEqual(number, numberValue.Value.Value);
        }

        [TestMethod]
        public void RecordValuesForActivePeriod_WithNoNumber_ShouldDoNothing_WhenNoValueExistsInAdvance()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithNumberFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {NumberFieldId, null}
                }, 
                null,
                _reqDefWithNumberFieldMock.Object);

            // Assert
            Assert.AreEqual(0, dut.ActivePeriod.FieldValues.Count);
        }

        [TestMethod]
        public void RecordValuesForActivePeriod_ShouldDeleteExistingCheckBoxValueAndNotCreateNew_WhenCheckBoxIsUncheckedAndValueExistsInAdvance()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "true"}
                }, 
                null,
                _reqDefWithCheckBoxFieldMock.Object);

            // Assert
            Assert.AreEqual(1, dut.ActivePeriod.FieldValues.Count);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "false"}
                }, 
                null,
                _reqDefWithCheckBoxFieldMock.Object);

            Assert.AreEqual(0, dut.ActivePeriod.FieldValues.Count);
        }

        [TestMethod]
        public void RecordValuesForActivePeriod_ShouldDeleteExistingNumberValueAndNotCreateNew_WhenNumberIsNullAndValueExistsInAdvance()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithNumberFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {NumberFieldId, "na"}
                }, 
                null,
                _reqDefWithNumberFieldMock.Object);

            // Assert
            Assert.AreEqual(1, dut.ActivePeriod.FieldValues.Count);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {NumberFieldId, null}
                }, 
                null,
                _reqDefWithNumberFieldMock.Object);

            // Assert
            Assert.AreEqual(0, dut.ActivePeriod.FieldValues.Count);
        }

        
        [TestMethod]
        public void RecordValuesForActivePeriod_ShouldMakeRequirementReadyToBePreserved_WhenRecordRealValues_OneByOne()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithNumberAndCheckBoxFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "true"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.ActivePeriod.Status);
            Assert.IsFalse(dut.ReadyToBePreserved);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {NumberFieldId, "na"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.ActivePeriod.Status);
            Assert.IsTrue(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void RecordValuesForActivePeriod_ShouldMakeRequirementReadyToBePreserved_WhenRecordRealValues_AllRequiredAtOnce()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithNumberAndCheckBoxFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "true"},
                    {NumberFieldId, "na"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.ActivePeriod.Status);
            Assert.IsTrue(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void RecordValuesForActivePeriod_ToggleReadyToBePreserved_WhenRecordRealValues_AllRequiredAtOnce_ThenRemoveCheckBox()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithNumberAndCheckBoxFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "true"},
                    {NumberFieldId, "na"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.ActivePeriod.Status);
            Assert.IsTrue(dut.ReadyToBePreserved);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "false"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.ActivePeriod.Status);
            Assert.IsFalse(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void RecordValuesForActivePeriod_ShouldToggleReadyToBePreserved_WhenRecordingCheckBoxValue()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "true"}
                }, 
                null,
                _reqDefWithCheckBoxFieldMock.Object);

            // Assert
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.ActivePeriod.Status);
            Assert.IsTrue(dut.ReadyToBePreserved);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "false"}
                }, 
                null,
                _reqDefWithCheckBoxFieldMock.Object);

            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.ActivePeriod.Status);
            Assert.IsFalse(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void RecordValuesForActivePeriod_ShouldToggleReadyToBePreserved_WhenRecordingNumberValue()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithNumberFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {NumberFieldId, "1"}
                }, 
                null,
                _reqDefWithNumberFieldMock.Object);

            // Assert
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.ActivePeriod.Status);
            Assert.IsTrue(dut.ReadyToBePreserved);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {NumberFieldId, null}
                }, 
                null,
                _reqDefWithNumberFieldMock.Object);

            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.ActivePeriod.Status);
            Assert.IsFalse(dut.ReadyToBePreserved);
        }

        #endregion

        #region GetCurrentValue

        [TestMethod]
        public void GetCurrentFieldValue_ShouldReturnNull_BeforeRecording()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithNumberFieldMock.Object);
            dut.StartPreservation(_utcNow);

            Assert.IsNull(dut.GetCurrentFieldValue(_numberFieldMock.Object));
        }

        [TestMethod]
        public void GetCurrentFieldValue_ShouldReturnNull_ForUnknownField()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithNumberFieldMock.Object);
            dut.StartPreservation(_utcNow);

            Assert.IsNull(dut.GetCurrentFieldValue(new Mock<Field>().Object));
        }

        [TestMethod]
        public void GetCurrentFieldValue_ShouldReturnCheckBoxValue_AfterRecordingCheckBoxTrue()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithNumberAndCheckBoxFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "true"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            // Assert
            var value = dut.GetCurrentFieldValue(_checkBoxFieldMock.Object);

            Assert.IsNotNull(value);
            Assert.IsInstanceOfType(value, typeof(CheckBoxChecked));
        }

        [TestMethod]
        public void GetCurrentFieldValue_ShouldReturnNull_AfterRecordingCheckBoxTrue_ThenRecordCheckBoxFalse()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithNumberAndCheckBoxFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "true"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            // Assert
            Assert.IsNotNull(dut.GetCurrentFieldValue(_checkBoxFieldMock.Object));

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "false"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            // Assert
            Assert.IsNull(dut.GetCurrentFieldValue(_checkBoxFieldMock.Object));
        }

        [TestMethod]
        public void GetCurrentFieldValue_ShouldReturnNumberValue_AfterRecordingNumber()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithNumberAndCheckBoxFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {NumberFieldId, "123"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            // Assert
            var value = dut.GetCurrentFieldValue(_numberFieldMock.Object);

            Assert.IsNotNull(value);
            Assert.IsInstanceOfType(value, typeof(NumberValue));
        }

        [TestMethod]
        public void GetCurrentFieldValue_ShouldReturnNull_AfterRecordingNumber_ThenRecordNull()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithNumberAndCheckBoxFieldMock.Object);
            dut.StartPreservation(_utcNow);

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {NumberFieldId, "123"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            // Assert
            Assert.IsNotNull(dut.GetCurrentFieldValue(_numberFieldMock.Object));

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {NumberFieldId, null}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            // Assert
            Assert.IsNull(dut.GetCurrentFieldValue(_numberFieldMock.Object));
        }

        #endregion

        #region GetPreviousValue

        [TestMethod]
        public void GetPreviousFieldValue_ShouldReturnValue_AfterRecordingAndPreserved()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithNumberFieldMock.Object);
            dut.StartPreservation(_utcNow);

            Assert.IsNull(dut.GetPreviousFieldValue(_numberFieldMock.Object));

            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {NumberFieldId, "1"}
                }, 
                null,
                _reqDefWithNumberFieldMock.Object);

            // Assert
            var value = dut.GetPreviousFieldValue(_numberFieldMock.Object);
            Assert.IsNull(value);

            // preserve and get a new period
            dut.Preserve(_utcNow.AddDays(5), new Mock<Person>().Object, false);

            value = dut.GetPreviousFieldValue(_numberFieldMock.Object);
            Assert.IsNotNull(value);
            Assert.IsInstanceOfType(value, typeof(NumberValue));
            Assert.AreEqual(1, ((NumberValue)value).Value);

            // record a new value in this period
            dut.RecordValuesForActivePeriod(
                new Dictionary<int, string>
                {
                    {NumberFieldId, "2"}
                }, 
                null,
                _reqDefWithNumberFieldMock.Object);

            value = dut.GetPreviousFieldValue(_numberFieldMock.Object);
            Assert.AreEqual(1, ((NumberValue)value).Value);
            
            dut.Preserve(_utcNow.AddDays(15), new Mock<Person>().Object, false);
            value = dut.GetPreviousFieldValue(_numberFieldMock.Object);
            Assert.AreEqual(2, ((NumberValue)value).Value);
        }

        [TestMethod]
        public void GetPreviousFieldValue_ShouldReturnNull_ForUnknownField()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithNumberFieldMock.Object);
            dut.StartPreservation(_utcNow);

            Assert.IsNull(dut.GetPreviousFieldValue(new Mock<Field>().Object));
        }

        #endregion

        [TestMethod]
        public void VoidUnVoid_ShouldToggleIsVoided()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            Assert.IsFalse(dut.IsVoided);

            dut.Void();
            Assert.IsTrue(dut.IsVoided);

            dut.UnVoid();
            Assert.IsFalse(dut.IsVoided);
        }
    }
}

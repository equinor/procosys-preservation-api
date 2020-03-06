using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Equinor.Procosys.Preservation.Test.Common;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class RequirementTests
    {
        private const string TestPlant = "PlantA";
    
        #region Setup
        private const int InfoFieldId = 5;
        private const int CheckBoxFieldId = 11;
        private const int NumberField1Id = 12;
        private const int NumberField2Id = 22;
        private const int NumberField3Id = 32;
        private const int TwoWeeksInterval = 2;
        private Mock<Field> _infoFieldMock;
        private Mock<Field> _checkBoxFieldMock;
        private Mock<Field> _numberField1Mock;
        private Mock<Field> _numberField2Mock;
        private Mock<Field> _numberField3Mock;
        private Mock<RequirementDefinition> _reqDefWithInfoFieldMock;
        private Mock<RequirementDefinition> _reqDefWithCheckBoxFieldMock;
        private Mock<RequirementDefinition> _reqDefWithOneNumberFieldMock;
        private Mock<RequirementDefinition> _reqDefWithTwoNumberFieldsMock;
        private Mock<RequirementDefinition> _reqDefWithNumberAndCheckBoxFieldMock;
        private DateTime _utcNow;
        private ManualTimeProvider _timeProvider;

        [TestInitialize]
        public void Setup()
        {
            _infoFieldMock = new Mock<Field>("", "", FieldType.Info, 0, null, null);
            _infoFieldMock.SetupGet(f => f.Id).Returns(InfoFieldId);
            _infoFieldMock.SetupGet(f => f.Schema).Returns(TestPlant);

            _checkBoxFieldMock = new Mock<Field>("", "", FieldType.CheckBox, 0, null, null);
            _checkBoxFieldMock.SetupGet(f => f.Id).Returns(CheckBoxFieldId);
            _checkBoxFieldMock.SetupGet(f => f.Schema).Returns(TestPlant);

            _numberField1Mock = new Mock<Field>("", "", FieldType.Number, 0, "mm", true);
            _numberField1Mock.SetupGet(f => f.Id).Returns(NumberField1Id);
            _numberField1Mock.SetupGet(f => f.Schema).Returns(TestPlant);

            _numberField2Mock = new Mock<Field>("", "", FieldType.Number, 0, "mm", true);
            _numberField2Mock.SetupGet(f => f.Id).Returns(NumberField2Id);
            _numberField2Mock.SetupGet(f => f.Schema).Returns(TestPlant);

            _numberField3Mock = new Mock<Field>("", "", FieldType.Number, 0, "mm", true);
            _numberField3Mock.SetupGet(f => f.Id).Returns(NumberField3Id);
            _numberField3Mock.SetupGet(f => f.Schema).Returns(TestPlant);
            
            _reqDefWithInfoFieldMock = new Mock<RequirementDefinition>();
            _reqDefWithInfoFieldMock.SetupGet(f => f.Schema).Returns(TestPlant);
            _reqDefWithInfoFieldMock.Object.AddField(_infoFieldMock.Object);
            
            _reqDefWithCheckBoxFieldMock = new Mock<RequirementDefinition>();
            _reqDefWithCheckBoxFieldMock.SetupGet(f => f.Schema).Returns(TestPlant);
            _reqDefWithCheckBoxFieldMock.Object.AddField(_checkBoxFieldMock.Object);
            
            _reqDefWithOneNumberFieldMock = new Mock<RequirementDefinition>();
            _reqDefWithOneNumberFieldMock.SetupGet(f => f.Schema).Returns(TestPlant);
            _reqDefWithOneNumberFieldMock.Object.AddField(_numberField1Mock.Object);
            
            _reqDefWithTwoNumberFieldsMock = new Mock<RequirementDefinition>();
            _reqDefWithTwoNumberFieldsMock.SetupGet(f => f.Schema).Returns(TestPlant);
            _reqDefWithTwoNumberFieldsMock.Object.AddField(_numberField2Mock.Object);
            _reqDefWithTwoNumberFieldsMock.Object.AddField(_numberField3Mock.Object);
            
            _reqDefWithNumberAndCheckBoxFieldMock = new Mock<RequirementDefinition>();
            _reqDefWithNumberAndCheckBoxFieldMock.SetupGet(f => f.Schema).Returns(TestPlant);
            _reqDefWithNumberAndCheckBoxFieldMock.Object.AddField(_numberField1Mock.Object);
            _reqDefWithNumberAndCheckBoxFieldMock.Object.AddField(_checkBoxFieldMock.Object);

            _reqDefWithInfoFieldMock.SetupGet(rd => rd.Id).Returns(1);
            _reqDefWithNumberAndCheckBoxFieldMock.SetupGet(f => f.Schema).Returns(TestPlant);
            _reqDefWithCheckBoxFieldMock.SetupGet(rd => rd.Id).Returns(2);
            _reqDefWithOneNumberFieldMock.SetupGet(rd => rd.Id).Returns(3);
            _reqDefWithTwoNumberFieldsMock.SetupGet(rd => rd.Id).Returns(13);
            _reqDefWithNumberAndCheckBoxFieldMock.SetupGet(rd => rd.Id).Returns(4);
            
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _timeProvider = new ManualTimeProvider(_utcNow);
            TimeService.SetProvider(_timeProvider);
        }

        #endregion

        #region Constructor

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            Assert.AreEqual(TestPlant, dut.Schema);
            Assert.AreEqual(_reqDefWithCheckBoxFieldMock.Object.Id, dut.RequirementDefinitionId);
            Assert.IsFalse(dut.IsVoided);
            Assert.IsFalse(dut.ReadyToBePreserved);
            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.IsFalse(dut.IsReadyAndDueToBePreserved());
        }

        [TestMethod]
        public void Constructor_ShouldNotSetActivePeriod()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            Assert.IsFalse(dut.HasActivePeriod);
            Assert.IsNull(dut.ActivePeriod);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementDefinitionNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new Requirement(TestPlant, 4, null)
            );

        #endregion

        #region StartPreservation

        [TestMethod]
        public void StartPreservation_ShouldSetCorrectNextDueDate()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            dut.StartPreservation();

            var expectedNextDueTimeUtc = _utcNow.AddWeeks(TwoWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.NextDueTimeUtc);
        }

        [TestMethod]
        public void StartPreservation_ShouldSetActivePeriod()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            dut.StartPreservation();

            Assert.IsTrue(dut.HasActivePeriod);
            Assert.IsNotNull(dut.ActivePeriod);
        }

        [TestMethod]
        public void StartPreservation_ShouldNotSetReadyToBePreserved_WhenFieldNeedsInput()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            dut.StartPreservation();

            Assert.IsFalse(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void StartPreservation_ShouldSetReadyToBePreserved_WhenFieldNotNeedInput()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);

            dut.StartPreservation();

            Assert.IsTrue(dut.ReadyToBePreserved);
        }

        #endregion

        #region IsReadyAndDueToBePreserved
        
        [TestMethod]
        public void IsReadyAndDueToBePreserved_ShouldBeFalse_BeforePeriod()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation();
            Assert.IsTrue(dut.ReadyToBePreserved);

            Assert.IsFalse(dut.IsReadyAndDueToBePreserved());
            Assert.AreEqual(2, dut.GetNextDueInWeeks());
        }

        [TestMethod]
        public void IsReadyAndDueToBePreserved_ShouldBeTrue_InPeriod_WhenNotNeedInput()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation();
            Assert.IsTrue(dut.ReadyToBePreserved);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);
            Assert.IsTrue(dut.IsReadyAndDueToBePreserved());
            Assert.AreEqual(0, dut.GetNextDueInWeeks());
        }

        [TestMethod]
        public void IsReadyAndDueToBePreserved_ShouldBeFalse_InPeriod_WhenNeedInput()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            dut.StartPreservation();
            Assert.IsFalse(dut.ReadyToBePreserved);

            _timeProvider.ElapseWeeks(TwoWeeksInterval);

            Assert.IsFalse(dut.IsReadyAndDueToBePreserved());
            Assert.AreEqual(0, dut.GetNextDueInWeeks());
        }

        [TestMethod]
        public void IsReadyAndDueToBePreserved_ShouldBeTrue_OnOverdue_WhenNotNeedInput()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation();
            Assert.IsTrue(dut.ReadyToBePreserved);

            _timeProvider.ElapseWeeks(TwoWeeksInterval + TwoWeeksInterval);

            Assert.IsTrue(dut.IsReadyAndDueToBePreserved());
            Assert.AreEqual(-2, dut.GetNextDueInWeeks());
        }

        [TestMethod]
        public void IsReadyAndDueToBePreserved_ShouldBeFalse_OnOverdue_WhenNeedInput()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            dut.StartPreservation();
            Assert.IsFalse(dut.ReadyToBePreserved);

            _timeProvider.ElapseWeeks(TwoWeeksInterval + TwoWeeksInterval);

            Assert.IsFalse(dut.IsReadyAndDueToBePreserved());
            Assert.AreEqual(-2, dut.GetNextDueInWeeks());
        }

        #endregion

        #region StartPreservation

        [TestMethod]
        public void StartPreservation_ShouldNotSetReadyToBeBulkPreserved_EvenWhenFieldNotNeedInput()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);

            dut.StartPreservation();

            _timeProvider.Elapse(TimeSpan.FromDays(2));
            Assert.IsFalse(dut.IsReadyAndDueToBePreserved());
        }

        [TestMethod]
        public void StartPreservation_ShouldThrowException_WhenPreservationAlreadyActive()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            dut.StartPreservation();

            Assert.ThrowsException<Exception>(() => dut.StartPreservation()
            );
        }

        [TestMethod]
        public void StartPreservation_ShouldAddNewPreservationPeriodWithCorrectDueDate()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            dut.StartPreservation();

            var expectedNextDueTimeUtc = _utcNow.AddWeeks(TwoWeeksInterval);
            Assert.AreEqual(1, dut.PreservationPeriods.Count);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.PreservationPeriods.First().DueTimeUtc);
        }

        [TestMethod]
        public void StartPreservation_ShouldAddNewPreservationPeriodWithoutPreservationRecord()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            dut.StartPreservation();

            Assert.IsNull(dut.PreservationPeriods.First().PreservationRecord);
        }

        [TestMethod]
        public void StartPreservation_ShouldAddNewPreservationPeriodWithStatusNeedsUserInput_WhenReqDefNeedsUserInput()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);

            dut.StartPreservation();

            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.PreservationPeriods.First().Status);
        }

        [TestMethod]
        public void StartPreservation_ShouldAddNewPreservationPeriodWithStatusReadyToBePreserved_WhenReqDefNotNeedsUserInput()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);

            dut.StartPreservation();

            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.PreservationPeriods.First().Status);
        }
        
        #endregion

        #region Preserve

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservationNotStarted()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);

            Assert.ThrowsException<Exception>(() =>
                dut.Preserve(new Mock<Person>().Object, false)
            );
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservationPeriodNeedsInput()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            dut.StartPreservation();

            Assert.ThrowsException<Exception>(() =>
                dut.Preserve(new Mock<Person>().Object, false)
            );
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservedByNoGiven()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation();

            Assert.ThrowsException<ArgumentNullException>(() =>
                dut.Preserve(null, false)
            );
        }
                
        [TestMethod]
        public void Preserve_ShouldUpdateCorrectNextDueDate()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);

            dut.StartPreservation();

            _timeProvider.Elapse(TimeSpan.FromDays(5));
            dut.Preserve(new Mock<Person>().Object, false);
            
            var expectedNextDueTimeUtc = _timeProvider.UtcNow.AddWeeks(TwoWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.NextDueTimeUtc);
        }

        [TestMethod]
        public void Preserve_ShouldSetStatusPreserveOnReadyPreservationPeriod()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation();

            dut.Preserve(new Mock<Person>().Object, false);

            Assert.AreEqual(PreservationPeriodStatus.Preserved, dut.PreservationPeriods.First().Status);
        }

        [TestMethod]
        public void Preserve_ShouldCreatePreservationRecordOnReadyPreservationPeriod()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation();

            var personMock = new Mock<Person>();
            personMock.SetupGet(p => p.Id).Returns(51);
            dut.Preserve(personMock.Object, false);

            var preservationRecord = dut.PreservationPeriods.First().PreservationRecord;
            Assert.IsNotNull(preservationRecord);
            Assert.AreEqual(51, preservationRecord.PreservedById);
            Assert.AreEqual(_utcNow, preservationRecord.PreservedAtUtc);
        }

        [TestMethod]
        public void Preserve_ShouldCreatePreservationRecordWithBulk_WhenBulkPreserve()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation();
            dut.Preserve(new Mock<Person>().Object, true);

            Assert.IsTrue(dut.PreservationPeriods.First().PreservationRecord.BulkPreserved);
        }

        [TestMethod]
        public void Preserve_ShouldCreatePreservationRecordWithoutBulk_WhenNotBulkPreserve()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation();
            dut.Preserve(new Mock<Person>().Object, false);

            Assert.IsFalse(dut.PreservationPeriods.First().PreservationRecord.BulkPreserved);
        }

        [TestMethod]
        public void Preserve_ShouldAddNewPreservationPeriodToPreservationPeriodsList()
        {
            var intervalWeeks = 2;
            var dut = new Requirement(TestPlant, intervalWeeks, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation();

            _timeProvider.Elapse(TimeSpan.FromDays(5));
            dut.Preserve(new Mock<Person>().Object, false);
            
            var expectedNextDueTimeUtc = _timeProvider.UtcNow.AddWeeks(intervalWeeks);
            Assert.AreEqual(2, dut.PreservationPeriods.Count);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.PreservationPeriods.Last().DueTimeUtc);
        }

        [TestMethod]
        public void Preserve_ShouldAddNewPreservationPeriodEachTime()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation();

            var preserveCount = 15;
            for (var i = 0; i < preserveCount; i++)
            {
                dut.Preserve(new Mock<Person>().Object, false);
            }
            
            Assert.AreEqual(preserveCount+1, dut.PreservationPeriods.Count);
        }

        #endregion

        #region RecordValuesForActivePeriod

        [TestMethod]
        public void RecordValues_ShouldThrowException_WhenPreservationNotStarted()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);

            Assert.ThrowsException<Exception>(() =>
                dut.RecordValues(null, null, _reqDefWithCheckBoxFieldMock.Object)
            );
        }

        [TestMethod]
        public void RecordValues_ShouldThrowException_WhenReqDefNotGiven()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation();

            Assert.ThrowsException<ArgumentNullException>(() =>
                dut.RecordValues(null, null, null)
            );
        }

        [TestMethod]
        public void RecordValues_ShouldThrowException_WhenRecordingOnWrongDefinition()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithOneNumberFieldMock.Object);
            dut.StartPreservation();

            Assert.ThrowsException<Exception>(() =>
                dut.RecordValues(null, null, _reqDefWithCheckBoxFieldMock.Object)
            );
        }

        [TestMethod]
        public void RecordValues_ShouldThrowException_WhenFieldIsInfo()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithInfoFieldMock.Object);
            dut.StartPreservation();

            Assert.ThrowsException<Exception>(() =>
                dut.RecordValues(
                    new Dictionary<int, string>{ {InfoFieldId, "x"}},
                    null,
                    _reqDefWithInfoFieldMock.Object)
            );
        }
        
        [TestMethod]
        public void RecordValues_WithComment_ShouldUpdateCommentOnActivePeriod()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(null, "Abc", _reqDefWithCheckBoxFieldMock.Object);
            Assert.AreEqual("Abc", dut.ActivePeriod.Comment);

            dut.RecordValues(null, null, _reqDefWithCheckBoxFieldMock.Object);
            Assert.IsNull(dut.ActivePeriod.Comment);
        }
        
        [TestMethod]
        public void RecordValues_WithCheckBoxChecked_ShouldCreateNewCheckBoxChecked_WhenValueIsTrue()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
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
        public void RecordValues_WithCheckBoxUnchecked_ShouldDoNothing_WhenNoValueExistsInAdvance()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
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
        public void RecordValues_WithNaAsNumber_ShouldCreateNumberValueWithNullValue()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithOneNumberFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField1Id, "n/a"}
                }, 
                null,
                _reqDefWithOneNumberFieldMock.Object);

            // Assert
            var fieldValues = dut.ActivePeriod.FieldValues;
            Assert.AreEqual(1, fieldValues.Count);
            var fv = fieldValues.First();
            Assert.IsInstanceOfType(fv, typeof(NumberValue));
            Assert.AreEqual(NumberField1Id, fv.FieldId);
            Assert.IsNull(((NumberValue)fv).Value);
        }

        [TestMethod]
        public void RecordValues_WithNumber_ShouldCreateNumberValueWithCorrectValue()
        {
            var number = 1282.91;
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithOneNumberFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField1Id, number.ToString("F2")}
                }, 
                null,
                _reqDefWithOneNumberFieldMock.Object);

            // Assert
            var fieldValues = dut.ActivePeriod.FieldValues;
            Assert.AreEqual(1, fieldValues.Count);
            var fv = fieldValues.First();
            Assert.IsInstanceOfType(fv, typeof(NumberValue));
            Assert.AreEqual(NumberField1Id, fv.FieldId);
            var numberValue = (NumberValue)fv;
            Assert.IsTrue( numberValue.Value.HasValue);
            Assert.AreEqual(number, numberValue.Value.Value);
        }

        [TestMethod]
        public void RecordValues_WithNoNumber_ShouldDoNothing_WhenNoValueExistsInAdvance()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithOneNumberFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField1Id, null}
                }, 
                null,
                _reqDefWithOneNumberFieldMock.Object);

            // Assert
            Assert.AreEqual(0, dut.ActivePeriod.FieldValues.Count);
        }

        [TestMethod]
        public void RecordValues_ShouldDeleteExistingCheckBoxValue_WhenCheckBoxIsUnchecked()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "true"}
                }, 
                null,
                _reqDefWithCheckBoxFieldMock.Object);

            // Assert
            Assert.AreEqual(1, dut.ActivePeriod.FieldValues.Count);

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "false"}
                }, 
                null,
                _reqDefWithCheckBoxFieldMock.Object);

            Assert.AreEqual(0, dut.ActivePeriod.FieldValues.Count);
        }

        [TestMethod]
        public void RecordValues_ShouldDeleteExistingNumberValue_WhenNumberIsNull()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithOneNumberFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField1Id, "na"}
                }, 
                null,
                _reqDefWithOneNumberFieldMock.Object);

            // Assert
            Assert.AreEqual(1, dut.ActivePeriod.FieldValues.Count);

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField1Id, null}
                }, 
                null,
                _reqDefWithOneNumberFieldMock.Object);

            // Assert
            Assert.AreEqual(0, dut.ActivePeriod.FieldValues.Count);
        }


        [TestMethod]
        public void RecordValues_ShouldDeleteExistingNumberValue_WhenNumberIsBlank()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithOneNumberFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField1Id, "na"}
                }, 
                null,
                _reqDefWithOneNumberFieldMock.Object);

            // Assert
            Assert.AreEqual(1, dut.ActivePeriod.FieldValues.Count);

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField1Id, string.Empty}
                }, 
                null,
                _reqDefWithOneNumberFieldMock.Object);

            // Assert
            Assert.AreEqual(0, dut.ActivePeriod.FieldValues.Count);
        }
        
        [TestMethod]
        public void RecordValues_ShouldMakeRequirementReadyToBePreserved_WhenRecordValues_OneByOne()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithNumberAndCheckBoxFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "true"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.ActivePeriod.Status);
            Assert.IsFalse(dut.ReadyToBePreserved);

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField1Id, "1"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.ActivePeriod.Status);
            Assert.IsTrue(dut.ReadyToBePreserved);
        }
        
        [TestMethod]
        public void RecordValues_ShouldNotMakeRequirementReadyToBePreserved_WhenRecordNaForSingleNumber_TogetherWithCheckBox()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithNumberAndCheckBoxFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "true"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.ActivePeriod.Status);
            Assert.IsFalse(dut.ReadyToBePreserved);

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField1Id, "na"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.ActivePeriod.Status);
            Assert.IsFalse(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void RecordValues_ShouldMakeRequirementReadyToBePreserved_WhenRecordValues_AllRequiredAtOnce()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithNumberAndCheckBoxFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "true"},
                    {NumberField1Id, "1"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.ActivePeriod.Status);
            Assert.IsTrue(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void RecordValues_ShouldNotMakeRequirementReadyToBePreserved_WhenRecordValues_AllRequiredAtOnce_WithNaForSingleNumber()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithNumberAndCheckBoxFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "true"},
                    {NumberField1Id, "na"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.ActivePeriod.Status);
            Assert.IsFalse(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void RecordValues_ShouldNotMakeRequirementReadyToBePreserved_WhenRecordValues_AllRequiredAtOnce_WithNaForAllNumbers()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithTwoNumberFieldsMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField2Id, "NA"},
                    {NumberField3Id, "n/a"}
                }, 
                null,
                _reqDefWithTwoNumberFieldsMock.Object);

            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.ActivePeriod.Status);
            Assert.IsFalse(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void RecordValues_ShouldMakeRequirementReadyToBePreserved_WhenRecordValues_AllRequiredAtOnce_WithNaForSomeNumbers()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithTwoNumberFieldsMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField2Id, "1"},
                    {NumberField3Id, "n/a"}
                }, 
                null,
                _reqDefWithTwoNumberFieldsMock.Object);

            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.ActivePeriod.Status);
            Assert.IsTrue(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void RecordValues_ToggleReadyToBePreserved_WhenRecordValues_AllRequiredAtOnce_ThenRemoveCheckBox()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithNumberAndCheckBoxFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "true"},
                    {NumberField1Id, "1"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.ActivePeriod.Status);
            Assert.IsTrue(dut.ReadyToBePreserved);

            dut.RecordValues(
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
        public void RecordValues_ShouldToggleReadyToBePreserved_WhenRecordingCheckBoxValue()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "true"}
                }, 
                null,
                _reqDefWithCheckBoxFieldMock.Object);

            // Assert
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.ActivePeriod.Status);
            Assert.IsTrue(dut.ReadyToBePreserved);

            dut.RecordValues(
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
        public void RecordValues_ShouldToggleReadyToBePreserved_WhenRecordingNumberValue()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithOneNumberFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField1Id, "1"}
                }, 
                null,
                _reqDefWithOneNumberFieldMock.Object);

            // Assert
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.ActivePeriod.Status);
            Assert.IsTrue(dut.ReadyToBePreserved);

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField1Id, null}
                }, 
                null,
                _reqDefWithOneNumberFieldMock.Object);

            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.ActivePeriod.Status);
            Assert.IsFalse(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void RecordValues_ShouldNotMakeRequirementReadyToBePreserved_WhenRecordNaForSingleNumber()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithOneNumberFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField1Id, "na"}
                }, 
                null,
                _reqDefWithOneNumberFieldMock.Object);

            // Assert
            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.ActivePeriod.Status);
            Assert.IsFalse(dut.ReadyToBePreserved);
        }

        #endregion

        #region GetCurrentFieldValue

        [TestMethod]
        public void GetCurrentFieldValue_ShouldReturnNull_BeforeRecording()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithOneNumberFieldMock.Object);
            dut.StartPreservation();

            Assert.IsNull(dut.GetCurrentFieldValue(_numberField1Mock.Object));
        }

        [TestMethod]
        public void GetCurrentFieldValue_ShouldReturnNull_ForUnknownField()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithOneNumberFieldMock.Object);
            dut.StartPreservation();

            Assert.IsNull(dut.GetCurrentFieldValue(new Mock<Field>().Object));
        }

        [TestMethod]
        public void GetCurrentFieldValue_ShouldReturnCheckBoxValue_AfterRecordingCheckBoxTrue()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithNumberAndCheckBoxFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
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
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithNumberAndCheckBoxFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "true"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            // Assert
            Assert.IsNotNull(dut.GetCurrentFieldValue(_checkBoxFieldMock.Object));

            dut.RecordValues(
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
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithNumberAndCheckBoxFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField1Id, "123"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            // Assert
            var value = dut.GetCurrentFieldValue(_numberField1Mock.Object);

            Assert.IsNotNull(value);
            Assert.IsInstanceOfType(value, typeof(NumberValue));
        }

        [TestMethod]
        public void GetCurrentFieldValue_ShouldReturnNull_AfterRecordingNumber_ThenRecordNull()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithNumberAndCheckBoxFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField1Id, "123"}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            // Assert
            Assert.IsNotNull(dut.GetCurrentFieldValue(_numberField1Mock.Object));

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField1Id, null}
                }, 
                null,
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            // Assert
            Assert.IsNull(dut.GetCurrentFieldValue(_numberField1Mock.Object));
        }

        #endregion

        #region GetCurrentComment

        [TestMethod]
        public void GetCurrentComment_ShouldReturnNull_BeforeRecording()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithOneNumberFieldMock.Object);
            dut.StartPreservation();

            Assert.IsNull(dut.GetCurrentComment());
        }

        [TestMethod]
        public void GetCurrentComment_ShouldReturnComment()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithNumberAndCheckBoxFieldMock.Object);
            dut.StartPreservation();

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {CheckBoxFieldId, "true"}
                }, 
                "CommentA",
                _reqDefWithNumberAndCheckBoxFieldMock.Object);

            // Assert
            Assert.AreEqual("CommentA", dut.GetCurrentComment());
        }

        #endregion

        #region GetPreviousFieldValue

        [TestMethod]
        public void GetPreviousFieldValue_GetCurrentFieldValue_ShouldReturnDifferentValues_DuringRecordingAndPreserving()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithOneNumberFieldMock.Object);
            dut.StartPreservation();

            AssertNumber(null, dut.GetCurrentFieldValue(_numberField1Mock.Object));
            AssertNumber(null, dut.GetPreviousFieldValue(_numberField1Mock.Object));

            RecordAndPreseve(dut, 7, null);

             RecordAndPreseve(dut, 14.1, 7);

            RecordAndPreseve(dut, 200, 14.1);
        }

        private void RecordAndPreseve(
            Requirement dut,
            double numberToRecord,
            double? expectedPreviousRecorded)
        {
            _timeProvider.Elapse(TimeSpan.FromDays(5));

            dut.RecordValues(
                new Dictionary<int, string>
                {
                    {NumberField1Id, numberToRecord.ToString("F2")}
                },
                null,
                _reqDefWithOneNumberFieldMock.Object);

            AssertNumber(numberToRecord, dut.GetCurrentFieldValue(_numberField1Mock.Object));
            AssertNumber(expectedPreviousRecorded, dut.GetPreviousFieldValue(_numberField1Mock.Object));

            // preserve and get a new period
            dut.Preserve(new Mock<Person>().Object, false);

            AssertNumber(null, dut.GetCurrentFieldValue(_numberField1Mock.Object));
            AssertNumber(numberToRecord, dut.GetPreviousFieldValue(_numberField1Mock.Object));
        }

        private static void AssertNumber(double? expectedValue, FieldValue value)
        {
            if (expectedValue.HasValue)
            {
                Assert.IsNotNull(value);
                Assert.IsInstanceOfType(value, typeof(NumberValue));
                Assert.AreEqual(expectedValue, ((NumberValue)value).Value);
            }
            else
            {
                Assert.IsNull(value);
            }
        }


        [TestMethod]
        public void GetPreviousFieldValue_ShouldReturnNull_ForUnknownField()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithOneNumberFieldMock.Object);
            dut.StartPreservation();

            Assert.IsNull(dut.GetPreviousFieldValue(new Mock<Field>().Object));
        }

        #endregion

        [TestMethod]
        public void VoidUnVoid_ShouldToggleIsVoided()
        {
            var dut = new Requirement(TestPlant, TwoWeeksInterval, _reqDefWithCheckBoxFieldMock.Object);
            Assert.IsFalse(dut.IsVoided);

            dut.Void();
            Assert.IsTrue(dut.IsVoided);

            dut.UnVoid();
            Assert.IsFalse(dut.IsVoided);
        }
    }
}

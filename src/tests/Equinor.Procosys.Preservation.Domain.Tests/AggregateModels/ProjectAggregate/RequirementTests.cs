using System;
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
        private const int TwoWeeksInterval = 2;
        private Mock<RequirementDefinition> _reqDefNeedInputMock;
        private Mock<RequirementDefinition> _reqDefNotNeedInputMock;
        private DateTime _utcNow;

        [TestInitialize]
        public void Setup()
        {
            _reqDefNeedInputMock = new Mock<RequirementDefinition>();
            _reqDefNeedInputMock.SetupGet(x => x.Id).Returns(3);
            _reqDefNeedInputMock.Object.AddField(new Field("", "", FieldType.CheckBox, 0));
            _reqDefNotNeedInputMock = new Mock<RequirementDefinition>();
            _reqDefNotNeedInputMock.Object.AddField(new Field("", "", FieldType.Info, 0));
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNeedInputMock.Object);

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual(_reqDefNeedInputMock.Object.Id, dut.RequirementDefinitionId);
            Assert.IsFalse(dut.IsVoided);
            Assert.IsFalse(dut.ReadyToBePreserved);
            Assert.IsFalse(dut.IsReadyToBeBulkPreserved(_utcNow.AddWeeks(TwoWeeksInterval)));
        }

        [TestMethod]
        public void Constructor_ShouldNotSetActivePeriod()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNeedInputMock.Object);

            Assert.IsFalse(dut.HasActivePeriod);
            Assert.IsNull(dut.ActivePeriod);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementDefinitionNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new Requirement("SchemaA", 4, null)
            );

        [TestMethod]
        public void StartPreservation_ShouldShouldSetCorrectNextDueDate()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNeedInputMock.Object);

            dut.StartPreservation(_utcNow);

            var expectedNextDueTimeUtc = _utcNow.AddWeeks(TwoWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.NextDueTimeUtc);
        }

        [TestMethod]
        public void StartPreservation_ShouldSetActivePeriod()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNeedInputMock.Object);

            dut.StartPreservation(_utcNow);

            Assert.IsTrue(dut.HasActivePeriod);
            Assert.IsNotNull(dut.ActivePeriod);
        }

        [TestMethod]
        public void StartPreservation_ShouldNotSetReadyToBePreserved_WhenFieldNeedsInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNeedInputMock.Object);

            dut.StartPreservation(_utcNow);

            Assert.IsFalse(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void StartPreservation_ShouldSetReadyToBePreserved_WhenFieldNotNeedInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNotNeedInputMock.Object);

            dut.StartPreservation(_utcNow);

            Assert.IsTrue(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void IsReadyToBeBulkPreserved_ShouldBeFalse_BeforePeriod()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNotNeedInputMock.Object);
            dut.StartPreservation(_utcNow);
            Assert.IsTrue(dut.ReadyToBePreserved);

            var sameDayAsStart = _utcNow;

            Assert.IsFalse(dut.IsReadyToBeBulkPreserved(sameDayAsStart));
            Assert.AreEqual(2, dut.GetNextDueInWeeks(sameDayAsStart));
        }

        [TestMethod]
        public void IsReadyToBeBulkPreserved_ShouldBeTrue_InPeriod_WhenNotNeedInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNotNeedInputMock.Object);
            dut.StartPreservation(_utcNow);
            Assert.IsTrue(dut.ReadyToBePreserved);

            var spotOnTime = _utcNow.AddWeeks(TwoWeeksInterval);

            Assert.IsTrue(dut.IsReadyToBeBulkPreserved(spotOnTime));
            Assert.AreEqual(0, dut.GetNextDueInWeeks(spotOnTime));
        }

        [TestMethod]
        public void IsReadyToBeBulkPreserved_ShouldBeFalse_InPeriod_WhenNeedInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNeedInputMock.Object);
            dut.StartPreservation(_utcNow);
            Assert.IsFalse(dut.ReadyToBePreserved);

            var spotOnTime = _utcNow.AddWeeks(TwoWeeksInterval);

            Assert.IsFalse(dut.IsReadyToBeBulkPreserved(spotOnTime));
            Assert.AreEqual(0, dut.GetNextDueInWeeks(spotOnTime));
        }

        [TestMethod]
        public void IsReadyToBeBulkPreserved_ShouldBeTrue_OnOverdue_WhenNotNeedInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNotNeedInputMock.Object);
            dut.StartPreservation(_utcNow);
            Assert.IsTrue(dut.ReadyToBePreserved);

            var twoWeekOverdue = _utcNow.AddWeeks(TwoWeeksInterval + TwoWeeksInterval);

            Assert.IsTrue(dut.IsReadyToBeBulkPreserved(twoWeekOverdue));
            Assert.AreEqual(-2, dut.GetNextDueInWeeks(twoWeekOverdue));
        }

        [TestMethod]
        public void IsReadyToBeBulkPreserved_ShouldBeFalse_OnOverdue_WhenNeedInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNeedInputMock.Object);
            dut.StartPreservation(_utcNow);
            Assert.IsFalse(dut.ReadyToBePreserved);

            var twoWeekOverdue = _utcNow.AddWeeks(TwoWeeksInterval+TwoWeeksInterval);

            Assert.IsFalse(dut.IsReadyToBeBulkPreserved(twoWeekOverdue));
            Assert.AreEqual(-2, dut.GetNextDueInWeeks(twoWeekOverdue));
        }

        [TestMethod]
        public void StartPreservation_ShouldNotSetReadyToBeBulkPreserved_EvenWhenFieldNotNeedInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNotNeedInputMock.Object);

            dut.StartPreservation(_utcNow);

            Assert.IsFalse(dut.IsReadyToBeBulkPreserved(_utcNow.AddDays(2)));
        }

        [TestMethod]
        public void StartPreservation_ShouldThrowException_WhenPreservationAlreadyActive()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNeedInputMock.Object);

            dut.StartPreservation(_utcNow);

            Assert.ThrowsException<Exception>(() => dut.StartPreservation(_utcNow)
            );
        }

        [TestMethod]
        public void StartPreservation_ShouldAddNewPreservationPeriodWithCorrectDueDate()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNeedInputMock.Object);

            dut.StartPreservation(_utcNow);

            var expectedNextDueTimeUtc = _utcNow.AddWeeks(TwoWeeksInterval);
            Assert.AreEqual(1, dut.PreservationPeriods.Count);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.PreservationPeriods.First().DueTimeUtc);
        }

        [TestMethod]
        public void StartPreservation_ShouldAddNewPreservationPeriodWithoutPreservationRecord()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNeedInputMock.Object);

            dut.StartPreservation(_utcNow);

            Assert.IsNull(dut.PreservationPeriods.First().PreservationRecord);
        }

        [TestMethod]
        public void StartPreservation_ShouldAddNewPreservationPeriodWithStatusNeedsUserInput_WhenReqDefNeedsUserInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNeedInputMock.Object);

            dut.StartPreservation(_utcNow);

            Assert.AreEqual(PreservationPeriodStatus.NeedsUserInput, dut.PreservationPeriods.First().Status);
        }

        [TestMethod]
        public void StartPreservation_ShouldAddNewPreservationPeriodWithStatusReadyToBePreserved_WhenReqDefNotNeedsUserInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNotNeedInputMock.Object);

            dut.StartPreservation(_utcNow);

            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.PreservationPeriods.First().Status);
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservationNotStarted()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNotNeedInputMock.Object);

            Assert.ThrowsException<Exception>(() =>
                dut.Preserve(_utcNow, new Mock<Person>().Object, false)
            );
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservationPeriodNeedsInput()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNeedInputMock.Object);
            dut.StartPreservation(_utcNow);

            Assert.ThrowsException<Exception>(() =>
                dut.Preserve(_utcNow, new Mock<Person>().Object, false)
            );
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservedByNoGiven()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNotNeedInputMock.Object);
            dut.StartPreservation(_utcNow);

            Assert.ThrowsException<ArgumentNullException>(() =>
                dut.Preserve(_utcNow, null, false)
            );
        }
                
        [TestMethod]
        public void Preserve_ShouldShouldUpdateCorrectNextDueDate()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNotNeedInputMock.Object);

            dut.StartPreservation(_utcNow);

            var preservedTime = _utcNow.AddDays(5);
            dut.Preserve(preservedTime, new Mock<Person>().Object, false);
            
            var expectedNextDueTimeUtc = preservedTime.AddWeeks(TwoWeeksInterval);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.NextDueTimeUtc);
        }

        [TestMethod]
        public void Preserve_ShouldSetStatusPreserveOnReadyPreservationPeriod()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNotNeedInputMock.Object);
            dut.StartPreservation(_utcNow);

            dut.Preserve(_utcNow, new Mock<Person>().Object, false);

            Assert.AreEqual(PreservationPeriodStatus.Preserved, dut.PreservationPeriods.First().Status);
        }

        [TestMethod]
        public void Preserve_ShouldCreatePreservationRecordOnReadyPreservationPeriod()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNotNeedInputMock.Object);
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
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNotNeedInputMock.Object);
            dut.StartPreservation(_utcNow);
            dut.Preserve(_utcNow, new Mock<Person>().Object, true);

            Assert.IsTrue(dut.PreservationPeriods.First().PreservationRecord.BulkPreserved);
        }

        [TestMethod]
        public void Preserve_ShouldCreatePreservationRecordWithoutBulk_WhenNotBulkPreserve()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNotNeedInputMock.Object);
            dut.StartPreservation(_utcNow);
            dut.Preserve(_utcNow, new Mock<Person>().Object, false);

            Assert.IsFalse(dut.PreservationPeriods.First().PreservationRecord.BulkPreserved);
        }

        [TestMethod]
        public void Preserve_ShouldAddNewPreservationPeriodToPreservationPeriodsList()
        {
            var intervalWeeks = 2;
            var dut = new Requirement("SchemaA", intervalWeeks, _reqDefNotNeedInputMock.Object);
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
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNotNeedInputMock.Object);
            dut.StartPreservation(_utcNow);

            var preserveCount = 15;
            for (var i = 0; i < preserveCount; i++)
            {
                dut.Preserve(_utcNow, new Mock<Person>().Object, false);
            }
            
            Assert.AreEqual(preserveCount+1, dut.PreservationPeriods.Count);
        }

        [TestMethod]
        public void VoidUnVoid_ShouldToggleIsVoided()
        {
            var dut = new Requirement("SchemaA", TwoWeeksInterval, _reqDefNeedInputMock.Object);
            Assert.IsFalse(dut.IsVoided);

            dut.Void();
            Assert.IsTrue(dut.IsVoided);

            dut.UnVoid();
            Assert.IsFalse(dut.IsVoided);
        }
    }
}

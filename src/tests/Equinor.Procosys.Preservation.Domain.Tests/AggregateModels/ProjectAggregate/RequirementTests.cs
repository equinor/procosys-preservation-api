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
        private Mock<RequirementDefinition> _reqDefMock;
        private DateTime _utcNow;

        [TestInitialize]
        public void Setup()
        {
            _reqDefMock = new Mock<RequirementDefinition>();
            _reqDefMock.SetupGet(x => x.Id).Returns(3);
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new Requirement("SchemaA", 24, _reqDefMock.Object);

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual(_reqDefMock.Object.Id, dut.RequirementDefinitionId);
            Assert.IsFalse(dut.IsVoided);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementDefinitionNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new Requirement("SchemaA", 4, null)
            );

        [TestMethod]
        public void StartPreservation_ShouldShouldSetCorrectNextDueDate()
        {
            var intervalWeeks = 2;
            var dut = new Requirement("SchemaA", intervalWeeks, _reqDefMock.Object);

            dut.StartPreservation(_utcNow);

            var expectedNextDueTimeUtc = _utcNow.AddWeeks(intervalWeeks);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.NextDueTimeUtc);
        }

        [TestMethod]
        public void StartPreservation_ShouldAddNewPreservationPeriodWithCorrectDueDate()
        {
            var intervalWeeks = 2;
            var dut = new Requirement("SchemaA", intervalWeeks, _reqDefMock.Object);

            dut.StartPreservation(_utcNow);

            var expectedNextDueTimeUtc = _utcNow.AddWeeks(intervalWeeks);
            Assert.AreEqual(1, dut.PreservationPeriods.Count);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.PreservationPeriods.First().DueTimeUtc);
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenPreservationNotReady()
        {
            var dut = new Requirement("SchemaA", 24, _reqDefMock.Object);

            Assert.ThrowsException<Exception>(() =>
                dut.Preserve(_utcNow, new Mock<Person>().Object, false)
            );
        }

        [TestMethod]
        public void Preserve_ShouldPreserveActivePreservationPeriod()
        {
            var dut = new Requirement("SchemaA", 24, _reqDefMock.Object);
            dut.StartPreservation(_utcNow);

            dut.Preserve(_utcNow, new Mock<Person>().Object, false);

            Assert.AreEqual(PreservationPeriodStatus.Preserved, dut.PreservationPeriods.First().Status);
        }

        [TestMethod]
        public void Preserve_ShouldAddNewPreservationPeriodToPreservationPeriodsList()
        {
            var intervalWeeks = 2;
            var dut = new Requirement("SchemaA", intervalWeeks, _reqDefMock.Object);
            dut.StartPreservation(_utcNow);

            var preservatedTime = _utcNow.AddDays(5);
            dut.Preserve(preservatedTime, new Mock<Person>().Object, false);
            
            var expectedNextDueTimeUtc = preservatedTime.AddWeeks(intervalWeeks);
            Assert.AreEqual(2, dut.PreservationPeriods.Count);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.PreservationPeriods.Last().DueTimeUtc);
        }
        
        [TestMethod]
        public void Preserve_ShouldShouldUpdateCorrectNextDueDate()
        {
            var intervalWeeks = 2;
            var dut = new Requirement("SchemaA", intervalWeeks, _reqDefMock.Object);

            dut.StartPreservation(_utcNow);

            var preservatedTime = _utcNow.AddDays(5);
            dut.Preserve(preservatedTime, new Mock<Person>().Object, false);
            
            var expectedNextDueTimeUtc = preservatedTime.AddWeeks(intervalWeeks);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.NextDueTimeUtc);
        }

        [TestMethod]
        public void VoidUnVoid_ShouldToggleIsVoided()
        {
            var dut = new Requirement("SchemaA", 24, _reqDefMock.Object);
            Assert.IsFalse(dut.IsVoided);

            dut.Void();
            Assert.IsTrue(dut.IsVoided);

            dut.UnVoid();
            Assert.IsFalse(dut.IsVoided);
        }
    }
}

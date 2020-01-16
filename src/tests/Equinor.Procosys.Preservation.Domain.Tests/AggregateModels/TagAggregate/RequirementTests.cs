using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.TagAggregate
{
    [TestClass]
    public class RequirementTests
    {
        private Mock<RequirementDefinition> _reqDefMock;

        [TestInitialize]
        public void Setup()
        {
            _reqDefMock = new Mock<RequirementDefinition>();
            _reqDefMock.SetupGet(x => x.Id).Returns(3);
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
        public void AddPreservationRecord_ShouldAddPreservationRecordToPreservationRecordsList()
        {
            var dut = new Requirement("SchemaA", 24, _reqDefMock.Object);
            var pr = new Mock<PreservationRecord>();

            dut.AddPreservationRecord(pr.Object);

            Assert.AreEqual(1, dut.PreservationRecords.Count);
            Assert.IsTrue(dut.PreservationRecords.Contains(pr.Object));
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
        
        [TestMethod]
        public void SetNextDueTimeUtc_ShouldSetCorrectNextDueDate()
        {
            var utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            var intervalWeeks = 8;
            var dut = new Requirement("SchemaA", intervalWeeks, _reqDefMock.Object);

            dut.SetNextDueTimeUtc(utcNow);

            var expectedNextDueTimeUtc = utcNow.AddWeeks(intervalWeeks);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.NextDueTimeUtc);
        }

        [TestMethod]
        public void SetNextDueTimeUtc_ShouldThrowException_WhenTimeNotGivenInUtc()
        {
            var dut = new Requirement("SchemaA", 24, _reqDefMock.Object);
            var now = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Local);
            Assert.ThrowsException<ArgumentException>(() =>
                dut.SetNextDueTimeUtc(now)
            );
        }

    }
}

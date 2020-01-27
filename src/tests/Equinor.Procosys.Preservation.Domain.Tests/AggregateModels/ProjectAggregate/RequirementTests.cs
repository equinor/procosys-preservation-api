using System;
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

            dut.StartPreservation(_utcNow, PreservationPeriodStatus.ReadyToPreserve);

            var expectedNextDueTimeUtc = _utcNow.AddWeeks(intervalWeeks);
            Assert.AreEqual(expectedNextDueTimeUtc, dut.NextDueTimeUtc);
        }

        // todo
        //[TestMethod]
        //public void Preserve_ShouldAddPreservationRecordToPreservationRecordsList()
        //{
        //    var dut = new Requirement("SchemaA", 24, _reqDefMock.Object);
        //    var pr = new Mock<PreservationRecord>();

        //    dut.Preserve(pr.Object);

        //    Assert.AreEqual(1, dut.PreservationRecords.Count);
        //    Assert.IsTrue(dut.PreservationRecords.Contains(pr.Object));
        //}
        
        //[TestMethod]
        //public void Preserve_ShouldSetCorrectNextDueDate()
        //{
        //    var intervalWeeks = 8;
        //    var p = new Mock<Person>();

        //    var dut = new Requirement("SchemaA", intervalWeeks, _reqDefMock.Object);
        //    var pr = new PreservationRecord("", _utcNow, p.Object, false, "");
        //    dut.Preserve(pr);

        //    var expectedNextDueTimeUtc = _utcNow.AddWeeks(intervalWeeks);
        //    Assert.AreEqual(expectedNextDueTimeUtc, dut.NextDueTimeUtc);
        //}

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

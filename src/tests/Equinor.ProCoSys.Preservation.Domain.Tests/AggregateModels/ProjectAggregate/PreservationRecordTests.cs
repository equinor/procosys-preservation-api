using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using HeboTech.TimeService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class PreservationRecordTests
    {
        private int _preservedById = 99;
        private Mock<Person> _preservedByMock;

        [TestInitialize]
        public void Setup()
        {
            TimeService.SetConstant(new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc));
            _preservedByMock = new Mock<Person>();
            _preservedByMock.SetupGet(p => p.Id).Returns(_preservedById);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new PreservationRecord("PlantA", _preservedByMock.Object, true);

            Assert.AreEqual("PlantA", dut.Plant);
            Assert.AreEqual(TimeService.Now, dut.PreservedAtUtc);
            Assert.AreEqual(_preservedById, dut.PreservedById);
            Assert.IsTrue(dut.BulkPreserved);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenPreservedByNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new PreservationRecord("PlantA", null, true)
            );
    }
}

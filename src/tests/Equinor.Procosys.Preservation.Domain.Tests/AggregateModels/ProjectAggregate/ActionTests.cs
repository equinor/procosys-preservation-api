using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class ActionTests
    {
        private const int ClearedById = 31;
        private Mock<Person> _clearedByMock;
        private DateTime _utcNow;
        private Action _dut;

        [TestInitialize]
        public void Setup()
        {
            _clearedByMock = new Mock<Person>();
            _clearedByMock.SetupGet(p => p.Id).Returns(ClearedById);
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _dut = new Action("SchemaA", "DescA", _utcNow);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WithoutDue()
        {
            _dut = new Action("SchemaA", "DescA", null);

            Assert.AreEqual("SchemaA", _dut.Schema);
            Assert.IsFalse(_dut.DueTimeUtc.HasValue);
            Assert.IsFalse(_dut.ClearedById.HasValue);
            Assert.IsNull(_dut.ClearedAtUtc);
            Assert.AreEqual("DescA", _dut.Description);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WithDue()
        {
            Assert.AreEqual("SchemaA", _dut.Schema);
            Assert.AreEqual(_utcNow, _dut.DueTimeUtc);
            Assert.AreEqual("DescA", _dut.Description);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenDueIsNotUtc()
            => Assert.ThrowsException<ArgumentException>(() =>
                new Action("SchemaA", "DescA", DateTime.Now)
            );

        [TestMethod]
        public void SetDueTime_ShouldSetDueDate()
        {
            var newDue = _utcNow.AddDays(3);
            _dut.SetDueTime(newDue);

            Assert.AreEqual(newDue, _dut.DueTimeUtc);
        }

        [TestMethod]
        public void SetDueTime_ShouldSetDueDateNull()
        {
            Assert.IsNotNull(_dut.DueTimeUtc);
            
            _dut.SetDueTime(null);

            Assert.IsNull(_dut.DueTimeUtc);
        }

        [TestMethod]
        public void Clear_Unclear_ShouldToggleClearedData()
        {
            _dut.Clear(_utcNow, _clearedByMock.Object);

            Assert.AreEqual(ClearedById, _dut.ClearedById);
            Assert.AreEqual(_utcNow, _dut.ClearedAtUtc);

            _dut.Unclear();

            Assert.IsNull(_dut.ClearedById);
            Assert.IsNull(_dut.ClearedAtUtc);
        }
    }
}

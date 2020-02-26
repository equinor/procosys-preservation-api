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
        private const int PersonId = 31;
        private Mock<Person> _personMock;
        private DateTime _utcNow;
        private Action _dut;

        [TestInitialize]
        public void Setup()
        {
            _personMock = new Mock<Person>();
            _personMock.SetupGet(p => p.Id).Returns(PersonId);
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _dut = new Action("SchemaA", "TitleA", "DescA", _utcNow);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WithoutDue()
        {
            _dut = new Action("SchemaA", "TitleA", "DescA", null);

            Assert.AreEqual("SchemaA", _dut.Schema);
            Assert.IsFalse(_dut.DueTimeUtc.HasValue);
            Assert.IsFalse(_dut.ClosedById.HasValue);
            Assert.IsNull(_dut.ClosedAtUtc);
            Assert.AreEqual("TitleA", _dut.Title);
            Assert.AreEqual("DescA", _dut.Description);
            Assert.IsFalse(_dut.IsClosed);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WithDue()
        {
            Assert.AreEqual("SchemaA", _dut.Schema);
            Assert.AreEqual(_utcNow, _dut.CreatedAtUtc);
            Assert.AreEqual(PersonId, _dut.CreatedById);
            Assert.AreEqual(_utcNow, _dut.DueTimeUtc);
            Assert.AreEqual("TitleA", _dut.Title);
            Assert.AreEqual("DescA", _dut.Description);
            Assert.IsFalse(_dut.IsClosed);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenCreatedIsNotUtc()
            => Assert.ThrowsException<ArgumentException>(() =>
                new Action("SchemaA", "", "", _utcNow)
            );

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenCreatedByIsNull()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new Action("SchemaA", "", "", _utcNow)
            );

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenDueIsNotUtc()
            => Assert.ThrowsException<ArgumentException>(() =>
                new Action("SchemaA", "", "", DateTime.Now)
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
        public void SetDueTime_ShouldThrowException_WhenDueIsNotUtc()
            => Assert.ThrowsException<ArgumentException>(()
                => _dut.SetDueTime(DateTime.Now));

        [TestMethod]
        public void SetDueTime_ShouldThrowException_WhenIsClosed()
        {
            _dut.Close(_utcNow, _personMock.Object);
            Assert.ThrowsException<Exception>(() => _dut.SetDueTime(_utcNow));
        }

        [TestMethod]
        public void Close_ShouldSetClosedData()
        {
            _dut.Close(_utcNow, _personMock.Object);

            Assert.AreEqual(PersonId, _dut.ClosedById);
            Assert.AreEqual(_utcNow, _dut.ClosedAtUtc);
            Assert.IsTrue(_dut.IsClosed);
        }
    }
}

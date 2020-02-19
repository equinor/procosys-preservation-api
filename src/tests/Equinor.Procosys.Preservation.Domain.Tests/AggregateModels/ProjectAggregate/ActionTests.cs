using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
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
            _dut = new Action("SchemaA", "DescA", _utcNow);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WithoutDue()
        {
            _dut = new Action("SchemaA", "DescA", null);

            Assert.AreEqual("SchemaA", _dut.Schema);
            Assert.IsFalse(_dut.DueTimeUtc.HasValue);
            Assert.IsFalse(_dut.ClosedById.HasValue);
            Assert.IsNull(_dut.ClosedAtUtc);
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
        public void Close_ShouldSetClosedData()
        {
            _dut.Close(_utcNow, _personMock.Object);

            Assert.AreEqual(PersonId, _dut.ClosedById);
            Assert.AreEqual(_utcNow, _dut.ClosedAtUtc);
        }

        [TestMethod]
        public void AddComment_ShouldAddComment()
        {
            var comment = new ActionComment("", "", _utcNow, _personMock.Object);
            _dut.AddComment(comment);

            Assert.AreEqual(comment, _dut.ActionComments.First());
        }

        [TestMethod]
        public void AddComment_ShouldThrowException_WhenCommentNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dut.AddComment(null));
    }
}

using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class ActionCommentTests
    {
        private const int CommentedById = 31;
        private Mock<Person> _commentedByMock;
        private DateTime _utcNow;
        private ActionComment _dut;

        [TestInitialize]
        public void Setup()
        {
            _commentedByMock = new Mock<Person>();
            _commentedByMock.SetupGet(p => p.Id).Returns(CommentedById);
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _dut = new ActionComment("SchemaA", "CommentA", _utcNow, _commentedByMock.Object);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual("SchemaA", _dut.Schema);
            Assert.AreEqual(_utcNow, _dut.CommentedAtUtc);
            Assert.AreEqual("CommentA", _dut.Comment);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenTimeIsNotUtc()
            => Assert.ThrowsException<ArgumentException>(() =>
                new ActionComment("", "", DateTime.Now, _commentedByMock.Object)
            );

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenPersonIsNull()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new ActionComment("", "", _utcNow, null)
            );
    }
}

using System;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Time;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class ActionTests
    {
        private const int PersonId = 31;
        private const string TestPlant = "PCS$PlantA";
        private Mock<Person> _personMock;
        private DateTime _utcNow;
        private Action _dut;
        private ManualTimeProvider _timeProvider;

        [TestInitialize]
        public void Setup()
        {
            _personMock = new Mock<Person>();
            _personMock.SetupGet(p => p.Id).Returns(PersonId);
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _timeProvider = new ManualTimeProvider(_utcNow);
            TimeService.SetProvider(_timeProvider);
            _dut = new Action(TestPlant, "TitleA", "DescA", _utcNow);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WithoutDue()
        {
            _dut = new Action(TestPlant, "TitleA", "DescA", null);

            Assert.AreEqual(TestPlant, _dut.Plant);
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
            Assert.AreEqual(TestPlant, _dut.Plant);
            Assert.AreEqual(_utcNow, _dut.DueTimeUtc);
            Assert.AreEqual("TitleA", _dut.Title);
            Assert.AreEqual("DescA", _dut.Description);
            Assert.IsFalse(_dut.IsClosed);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenDueIsNotUtc()
            => Assert.ThrowsException<ArgumentException>(() =>
                new Action(TestPlant, "", "", DateTime.Now)
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

        [TestMethod]
        public void AddAttachment_ShouldAddAttachment()
        {
            var attachment = new ActionAttachment(TestPlant, Guid.Empty, "A.txt");
            _dut.AddAttachment(attachment);

            Assert.AreEqual(attachment, _dut.Attachments.First());
        }

        [TestMethod]
        public void AddAttachment_ShouldThrowException_WhenAttachmentNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dut.AddAttachment(null));

        [TestMethod]
        public void GetAttachmentByFileName_ShouldGetAttachmentWhenExists()
        {
            // Arrange
            var fileName = "FileA";
            var attachment = new ActionAttachment(TestPlant, Guid.Empty, fileName);
            _dut.AddAttachment(attachment);

            // Act
            var result = _dut.GetAttachmentByFileName(fileName);

            // Arrange
            Assert.AreEqual(attachment, result);
        }

        [TestMethod]
        public void GetAttachmentByFileName_ShouldGetAttachmentWhenExists_RegardlessOfCasing()
        {
            // Arrange
            var fileName = "FileA";
            var attachment = new ActionAttachment(TestPlant, Guid.Empty, fileName);
            _dut.AddAttachment(attachment);

            // Act
            var result = _dut.GetAttachmentByFileName(fileName.ToUpper());

            // Arrange
            Assert.AreEqual(attachment, result);
        }

        [TestMethod]
        public void GetAttachmentByFileName_ShouldReturnNullWhenNotFound()
        {
            // Act
            var result = _dut.GetAttachmentByFileName("FileA");

            // Arrange
            Assert.IsNull(result);
        }

        [TestMethod]
        public void RemoveAttachment_ShouldRemoveAttachment()
        {
            var attachment = new ActionAttachment(TestPlant, Guid.Empty, "A.txt");
            _dut.AddAttachment(attachment);
            Assert.AreEqual(1, _dut.Attachments.Count);

            // Act
            _dut.RemoveAttachment(attachment);

            Assert.AreEqual(0, _dut.Attachments.Count);
        }

        [TestMethod]
        public void RemoveAttachment_ShouldThrowException_WhenAttachmentNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dut.RemoveAttachment(null));

        [TestMethod]
        public void IsOverDue_ShouldBeFalseWhenNoDueDate()
        {
            // Arrange
            _dut = new Action(TestPlant, "TitleA", "DescA", null);

            // Act
            var overDue = _dut.IsOverDue();

            // Assert
            Assert.IsFalse(overDue);
        }

        [TestMethod]
        public void IsOverDue_ShouldBeFalseWhenDueDateExactNow()
        {
            // Arrange
            _dut = new Action(TestPlant, "TitleA", "DescA", _utcNow);

            // Act
            var overDue = _dut.IsOverDue();

            // Assert
            Assert.IsFalse(overDue);
        }

        [TestMethod]
        public void IsOverDue_ShouldBeFalseWhenDueDateInFuture()
        {
            // Arrange
            _dut = new Action(TestPlant, "TitleA", "DescA", _utcNow.AddHours(1));

            // Act
            var overDue = _dut.IsOverDue();

            // Assert
            Assert.IsFalse(overDue);
        }

        [TestMethod]
        public void IsOverDue_ShouldBeTrueWhenDueDateInPast()
        {
            // Arrange
            _dut = new Action(TestPlant, "TitleA", "DescA", _utcNow);
            _timeProvider.Elapse(new TimeSpan(1, 0, 0));

            // Act
            var overDue = _dut.IsOverDue();

            // Assert
            Assert.IsTrue(overDue);
        }

        [TestMethod]
        public void IsOverDue_ShouldBeFalseWhenDueDateInPast_ButClosed()
        {
            // Arrange
            _dut = new Action(TestPlant, "TitleA", "DescA", _utcNow);
            _timeProvider.Elapse(new TimeSpan(1, 0, 0));
            _dut.Close(_utcNow, _personMock.Object);

            // Act
            var overDue = _dut.IsOverDue();

            // Assert
            Assert.IsFalse(overDue);
        }
    }
}

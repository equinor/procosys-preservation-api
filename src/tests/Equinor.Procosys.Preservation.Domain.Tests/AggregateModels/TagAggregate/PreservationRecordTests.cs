using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.TagAggregate
{
    [TestClass]
    public class PreservationRecordTests
    {
        private Mock<ITimeService> _tsMock;
        private DateTime _utcNow;
        private Mock<Requirement> _reqMock;

        [TestInitialize]
        public void Setup()
        {
            _utcNow = DateTime.UtcNow;
            _tsMock = new Mock<ITimeService>();
            _tsMock.Setup(s => s.GetCurrentTimeUtc()).Returns(_utcNow);
            _reqMock = new Mock<Requirement>();
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            _reqMock.SetupGet(r => r.Id).Returns(3);

            var req = new PreservationRecord("SchemaA", _reqMock.Object, _tsMock.Object);

            Assert.AreEqual("SchemaA", req.Schema);
            Assert.AreEqual(_reqMock.Object.Id, req.RequirementId);
            Assert.IsFalse(req.BulkPreserved.HasValue);
            Assert.IsFalse(req.Preserved.HasValue);
            Assert.IsFalse(req.PreservedBy.HasValue);
        }
        [TestMethod]

        public void Constructor_ShouldSetCorrectNextDueDateBasedOnRequirement()
        {
            var rdMock = new Mock<RequirementDefinition>();
            var intervalWeeks = 8;
            var r = new Requirement("SchemaA", intervalWeeks, rdMock.Object);

            var req = new PreservationRecord("SchemaA", r, _tsMock.Object);

            var expectedNextDueTime = _utcNow.AddDays(7 * intervalWeeks);
            Assert.AreEqual(expectedNextDueTime, req.NextDueTime);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new PreservationRecord("SchemaA", null, _tsMock.Object)
            );

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenTimeServiceNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() =>
                new PreservationRecord("SchemaA", _reqMock.Object, null)
            );
    }
}

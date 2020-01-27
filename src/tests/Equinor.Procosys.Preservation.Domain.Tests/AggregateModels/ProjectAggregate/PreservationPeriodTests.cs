using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class PreservationPeriodTests
    {
        private DateTime _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.ReadyToBePreserved);

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual(_utcNow, dut.DueTimeUtc);
            Assert.AreEqual(PreservationPeriodStatus.ReadyToBePreserved, dut.Status);
        }

        [TestMethod]
        public void Constructor_ShouldAllowStatusNeedUserInput()
        {
            var dut = new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.NeedUserInput);

            Assert.AreEqual(PreservationPeriodStatus.NeedUserInput, dut.Status);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenStatusIsPreserved()
            => Assert.ThrowsException<ArgumentException>(() =>
                new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.Preserved)
            );

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenStatusIsStopped()
            => Assert.ThrowsException<ArgumentException>(() =>
                new PreservationPeriod("SchemaA", _utcNow, PreservationPeriodStatus.Stopped)
            );

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenDateNotUtc()
            => Assert.ThrowsException<ArgumentException>(() =>
                new PreservationPeriod("SchemaA", DateTime.Now, PreservationPeriodStatus.NeedUserInput)
            );
    }
}

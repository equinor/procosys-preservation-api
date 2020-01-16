using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.JourneyAggregate
{
    [TestClass]
    public class JourneyTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new Journey("SchemaA", "TitleA");

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual("TitleA", dut.Title);
            Assert.AreEqual(0, dut.Steps.Count);
        }

        [TestMethod]
        public void AddStep_ShouldThrowException_WhenStepNotGiven()
        {
            var dut = new Journey("", "");

            Assert.ThrowsException<ArgumentNullException>(() =>
                dut.AddStep(null));
            Assert.AreEqual(0, dut.Steps.Count);
        }

        [TestMethod]
        public void AddStep_ShouldAddStepToStepsList()
        {
            var dut = new Journey("", "");
            var step = new Mock<Step>();

            dut.AddStep(step.Object);

            Assert.AreEqual(1, dut.Steps.Count);
            Assert.IsTrue(dut.Steps.Contains(step.Object));
        }

        [TestMethod]
        public void VoidUnVoid_ShouldToggleIsVoided()
        {
            var dut = new Journey("", "");
            Assert.IsFalse(dut.IsVoided);

            dut.Void();
            Assert.IsTrue(dut.IsVoided);

            dut.UnVoid();
            Assert.IsFalse(dut.IsVoided);
        }
    }
}

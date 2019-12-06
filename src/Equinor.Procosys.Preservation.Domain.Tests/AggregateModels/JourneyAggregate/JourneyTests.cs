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
        public void ConstructorSetsPropertiesTest()
        {
            Journey dut = new Journey("SchemaA", "TitleA");

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual("TitleA", dut.Title);
        }

        [TestMethod]
        public void AddingEmptyStepThrowsExceptionTest()
        {
            Journey dut = new Journey("", "");

            Assert.ThrowsException<ArgumentNullException>(() =>
                dut.AddStep(null)
                );
        }

        [TestMethod]
        public void StepIsAddedToStepsListTest()
        {
            Journey dut = new Journey("", "");
            var step = new Mock<Step>();

            dut.AddStep(step.Object);

            Assert.IsTrue(dut.Steps.Contains(step.Object));
        }
    }
}

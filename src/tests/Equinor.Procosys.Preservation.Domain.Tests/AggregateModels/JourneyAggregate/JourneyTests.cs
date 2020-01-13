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
            var j = new Journey("SchemaA", "TitleA");

            Assert.AreEqual("SchemaA", j.Schema);
            Assert.AreEqual("TitleA", j.Title);
            Assert.AreEqual(0, j.Steps.Count);
        }

        [TestMethod]
        public void AddingEmptyStepThrowsExceptionTest()
        {
            var j = new Journey("", "");

            Assert.ThrowsException<ArgumentNullException>(() =>
                j.AddStep(null));
            Assert.AreEqual(0, j.Steps.Count);
        }

        [TestMethod]
        public void StepIsAddedToStepsListTest()
        {
            var j = new Journey("", "");
            var step = new Mock<Step>();

            j.AddStep(step.Object);

            Assert.AreEqual(1, j.Steps.Count);
            Assert.IsTrue(j.Steps.Contains(step.Object));
        }
    }
}

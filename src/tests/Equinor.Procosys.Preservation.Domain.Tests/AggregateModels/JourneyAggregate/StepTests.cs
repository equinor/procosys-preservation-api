using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.JourneyAggregate
{
    [TestClass]
    public class StepTests
    {
        [TestMethod]
        public void ConstructorSetsPropertiesTest()
        {
            var mode = new Mock<Mode>();
            mode.SetupGet(x => x.Id).Returns(3);

            var responsible = new Mock<Responsible>();
            responsible.SetupGet(x => x.Id).Returns(4);

            var dut = new Step("SchemaA", mode.Object, responsible.Object);

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual(dut.ModeId, mode.Object.Id);
            Assert.AreEqual(dut.ResponsibleId, responsible.Object.Id);
        }

        [TestMethod]
        public void ConstructorWithNullModeThrowsExceptionTest()
        {
            var responsible = new Mock<Responsible>();
            responsible.SetupGet(x => x.Id).Returns(4);

            Assert.ThrowsException<ArgumentNullException>(() =>
                new Step("SchemaA", null, responsible.Object)
                );
        }

        [TestMethod]
        public void ConstructorWithNullResponsibleThrowsExceptionTest()
        {
            var mode = new Mock<Mode>();

            Assert.ThrowsException<ArgumentNullException>(() =>
                new Step("SchemaA", mode.Object, null)
                );
        }
    }
}

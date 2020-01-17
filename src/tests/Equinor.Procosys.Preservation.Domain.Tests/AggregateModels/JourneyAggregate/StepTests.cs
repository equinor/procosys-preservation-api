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
        private Mock<Mode> _modeMock;
        private Mock<Responsible> _responsibleMock;

        [TestInitialize]
        public void Setup()
        {
            _modeMock = new Mock<Mode>();
            _modeMock.SetupGet(x => x.Id).Returns(3);

            _responsibleMock = new Mock<Responsible>();
            _responsibleMock.SetupGet(x => x.Id).Returns(4);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new Step("SchemaA", _modeMock.Object, _responsibleMock.Object);

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual(_modeMock.Object.Id, dut.ModeId);
            Assert.AreEqual(_responsibleMock.Object.Id, dut.ResponsibleId);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenModeNotGiven()
        {
            var responsible = new Mock<Responsible>();
            responsible.SetupGet(x => x.Id).Returns(4);

            Assert.ThrowsException<ArgumentNullException>(() =>
                new Step("SchemaA", null, responsible.Object)
                );
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenResponsibleNotGiven()
        {
            var mode = new Mock<Mode>();

            Assert.ThrowsException<ArgumentNullException>(() =>
                new Step("SchemaA", mode.Object, null)
                );
        }

        [TestMethod]
        public void VoidUnVoid_ShouldToggleIsVoided()
        {
            var dut = new Step("SchemaA", _modeMock.Object, _responsibleMock.Object);
            Assert.IsFalse(dut.IsVoided);

            dut.Void();
            Assert.IsTrue(dut.IsVoided);

            dut.UnVoid();
            Assert.IsFalse(dut.IsVoided);
        }
    }
}

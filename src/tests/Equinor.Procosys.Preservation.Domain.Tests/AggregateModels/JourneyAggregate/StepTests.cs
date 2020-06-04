using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.JourneyAggregate
{
    [TestClass]
    public class StepTests
    {
        private const string TestPlant = "PlantA";
        private Mock<Mode> _modeMock;
        private Mock<Responsible> _responsibleMock;
        private Step _dut;

        [TestInitialize]
        public void Setup()
        {
            _modeMock = new Mock<Mode>();
            _modeMock.SetupGet(x => x.Id).Returns(3);
            _modeMock.SetupGet(x => x.Plant).Returns(TestPlant);

            _responsibleMock = new Mock<Responsible>();
            _responsibleMock.SetupGet(x => x.Id).Returns(4);
            _responsibleMock.SetupGet(x => x.Plant).Returns(TestPlant);

            _dut = new Step(TestPlant, "S", _modeMock.Object, _responsibleMock.Object);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual(TestPlant, _dut.Plant);
            Assert.AreEqual("S", _dut.Title);
            Assert.AreEqual(_modeMock.Object.Id, _dut.ModeId);
            Assert.AreEqual(_responsibleMock.Object.Id, _dut.ResponsibleId);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenTitleNotGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() =>
                new Step(TestPlant, null, _modeMock.Object, _responsibleMock.Object)
            );

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenModeNotGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() =>
                new Step(TestPlant, "S", null, _responsibleMock.Object)
            );

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenResponsibleNotGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() =>
                new Step(TestPlant, "S", _modeMock.Object, null)
            );

        [TestMethod]
        public void VoidUnVoid_ShouldToggleIsVoided()
        {
            Assert.IsFalse(_dut.IsVoided);

            _dut.Void();
            Assert.IsTrue(_dut.IsVoided);

            _dut.UnVoid();
            Assert.IsFalse(_dut.IsVoided);
        }

        [TestMethod]
        public void SetMode_ShouldSetMode()
        {
            var modeId = 1;
            var mode = new Mode(_dut.Plant, "ModeTitle", false);
            mode.SetProtectedIdForTesting(modeId);
            _dut.SetMode(mode);

            Assert.AreEqual(modeId, _dut.ModeId);
        }

        [TestMethod]
        public void SetResponsible_ShouldSetResponsible()
        {
            var responsibleId = 1;
            var responsible = new Responsible(_dut.Plant, "C", "Title");
            responsible.SetProtectedIdForTesting(responsibleId);
            _dut.SetResponsible(responsible);

            Assert.AreEqual(responsibleId, _dut.ResponsibleId);
        }
    }
}

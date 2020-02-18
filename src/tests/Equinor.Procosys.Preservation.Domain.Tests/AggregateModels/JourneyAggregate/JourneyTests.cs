using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.JourneyAggregate
{
    [TestClass]
    public class JourneyTests
    {
        private Journey _dut;
        private int _firstStepId = 10033;
        private int _secondStepId = 3;
        private int _thirdStepId = 967;
        private Step _firstStep;
        private Step _secondStep;
        private Step _thirdStep;

        [TestInitialize]
        public void Setup()
        {
            _dut = new Journey("SchemaA", "TitleA");

            var firstStepMock = new Mock<Step>("S", new Mock<Mode>().Object, new Mock<Responsible>().Object, 10);
            firstStepMock.SetupGet(s => s.Id).Returns(_firstStepId);
            _firstStep = firstStepMock.Object;

            var secondStepMock = new Mock<Step>("S", new Mock<Mode>().Object, new Mock<Responsible>().Object, 20);
            secondStepMock.SetupGet(s => s.Id).Returns(_secondStepId);
            _secondStep = secondStepMock.Object;

            var thirdStepMock = new Mock<Step>("S", new Mock<Mode>().Object, new Mock<Responsible>().Object, 30);
            thirdStepMock.SetupGet(s => s.Id).Returns(_thirdStepId);
            _thirdStep = thirdStepMock.Object;
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual("SchemaA", _dut.Schema);
            Assert.AreEqual("TitleA", _dut.Title);
            Assert.IsFalse(_dut.IsVoided);
            Assert.AreEqual(0, _dut.Steps.Count);
            Assert.AreEqual(0, _dut.OrderedSteps().Count());
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
            var step = new Mock<Step>();

            _dut.AddStep(step.Object);

            Assert.AreEqual(1, _dut.Steps.Count);
            Assert.IsTrue(_dut.Steps.Contains(step.Object));
        }

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
        public void OrderedSteps_Should_ReturnCorrectOrder()
        {
            _dut.AddStep(_thirdStep);
            _dut.AddStep(_firstStep);
            _dut.AddStep(_secondStep);

            var steps = _dut.OrderedSteps().ToList();
            Assert.AreEqual(_firstStep, steps[0]);
            Assert.AreEqual(_secondStep, steps[1]);
            Assert.AreEqual(_thirdStep, steps[2]);
        }

        [TestMethod]
        public void GetNextStep_Should_ReturnCorrectStep()
        {
            _dut.AddStep(_thirdStep);
            _dut.AddStep(_firstStep);
            _dut.AddStep(_secondStep);

            var step = _dut.GetNextStep(_firstStepId);
            Assert.AreEqual(_secondStep, step);

            step = _dut.GetNextStep(_secondStepId);
            Assert.AreEqual(_thirdStep, step);

            step = _dut.GetNextStep(_thirdStepId);
            Assert.IsNull(step);
        }
    }
}

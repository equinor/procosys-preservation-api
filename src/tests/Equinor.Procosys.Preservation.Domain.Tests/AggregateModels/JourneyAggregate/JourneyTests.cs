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

        [TestInitialize]
        public void Setup() => _dut = new Journey("SchemaA", "TitleA");

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual("SchemaA", _dut.Schema);
            Assert.AreEqual("TitleA", _dut.Title);
            Assert.IsFalse(_dut.IsVoided);
            Assert.AreEqual(0, _dut.Steps.Count);
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
        public void GetNextStep_ShouldReturnStepWithNextSortKey_WhenDifferentSortKeys()
        {
            var firstStepId = 10033;
            var secondStepId = 3;
            var thirdStepId = 967;
            
            var firstStepMock = new Mock<Step>("S", new Mock<Mode>().Object, new Mock<Responsible>().Object);
            firstStepMock.SetupGet(s => s.Id).Returns(firstStepId);
            firstStepMock.Object.SortKey = 10;
            var firstStep = firstStepMock.Object;

            var secondStepMock = new Mock<Step>("S", new Mock<Mode>().Object, new Mock<Responsible>().Object);
            secondStepMock.SetupGet(s => s.Id).Returns(secondStepId);
            secondStepMock.Object.SortKey = 20;
            var secondStep = secondStepMock.Object;

            var thirdStepMock = new Mock<Step>("S", new Mock<Mode>().Object, new Mock<Responsible>().Object);
            thirdStepMock.SetupGet(s => s.Id).Returns(thirdStepId);
            thirdStepMock.Object.SortKey = 30;
            var thirdStep = thirdStepMock.Object;
            
            _dut.AddStep(thirdStep);
            _dut.AddStep(firstStep);
            _dut.AddStep(secondStep);

            var step = _dut.GetNextStep(firstStepId);
            Assert.AreEqual(secondStep, step);

            step = _dut.GetNextStep(secondStepId);
            Assert.AreEqual(thirdStep, step);

            step = _dut.GetNextStep(thirdStepId);
            Assert.IsNull(step);
        }

        [TestMethod]
        public void GetNextStep_ShouldReturnNextStepInList_WhenEqualSortKeys()
        {
            // this test exists to ensure reconsidering the case with Equal SortKeys when step sorting is implemented properly later
            var firstStepId = 1;
            var secondStepId = 2;
            var thirdStepId = 3;
            
            var firstStepMock = new Mock<Step>("S", new Mock<Mode>().Object, new Mock<Responsible>().Object);
            firstStepMock.SetupGet(s => s.Id).Returns(firstStepId);
            var firstStep = firstStepMock.Object;

            var secondStepMock = new Mock<Step>("S", new Mock<Mode>().Object, new Mock<Responsible>().Object);
            secondStepMock.SetupGet(s => s.Id).Returns(secondStepId);
            var secondStep = secondStepMock.Object;

            var thirdStepMock = new Mock<Step>("S", new Mock<Mode>().Object, new Mock<Responsible>().Object);
            thirdStepMock.SetupGet(s => s.Id).Returns(thirdStepId);
            var thirdStep = thirdStepMock.Object;
            
            _dut.AddStep(firstStep);
            _dut.AddStep(secondStep);
            _dut.AddStep(thirdStep);

            var step = _dut.GetNextStep(firstStepId);
            Assert.AreEqual(secondStep, step);

            step = _dut.GetNextStep(secondStepId);
            Assert.AreEqual(thirdStep, step);

            step = _dut.GetNextStep(thirdStepId);
            Assert.IsNull(step);
        }
    }
}

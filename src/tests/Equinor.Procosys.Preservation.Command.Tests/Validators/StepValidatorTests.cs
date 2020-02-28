using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.Step;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class StepValidatorTests
    {
        private const string TestPlant = "PlantA";
        private const int StepIdNonVoided = 1;
        private const int StepIdVoided = 2;
        private StepValidator _dut;

        [TestInitialize]
        public void Setup()
        {
            var modeMock = new Mock<Mode>();
            modeMock.SetupGet(x => x.Id).Returns(3);
            modeMock.SetupGet(x => x.Schema).Returns(TestPlant);

            var responsibleMock = new Mock<Responsible>();
            responsibleMock.SetupGet(x => x.Id).Returns(4);
            responsibleMock.SetupGet(x => x.Schema).Returns(TestPlant);
            var journeyRepositoryMock = new Mock<IJourneyRepository>();

            var step = new Step(TestPlant, modeMock.Object, responsibleMock.Object);
            var stepVoided = new Step(TestPlant, modeMock.Object, responsibleMock.Object);
            stepVoided.Void();

            journeyRepositoryMock.Setup(r => r.GetStepByStepIdAsync(StepIdNonVoided)).Returns(Task.FromResult(step));
            journeyRepositoryMock.Setup(r => r.GetStepByStepIdAsync(StepIdVoided)).Returns(Task.FromResult(stepVoided));

            _dut = new StepValidator(journeyRepositoryMock.Object);
        }

        [TestMethod]
        public void ValidateExists_KnownId_ReturnsTrue()
        {
            var result = _dut.Exists(StepIdNonVoided);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateExists_UnknownId_ReturnsFalse()
        {
            var result = _dut.Exists(126234);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateIsVoided_KnownVoided_ReturnsTrue()
        {
            var result = _dut.IsVoided(StepIdVoided);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateIsVoided_KnownNotVoided_ReturnsFalse()
        {
            var result = _dut.IsVoided(StepIdNonVoided);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateIsVoided_UnknownId_ReturnsFalse()
        {
            var result = _dut.IsVoided(126234);
            Assert.IsFalse(result);
        }
    }
}

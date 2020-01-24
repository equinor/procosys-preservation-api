using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.Responsible;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class ResponsibleValidatorTests
    {
        private const int ResponsibleIdNonVoided = 1;
        private const int ResponsibleIdVoided = 2;
        private ResponsibleValidator _dut;

        [TestInitialize]
        public void Setup()
        {
            var respRepoMock = new Mock<IResponsibleRepository>();

            var responsible = new Responsible("S", "R");
            var responsibleVoided = new Responsible("S", "R");
            responsibleVoided.Void();

            respRepoMock.Setup(r => r.GetByIdAsync(ResponsibleIdNonVoided)).Returns(Task.FromResult(responsible));
            respRepoMock.Setup(r => r.GetByIdAsync(ResponsibleIdVoided)).Returns(Task.FromResult(responsibleVoided));
            respRepoMock.Setup(r => r.Exists(ResponsibleIdNonVoided)).Returns(Task.FromResult(true));

            _dut = new ResponsibleValidator(respRepoMock.Object);
        }

        [TestMethod]
        public void ValidateExists_KnownId_ReturnsTrue()
        {
            var result = _dut.Exists(ResponsibleIdNonVoided);
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
            var result = _dut.IsVoided(ResponsibleIdVoided);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateIsVoided_KnownNotVoided_ReturnsFalse()
        {
            var result = _dut.IsVoided(ResponsibleIdNonVoided);
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

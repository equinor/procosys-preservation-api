using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.Field;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class FieldValidatorTests
    {
        private const int FieldIdNonVoided = 1;
        private const int FieldIdVoided = 2;
        private FieldValidator _dut;

        [TestInitialize]
        public void Setup()
        {
            var rtRepoMock = new Mock<IRequirementTypeRepository>();

            var field = new Field("", "", FieldType.Info, 0);
            var fieldVoided = new Field("", "", FieldType.Info, 0);
            fieldVoided.Void();

            rtRepoMock.Setup(r => r.GetFieldByIdAsync(FieldIdNonVoided)).Returns(Task.FromResult(field));
            rtRepoMock.Setup(r => r.GetFieldByIdAsync(FieldIdVoided)).Returns(Task.FromResult(fieldVoided));

            _dut = new FieldValidator(rtRepoMock.Object);
        }

        [TestMethod]
        public void ValidateExists_KnownId_ReturnsTrue()
        {
            var result = _dut.Exists(FieldIdNonVoided);
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
            var result = _dut.IsVoided(FieldIdVoided);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateIsVoided_KnownNotVoided_ReturnsFalse()
        {
            var result = _dut.IsVoided(FieldIdNonVoided);
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

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
        private const int InfoFieldIdNonVoided = 1;
        private const int InfoFieldIdVoided = 2;
        private FieldValidator _dut;

        [TestInitialize]
        public void Setup()
        {
            var rtRepoMock = new Mock<IRequirementTypeRepository>();

            var nonVoidedInfoField = new Field("", "", FieldType.Info, 0);
            var voidedInfoField = new Field("", "", FieldType.Info, 0);
            voidedInfoField.Void();

            rtRepoMock.Setup(r => r.GetFieldByIdAsync(InfoFieldIdNonVoided)).Returns(Task.FromResult(nonVoidedInfoField));
            rtRepoMock.Setup(r => r.GetFieldByIdAsync(InfoFieldIdVoided)).Returns(Task.FromResult(voidedInfoField));

            _dut = new FieldValidator(rtRepoMock.Object);
        }

        [TestMethod]
        public void ValidateExists_KnownId_ReturnsTrue()
        {
            var result = _dut.Exists(InfoFieldIdNonVoided);
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
            var result = _dut.IsVoided(InfoFieldIdVoided);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateIsVoided_KnownNotVoided_ReturnsFalse()
        {
            var result = _dut.IsVoided(InfoFieldIdNonVoided);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateIsVoided_UnknownId_ReturnsFalse()
        {
            var result = _dut.IsVoided(126234);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateVerifyFieldType_NotNumber_ReturnsFalse()
        {
            var result = _dut.VerifyFieldType(InfoFieldIdNonVoided, FieldType.Number);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateVerifyFieldType_Info_ReturnsTrue()
        {
            var result = _dut.VerifyFieldType(InfoFieldIdNonVoided, FieldType.Info);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateVerifyFieldType_UnknownId_ReturnsFalse()
        {
            var result = _dut.VerifyFieldType(126234, FieldType.Info);
            Assert.IsFalse(result);
        }
    }
}

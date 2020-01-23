using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinition;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class RequirementDefinitionValidatorTests
    {
        private const int ReqDefIdNonVoided = 1;
        private const int ReqDefIdVoided = 2;
        private RequirementDefinitionValidator _dut;

        [TestInitialize]
        public void Setup()
        {
            var reqTypeRepoMock = new Mock<IRequirementTypeRepository>();

            var reqDef = new RequirementDefinition("S", "T", 1, 0);
            var reqDefVoided = new RequirementDefinition("S", "T", 1, 0);
            reqDefVoided.Void();

            reqTypeRepoMock.Setup(r => r.GetRequirementDefinitionByIdAsync(ReqDefIdNonVoided)).Returns(Task.FromResult(reqDef));
            reqTypeRepoMock.Setup(r => r.GetRequirementDefinitionByIdAsync(ReqDefIdVoided)).Returns(Task.FromResult(reqDefVoided));

            _dut = new RequirementDefinitionValidator(reqTypeRepoMock.Object);
        }

        [TestMethod]
        public void ValidateExists_KnownId_ReturnsTrue()
        {
            var result = _dut.Exists(ReqDefIdNonVoided);
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
            var result = _dut.IsVoided(ReqDefIdVoided);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateIsVoided_KnownNotVoided_ReturnsFalse()
        {
            var result = _dut.IsVoided(ReqDefIdNonVoided);
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

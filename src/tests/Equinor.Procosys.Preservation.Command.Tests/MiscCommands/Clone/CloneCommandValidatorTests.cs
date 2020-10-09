using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.MiscCommands.Clone;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.MiscCommands.Clone
{
    [TestClass]
    public class CloneCommandValidatorTests
    {
        private CloneCommand _command;
        private CloneCommandValidator _dut;
        private readonly string _sourcePlant = "SOURCE";
        private readonly string _targetPlant = "TARGET";
        private Mock<IPlantCache> _plantCacheMock;

        [TestInitialize]
        public void Setup_OkState()
        {
            _plantCacheMock = new Mock<IPlantCache>();
            _plantCacheMock.Setup(r => r.HasCurrentUserAccessToPlantAsync(_sourcePlant)).Returns(Task.FromResult(true));
            _plantCacheMock.Setup(r => r.HasCurrentUserAccessToPlantAsync(_targetPlant)).Returns(Task.FromResult(true));

            _command = new CloneCommand(_sourcePlant, _targetPlant);
            _dut = new CloneCommandValidator(_plantCacheMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTargetPlantNotValid()
        {
            _plantCacheMock.Setup(r => r.HasCurrentUserAccessToPlantAsync(_targetPlant)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Target plant is not valid or access missing!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenSourcePlantNotValid()
        {
            _plantCacheMock.Setup(r => r.HasCurrentUserAccessToPlantAsync(_sourcePlant)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Source plant is not valid or access missing!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTargetPlantIsABasisPlant()
        {
            var targetPlant = "PCS$STATOIL_BASIS";
            _plantCacheMock.Setup(r => r.HasCurrentUserAccessToPlantAsync(targetPlant)).Returns(Task.FromResult(true));
            var command = new CloneCommand(_sourcePlant, targetPlant);
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Target plant can not be a basis plant!"));
        }
    }
}

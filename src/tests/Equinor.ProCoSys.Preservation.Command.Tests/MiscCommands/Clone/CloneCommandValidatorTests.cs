using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Caches;
using Equinor.ProCoSys.Preservation.Command.MiscCommands.Clone;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.MiscCommands.Clone
{
    [TestClass]
    public class CloneCommandValidatorTests
    {
        private CloneCommand _command;
        private CloneCommandValidator _dut;
        private readonly string _sourcePlant = "SOURCE";
        private readonly string _targetPlant = "TARGET";
        private Mock<IPermissionCache> _permissionCacheMock;

        [TestInitialize]
        public void Setup_OkState()
        {
            _permissionCacheMock = new Mock<IPermissionCache>();
            _permissionCacheMock.Setup(r => r.HasCurrentUserAccessToPlantAsync(_sourcePlant)).Returns(Task.FromResult(true));
            _permissionCacheMock.Setup(r => r.HasCurrentUserAccessToPlantAsync(_targetPlant)).Returns(Task.FromResult(true));

            _command = new CloneCommand(_sourcePlant, _targetPlant);
            _dut = new CloneCommandValidator(_permissionCacheMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenOkState()
        {
            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenTargetPlantNotValid()
        {
            _permissionCacheMock.Setup(r => r.HasCurrentUserAccessToPlantAsync(_targetPlant)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Target plant is not valid or access missing!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenSourcePlantNotValid()
        {
            _permissionCacheMock.Setup(r => r.HasCurrentUserAccessToPlantAsync(_sourcePlant)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Source plant is not valid or access missing!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenTargetPlantIsABasisPlant()
        {
            var targetPlant = "PCS$STATOIL_BASIS";
            _permissionCacheMock.Setup(r => r.HasCurrentUserAccessToPlantAsync(targetPlant)).Returns(Task.FromResult(true));
            var command = new CloneCommand(_sourcePlant, targetPlant);
            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Target plant can not be a basis plant!"));
        }
    }
}

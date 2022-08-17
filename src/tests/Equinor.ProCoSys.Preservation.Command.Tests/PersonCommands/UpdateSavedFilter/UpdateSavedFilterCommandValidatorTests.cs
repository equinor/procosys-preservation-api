using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.PersonCommands.UpdateSavedFilter;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.SavedFilterValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.PersonCommands.UpdateSavedFilter
{
    [TestClass]
    public class UpdateSavedFilterCommandValidatorTests
    {
        private UpdateSavedFilterCommand _command;
        private UpdateSavedFilterCommandValidator _dut;

        private Mock<ISavedFilterValidator> _savedFilterValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;

        private int _savedFilterId = 2;
        private string _title = "Title";
        private string _criteria = "Criteria";

        [TestInitialize]
        public void Setup_OkState()
        {
        var RowVersion = "AAAAAAAAJ00=";

        _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(RowVersion)).Returns(true);

            _savedFilterValidatorMock = new Mock<ISavedFilterValidator>();
            _savedFilterValidatorMock.Setup(r => r.ExistsAsync(_savedFilterId, default)).Returns(Task.FromResult(true));

            _command = new UpdateSavedFilterCommand(_savedFilterId, _title, _criteria, false,  RowVersion);

            _dut = new UpdateSavedFilterCommandValidator(
                _savedFilterValidatorMock.Object,
                _rowVersionValidatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenOkState()
        {
            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenSavedFilterNotExists()
        {
            _savedFilterValidatorMock.Setup(r => r.ExistsAsync(_savedFilterId, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Saved filter doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenSavedFilterExistsWithSameTitleForPersonInProject()
        {
            // Arrange
            _savedFilterValidatorMock.Setup(r => r.ExistsAnotherWithSameTitleForPersonInProjectAsync(_savedFilterId, _title, default))
                .Returns(Task.FromResult(true));

            // Act
            var result = await _dut.ValidateAsync(_command);

            // Arrange
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("A saved filter with this title already exists!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";

            _command = new UpdateSavedFilterCommand(_savedFilterId, _title, _criteria,
                false, invalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion)).Returns(false);

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }
    }
}

using Equinor.ProCoSys.Preservation.Command.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class RowVersionValidatorTests
    {
        private RowVersionValidator _dut;

        [TestInitialize]
        public void SetUp() => _dut = new RowVersionValidator();

        [TestMethod]
        public void IsValid_ValidRowVersion_ShouldReturnTrue()
        {
            const string ValidRowVersion = "AAAAAAAAABA=";

            var result = _dut.IsValid(ValidRowVersion);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValid_InvalidRowVersion_ShouldReturnFalse()
        {
            const string InvalidRowVersion = "String";

            var result = _dut.IsValid(InvalidRowVersion);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValid_EmptyString_ShouldReturnFalse()
        {
            var result = _dut.IsValid(string.Empty);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValid_Null_ShouldReturnFalse()
        {
            var result = _dut.IsValid(null);
            Assert.IsFalse(result);
        }
    }
}

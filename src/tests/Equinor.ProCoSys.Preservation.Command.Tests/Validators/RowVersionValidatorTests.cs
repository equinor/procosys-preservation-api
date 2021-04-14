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
            const string validRowVersion = "AAAAAAAAABA=";

            var result = _dut.IsValid(validRowVersion);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValid_InvalidRowVersion_ShouldReturnFalse()
        {
            const string invalidRowVersion = "String";

            var result = _dut.IsValid(invalidRowVersion);
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

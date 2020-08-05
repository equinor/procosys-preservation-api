using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class RowVersionValidatorTests
    {
        private RowVersionValidator _dut;

        [TestInitialize]
        public void SetUp() => _dut = new RowVersionValidator();

        [TestMethod]
        public async Task IsValid_ValidRowVersion_ReturnsTrue()
        { 
            const string validRowVersion = "AAAAAAAAABA=";

            var result = await _dut.IsValid(validRowVersion, default);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task IsValid_InvalidRowVersion_ReturnsFalse()
        {
            const string invalidRowVersion = "String";

            var result = await _dut.IsValid(invalidRowVersion, default);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsValid_EmptyString_ReturnsFalse()
        {
            var result = await _dut.IsValid(string.Empty, default);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task IsValid_Null_ReturnsFalse()
        {
            var result = await _dut.IsValid(null, default);
            Assert.IsFalse(result);
        }
    }
}

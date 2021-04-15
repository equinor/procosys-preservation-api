using Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Controllers.Tags
{
    [TestClass]
    public class DuplicateAreaTagDtoValidatorTests
    {
        [TestMethod]
        public void Validate_OK()
        {
            var dut = new DuplicateAreaTagDtoValidator();
            var validDuplicateAreaTagDto = new DuplicateAreaTagDto
            {
                DisciplineCode = "I",
                TagNoSuffix = "10"
            };

            var result = dut.Validate(validDuplicateAreaTagDto);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Fail_WhenWhiteSpaceInSuffix()
        {
            var dut = new DuplicateAreaTagDtoValidator();

            var inValidDuplicateAreaTagDto = new DuplicateAreaTagDto
            {
                DisciplineCode = "I",
                TagNoSuffix = "10 A"
            };

            var result = dut.Validate(inValidDuplicateAreaTagDto);

            Assert.IsFalse(result.IsValid);
        }
    }
}

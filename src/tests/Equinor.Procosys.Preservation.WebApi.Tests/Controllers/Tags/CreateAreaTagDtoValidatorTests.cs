using System.Collections.Generic;
using Equinor.Procosys.Preservation.WebApi.Controllers.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Controllers.Tags
{
    [TestClass]
    public class CreateAreaTagDtoValidatorTests
    {
        [TestMethod]
        public void Validate_OK()
        {
            var dut = new CreateAreaTagDtoValidator();
            var validCreateAreaTagDto = new CreateAreaTagDto
            {
                ProjectName = "P",
                DisciplineCode = "I",
                Requirements = new List<TagRequirementDto> {new TagRequirementDto {IntervalWeeks = 2}},
                TagNoSuffix = "10"
            };

            var result = dut.Validate(validCreateAreaTagDto);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Fail_WhenWhiteSpaceInSuffix()
        {
            var dut = new CreateAreaTagDtoValidator();

            var inValidCreateAreaTagDto = new CreateAreaTagDto
            {
                ProjectName = "P",
                DisciplineCode = "I",
                Requirements = new List<TagRequirementDto> {new TagRequirementDto {IntervalWeeks = 2}},
                TagNoSuffix = "10 A"
            };

            var result = dut.Validate(inValidCreateAreaTagDto);

            Assert.IsFalse(result.IsValid);
        }
    }
}

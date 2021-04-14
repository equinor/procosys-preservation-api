using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.WebApi.Controllers.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Controllers.Tags
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

        [TestMethod]
        public void Fail_WhenPoTag_AndPoNotGiven()
        {
            var dut = new CreateAreaTagDtoValidator();

            var inValidCreateAreaTagDto = new CreateAreaTagDto
            {
                AreaTagType = AreaTagType.PoArea,
                ProjectName = "P",
                DisciplineCode = "I",
                Requirements = new List<TagRequirementDto> {new TagRequirementDto {IntervalWeeks = 2}}
            };

            var result = dut.Validate(inValidCreateAreaTagDto);

            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_OK_WhenPoTag_AndPoGiven()
        {
            var dut = new CreateAreaTagDtoValidator();

            var inValidCreateAreaTagDto = new CreateAreaTagDto
            {
                AreaTagType = AreaTagType.PoArea,
                ProjectName = "P",
                DisciplineCode = "I",
                Requirements = new List<TagRequirementDto> {new TagRequirementDto {IntervalWeeks = 2}},
                PurchaseOrderCalloffCode = "X"
            };

            var result = dut.Validate(inValidCreateAreaTagDto);

            Assert.IsTrue(result.IsValid);
        }
    }
}

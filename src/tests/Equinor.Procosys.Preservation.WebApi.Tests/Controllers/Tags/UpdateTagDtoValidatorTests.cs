using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.WebApi.Controllers.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Controllers.Tags
{
    [TestClass]
    public class UpdateTagDtoValidatorTests
    {
        [TestMethod]
        public void Validate_OK()
        {
            var dut = new UpdateTagDtoValidator();
            var validUpdateTagDto = new UpdateTagDto();

            var result = dut.Validate(validUpdateTagDto);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Fail_WhenRemarkIsTooLong()
        {
            var dut = new UpdateTagDtoValidator();
            var inValidUpdateTagDto = new UpdateTagDto()
            {
                Remark = new string('x', Tag.RemarkLengthMax + 1),
                StorageArea = "StorageArea"
            };

            var result = dut.Validate(inValidUpdateTagDto);

            Assert.IsFalse(result.IsValid);
        }


        [TestMethod]
        public void Fail_WhenStorageAreaIsTooLong()
        {
            var dut = new UpdateTagDtoValidator();
            var inValidUpdateTagDto = new UpdateTagDto()
            {
                Remark = "Remark",
                StorageArea = new string('x', Tag.StorageAreaLengthMax + 1)
            };

            var result = dut.Validate(inValidUpdateTagDto);

            Assert.IsFalse(result.IsValid);
        }
    }
}

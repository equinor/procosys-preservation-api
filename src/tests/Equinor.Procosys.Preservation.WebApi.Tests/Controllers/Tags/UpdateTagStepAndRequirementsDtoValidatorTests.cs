using System.Collections.Generic;
using Equinor.Procosys.Preservation.WebApi.Controllers.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Controllers.Tags
{
    [TestClass]
    public class UpdateTagStepAndRequirementsDtoValidatorTests
    {
        [TestMethod]
        public void Validate_OK()
        {
            var dut = new UpdateTagStepAndRequirementsDtoValidator();
            var validUpdateTagDto = new UpdateTagStepAndRequirementsDto();

            var result = dut.Validate(validUpdateTagDto);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_OK_WhenRequirementListsExplicitNull()
        {
            var dut = new UpdateTagStepAndRequirementsDtoValidator();
            var inValidUpdateTagDto = new UpdateTagStepAndRequirementsDto
            {
                NewRequirements = null,
                UpdatedRequirements = null
            };

            var result = dut.Validate(inValidUpdateTagDto);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Fail_WhenIntervalWeeksIsZeroForNewRequirement()
        {
            var dut = new UpdateTagStepAndRequirementsDtoValidator();
            var inValidUpdateTagDto = new UpdateTagStepAndRequirementsDto
            {
                NewRequirements = new List<TagRequirementDto>
                {
                    new TagRequirementDto
                    {
                        IntervalWeeks = 0,
                        RequirementDefinitionId = 2
                    }
                }
            };

            var result = dut.Validate(inValidUpdateTagDto);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Week interval must be positive!"));
        }

        [TestMethod]
        public void Fail_WhenIntervalWeeksIsZeroForUpdatedRequirement()
        {
            var dut = new UpdateTagStepAndRequirementsDtoValidator();
            var inValidUpdateTagDto = new UpdateTagStepAndRequirementsDto
            {
                UpdatedRequirements = new List<UpdatedTagRequirementDto>
                {
                    new UpdatedTagRequirementDto
                    {
                        IntervalWeeks = 0,
                        RequirementId = 2
                    }
                }
            };

            var result = dut.Validate(inValidUpdateTagDto);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Week interval must be positive!"));
        }
    }
}

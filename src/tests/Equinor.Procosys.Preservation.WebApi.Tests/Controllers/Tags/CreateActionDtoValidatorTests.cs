using System;
using Equinor.Procosys.Preservation.WebApi.Controllers.Tags;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Controllers
{
    [TestClass]
    public class CreateActionDtoValidatorTests
    {
        [TestMethod]
        public void Validate_OK()
        {
            var dut = new CreateActionDtoValidator();
            var validCreateActionDto = new CreateActionDto
            {
                TagId = 2,
                Title = "ActionTitle",
                Description = "ActionDescription",
                DueTimeUtc = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            };

            var result = dut.Validate(validCreateActionDto);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Fail_WhenTitleIsTooLong()
        {
            var dut = new CreateActionDtoValidator();

            var inValidCreateActionDto = new CreateActionDto
            {
                TagId = 2,
                Title = new string('x', Action.TitleLengthMax + 1),
                Description = "ActionDescription",
                DueTimeUtc = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            };

            var result = dut.Validate(inValidCreateActionDto);

            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Fail_WhenDescriptionIsTooLong()
        {
            var dut = new CreateActionDtoValidator();

            var inValidCreateActionDto = new CreateActionDto
            {
                TagId = 2,
                Title = "ActionTitle",
                Description = new string('x', Action.DescriptionLengthMax + 1),
                DueTimeUtc = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            };

            var result = dut.Validate(inValidCreateActionDto);

            Assert.IsFalse(result.IsValid);
        }
    }
}

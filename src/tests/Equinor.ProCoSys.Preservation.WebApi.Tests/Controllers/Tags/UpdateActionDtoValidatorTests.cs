﻿using System;
using Equinor.ProCoSys.Preservation.WebApi.Tags.Controllers.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.WebApi.Tests.Controllers.Tags
{
    [TestClass]
    public class UpdateActionDtoValidatorTests
    {
        [TestMethod]
        public void Validate_OK()
        {
            var dut = new UpdateActionDtoValidator();
            var validUpdateActionDto = new UpdateActionDto
            {
                Title = "UpdatedActionTitle",
                Description = "UpdatedActionDescription",
                DueTimeUtc = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            };

            var result = dut.Validate(validUpdateActionDto);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Fail_WhenTitleIsTooLong()
        {
            var dut = new UpdateActionDtoValidator();

            var inValidUpdateActionDto = new UpdateActionDto
            {
                Title = new string('x', Action.TitleLengthMax + 1),
                Description = "UpdatedActionDescription",
                DueTimeUtc = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            };

            var result = dut.Validate(inValidUpdateActionDto);

            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Fail_WhenDescriptionIsTooLong()
        {
            var dut = new UpdateActionDtoValidator();

            var inValidUpdateActionDto = new UpdateActionDto
            {
                Title = "NewActionTitle",
                Description = new string('x', Action.DescriptionLengthMax + 1),
                DueTimeUtc = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc)
            };

            var result = dut.Validate(inValidUpdateActionDto);

            Assert.IsFalse(result.IsValid);
        }
    }
}

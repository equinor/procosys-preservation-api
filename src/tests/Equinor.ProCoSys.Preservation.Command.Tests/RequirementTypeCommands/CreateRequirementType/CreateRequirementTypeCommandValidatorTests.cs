﻿using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.CreateRequirementType;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementTypeValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.RequirementTypeCommands.CreateRequirementType
{
    [TestClass]
    public class CreateRequirementTypeCommandValidatorTests
    {
        private CreateRequirementTypeCommandValidator _dut;
        private Mock<IRequirementTypeValidator> _requirementTypeValidatorMock;
        private CreateRequirementTypeCommand _command;

        private int _sortKey = 10;
        private string _title = "Title";
        private string _code = "Code";

        [TestInitialize]
        public void Setup_OkState()
        {
            _requirementTypeValidatorMock = new Mock<IRequirementTypeValidator>();
            _command = new CreateRequirementTypeCommand(_sortKey, _code, _title, RequirementTypeIcon.Other);
            _dut = new CreateRequirementTypeCommandValidator(_requirementTypeValidatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenOkState()
        {
            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementTypeWithSameTitleAlreadyExists()
        {
            _requirementTypeValidatorMock.Setup(r => r.ExistsWithSameTitleAsync(_title, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type with this title already exists!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementTypeWithSameCodeAlreadyExists()
        {
            _requirementTypeValidatorMock.Setup(r => r.ExistsWithSameCodeAsync(_code, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement type with this code already exists!"));
        }
    }
}

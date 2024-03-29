﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagFunctionCommands.UpdateRequirements;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagFunctionCommands.UpdateRequirements
{
    [TestClass]
    public class UpdateRequirementsCommandValidatorTests
    {
        private UpdateRequirementsCommandValidator _dut;
        private Mock<IRequirementDefinitionValidator> _rdValidatorMock;
        private UpdateRequirementsCommand _command;
        private int _rd1Id = 2;
        private int _rd2Id = 3;

        [TestInitialize]
        public void Setup_OkState()
        {
            _rdValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rd1Id, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rd2Id, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(new List<int>{_rd1Id, _rd2Id}, default)).Returns(Task.FromResult(true));

            _command = new UpdateRequirementsCommand("", "",
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rd1Id, 1),
                    new RequirementForCommand(_rd2Id, 1)
                });

            _dut = new UpdateRequirementsCommandValidator(_rdValidatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenOkState()
        {
            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WithoutRequirements()
        {
            var command = new UpdateRequirementsCommand("", "", new List<RequirementForCommand>());
            var result = await _dut.ValidateAsync(command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenAnyRequirementDefinitionNotExists()
        {
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rd2Id, default)).Returns(Task.FromResult(false));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition doesn't exist!"));
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains(_rd2Id.ToString()));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementUsageForSupplierNotGiven()
        {
            _rdValidatorMock.Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(new List<int>{_rd1Id, _rd2Id}, default)).Returns(Task.FromResult(false));
            _rdValidatorMock.Setup(r => r.UsageCoversForOtherThanSuppliersAsync(new List<int>{_rd1Id, _rd2Id}, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must include requirements to be used both for supplier and other than suppliers!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenAnyRequirementDefinitionIsVoided()
        {
            _rdValidatorMock.Setup(r => r.IsVoidedAsync(_rd2Id, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition is voided!"));
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains(_rd2Id.ToString()));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementsNotUnique()
        {
            var command = new UpdateRequirementsCommand("", "",
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rd1Id, 1),
                    new RequirementForCommand(_rd1Id, 1)
                });
            
            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definitions must be unique!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFailWith1Error_When2ErrorsWithinSameRule()
        {
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rd2Id, default)).Returns(Task.FromResult(false));
            _rdValidatorMock.Setup(r => r.IsVoidedAsync(_rd2Id, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }
    }
}

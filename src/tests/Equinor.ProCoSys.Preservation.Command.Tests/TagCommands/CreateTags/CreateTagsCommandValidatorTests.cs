﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.CreateTags;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.StepValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.CreateTags
{
    [TestClass]
    public class CreateTagsCommandValidatorTests
    {
        private CreateTagsCommandValidator _dut;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IStepValidator> _stepValidatorMock;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<IRequirementDefinitionValidator> _rdValidatorMock;
        private CreateTagsCommand _command;

        private string _tagNo1 = "Tag1";
        private string _tagNo2 = "Tag2";
        private string _projectName = "Project";
        private int _stepId = 1;
        private int _rd1Id = 2;
        private int _rd2Id = 3;
        private List<RequirementForCommand> _requirements;

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagValidatorMock = new Mock<ITagValidator>();

            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.ExistsAsync(_stepId, default)).Returns(Task.FromResult(true));
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(true));

            _projectValidatorMock = new Mock<IProjectValidator>();

            _rdValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rd1Id, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rd2Id, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(new List<int>{_rd1Id, _rd2Id}, default)).Returns(Task.FromResult(true));

            _requirements = new List<RequirementForCommand>
            {
                new RequirementForCommand(_rd1Id, 1),
                new RequirementForCommand(_rd2Id, 1)
            };
            _command = new CreateTagsCommand(
                new List<string>{_tagNo1, _tagNo2}, 
                _projectName,
                _stepId,
                _requirements,
                null,
                null);

            _dut = new CreateTagsCommandValidator(
                _tagValidatorMock.Object, 
                _stepValidatorMock.Object, 
                _projectValidatorMock.Object,
                _rdValidatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenOkState()
        {
            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenAnyTagAlreadyExists()
        {
            _tagValidatorMock.Setup(r => r.ExistsAsync(_tagNo2, _projectName, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag already exists in scope for project!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenProjectExistsButClosed()
        {
            _projectValidatorMock.Setup(r => r.IsExistingAndClosedAsync(_projectName, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project is closed!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenStepNotExists()
        {
            _stepValidatorMock.Setup(r => r.ExistsAsync(_stepId, default)).Returns(Task.FromResult(false));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenStepIsVoided()
        {
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(_stepId, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step is voided!"));
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
        public async Task Validate_ShouldFail_WhenNoTagNosGiven()
        {
            var command = new CreateTagsCommand(
                new List<string>(), 
                _projectName,
                _stepId,
                _requirements,
                null,
                null);
            
            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("At least 1 TagNo must be given!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenNoTagNosNotUnique()
        {
            var command = new CreateTagsCommand(
                new List<string>{"X","x"}, 
                _projectName,
                _stepId,
                _requirements,
                null,
                null);
            
            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tags must be unique!"));
        }
        
        [TestMethod]
        public async Task Validate_ShouldFailForNonSupplierStep_WhenNoRequirementsGiven()
        {
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));
            var command = new CreateTagsCommand(
                new List<string>{_tagNo1, _tagNo2}, 
                _projectName,
                _stepId,
                new List<RequirementForCommand>(),
                null,
                null);
            
            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must include requirements to be used for other than suppliers!"));
        }

        [TestMethod]
        public async Task Validate_ShouldBeValidForNonSupplierStep_WhenRequirementsForOtherGiven()
        {
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));
            _rdValidatorMock.Setup(r => r.UsageCoversForOtherThanSuppliersAsync(new List<int>{_rd1Id, _rd2Id}, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }
        
        [TestMethod]
        public async Task Validate_ShouldFailForNonSupplierStep_WhenRequirementForSupplierOnlyGiven()
        {
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));
            _rdValidatorMock.Setup(r => r.UsageCoversForOtherThanSuppliersAsync(new List<int>{_rd1Id, _rd2Id}, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.HasAnyForSupplierOnlyUsageAsync(new List<int>{_rd1Id, _rd2Id}, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements can not include requirements just for suppliers!"));
        }
        
        [TestMethod]
        public async Task Validate_ShouldFailForNonSupplierStep_WhenRequirementForOtherNotGiven()
        {
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));
            _rdValidatorMock.Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(new List<int>{_rd1Id, _rd2Id}, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must include requirements to be used for other than suppliers!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFailForSupplierStep_WhenNoRequirementsGiven()
        {
            var command = new CreateTagsCommand(
                new List<string>{_tagNo1, _tagNo2}, 
                _projectName,
                _stepId,
                new List<RequirementForCommand>(),
                null,
                null);
            
            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must include requirements to be used both for supplier and other than suppliers!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFailForSupplierStep_WhenRequirementForSupplierNotGiven()
        {
            _rdValidatorMock.Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(new List<int>{_rd1Id, _rd2Id}, default)).Returns(Task.FromResult(false));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must include requirements to be used both for supplier and other than suppliers!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementsNotUnique()
        {
            _rdValidatorMock.Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(new List<int>{_rd1Id, _rd1Id}, default)).Returns(Task.FromResult(true));
            var command = new CreateTagsCommand(
                new List<string>{_tagNo1}, 
                _projectName,
                _stepId,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rd1Id, 1),
                    new RequirementForCommand(_rd1Id, 1)
                },
                null,
                null);
            
            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definitions must be unique!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFailWith1Error_When2ErrorsWithinSameRule()
        {
            _stepValidatorMock.Setup(r => r.ExistsAsync(_stepId, default)).Returns(Task.FromResult(false));
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(_stepId, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [TestMethod]
        public async Task Validate_ShouldFailWith1Error_WhenErrorsInDifferentRules()
        {
            _tagValidatorMock.Setup(r => r.ExistsAsync(_tagNo1, _projectName, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rd2Id, default)).Returns(Task.FromResult(false));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }
    }
}

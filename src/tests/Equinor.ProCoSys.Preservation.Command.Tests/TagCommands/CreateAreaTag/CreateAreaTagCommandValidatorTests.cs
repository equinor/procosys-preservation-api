using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.CreateAreaTag;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.StepValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.CreateAreaTag
{
    [TestClass]
    public class CreateAreaTagCommandValidatorTests
    {
        private CreateAreaTagCommandValidator _dut;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IStepValidator> _stepValidatorMock;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<IRequirementDefinitionValidator> _rdValidatorMock;
        private CreateAreaTagCommand _createPreAreaTagCommand;
        private CreateAreaTagCommand _createPoAreaTagCommand;

        private string _projectName = "Project";
        private int _stepId = 1;
        private int _rdId = 2;

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagValidatorMock = new Mock<ITagValidator>();

            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.ExistsAsync(_stepId, default)).Returns(Task.FromResult(true));
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(true));

            _projectValidatorMock = new Mock<IProjectValidator>();

            _rdValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rdId, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(new List<int>{_rdId}, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.UsageCoversForOtherThanSuppliersAsync(new List<int>{_rdId}, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.UsageCoversForSuppliersAsync(new List<int>{_rdId}, default)).Returns(Task.FromResult(true));

            _createPreAreaTagCommand = new CreateAreaTagCommand(
                _projectName,
                TagType.PreArea,
                "D",
                "A",
                null,
                null,
                _stepId,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdId, 1)
                },
                "Desc",
                "Remark",
                "SA");

            _createPoAreaTagCommand = new CreateAreaTagCommand(
                _projectName,
                TagType.PoArea,
                "D",
                "A",
                null,
                null,
                _stepId,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdId, 1)
                },
                "Desc",
                "Remark",
                "SA");

            _dut = new CreateAreaTagCommandValidator(
                _tagValidatorMock.Object, 
                _stepValidatorMock.Object, 
                _projectValidatorMock.Object,
                _rdValidatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_ForPreArea_InSupplierStep()
        {
            var result = await _dut.ValidateAsync(_createPreAreaTagCommand);

            Assert.IsTrue(result.IsValid);
        }
        
        [TestMethod]
        public async Task Validate_ShouldBeValid_ForPreArea_InNonSupplierStep()
        {
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_createPreAreaTagCommand);

            Assert.IsTrue(result.IsValid);
        }
        
        [TestMethod]
        public async Task Validate_ShouldFail_ForPreArea_InSupplierStep_WhenRequirementUsageIsNotForAllJourneys()
        {
            _rdValidatorMock.Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(new List<int>{_rdId}, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must include requirements to be used both for supplier and other than suppliers!"));
        }
        
        [TestMethod]
        public async Task Validate_ShouldFail_ForPreArea_InNonSupplierStep_WhenRequirementUsageIsNotForJourneysWithoutSupplier()
        {
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));
            _rdValidatorMock.Setup(r => r.UsageCoversForOtherThanSuppliersAsync(new List<int>{_rdId}, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must include requirements to be used for other than suppliers!"));
        }
        
        [TestMethod]
        public async Task Validate_ShouldFail_ForPreArea_InNonSupplierStep_WhenAnyRequirementForSupplierOnlyGiven()
        {
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));
            _rdValidatorMock.Setup(r => r.HasAnyForSupplierOnlyUsageAsync(new List<int>{_rdId}, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements can not include requirements just for suppliers!"));
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_ForPoArea_InSupplierStep()
        {
            var result = await _dut.ValidateAsync(_createPoAreaTagCommand);

            Assert.IsTrue(result.IsValid);
        }
        
        [TestMethod]
        public async Task Validate_ShouldFail_ForPoArea_InSupplierStep_WhenRequirementUsageIsNotForSupplier()
        {
            _rdValidatorMock.Setup(r => r.UsageCoversForSuppliersAsync(new List<int>{_rdId}, default)).Returns(Task.FromResult(false));

            var result = await _dut.ValidateAsync(_createPoAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must include requirements to be used for supplier!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_ForPoArea_InSupplierStep_WhenRequirementUsageIsForOtherThanSupplier()
        {
            _rdValidatorMock.Setup(r => r.HasAnyForForOtherThanSuppliersUsageAsync(new List<int>{_rdId}, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_createPoAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements can not include requirements for other than suppliers!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_ForPoArea_InNonSupplierStep()
        {
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));
            
            var result = await _dut.ValidateAsync(_createPoAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Step for a {TagType.PoArea.GetTagNoPrefix()} tag needs to be for supplier!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenAnyTagAlreadyExists()
        {
            _tagValidatorMock.Setup(r => r.ExistsAsync(_createPreAreaTagCommand.GetTagNo(), _projectName, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag already exists in scope for project!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenProjectExistsButClosed()
        {
            _projectValidatorMock.Setup(r => r.IsExistingAndClosedAsync(_projectName, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project is closed!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenStepNotExists()
        {
            _stepValidatorMock.Setup(r => r.ExistsAsync(_stepId, default)).Returns(Task.FromResult(false));
            
            var result = await _dut.ValidateAsync(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenStepIsVoided()
        {
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(_stepId, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step is voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenAnyRequirementDefinitionNotExists()
        {
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rdId, default)).Returns(Task.FromResult(false));
            
            var result = await _dut.ValidateAsync(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenAnyRequirementDefinitionIsVoided()
        {
            _rdValidatorMock.Setup(r => r.IsVoidedAsync(_rdId, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition is voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementsNotUnique()
        {
            _rdValidatorMock
                .Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(
                    new List<int> {_rdId, _rdId}, default))
                .Returns(Task.FromResult(true));
            
            var command = new CreateAreaTagCommand(
                _projectName,
                TagType.PreArea,
                "DisciplineA",
                "AreaA",
                null,
                null,
                _stepId,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdId, 1),
                    new RequirementForCommand(_rdId, 2)
                },
                "DescriptionA",
                "RemarkA",
                "SA_A");
            
            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definitions must be unique!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFailWith1Error_WhenMultipleErrorsInSameRule()
        {
            _stepValidatorMock.Setup(r => r.ExistsAsync(_stepId, default)).Returns(Task.FromResult(false));
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(_stepId, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [TestMethod]
        public async Task Validate_ShouldFailWith1Error_WhenErrorsInDifferentRules()
        {
            _tagValidatorMock.Setup(r => r.ExistsAsync(_createPreAreaTagCommand.GetTagNo(), _projectName, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rdId, default)).Returns(Task.FromResult(false));
            
            var result = await _dut.ValidateAsync(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }
    }
}

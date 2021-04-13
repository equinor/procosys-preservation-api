using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateAreaTag;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.CreateAreaTag
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
        private int _rdForSupplierId = 2;
        private int _rdForOtherThanSupplierId = 3;

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagValidatorMock = new Mock<ITagValidator>();

            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.ExistsAsync(_stepId, default)).Returns(Task.FromResult(true));
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(true));

            _projectValidatorMock = new Mock<IProjectValidator>();

            _rdValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rdForSupplierId, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rdForOtherThanSupplierId, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(new List<int>{_rdForSupplierId, _rdForOtherThanSupplierId}, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.UsageCoversForSuppliersAsync(new List<int>{_rdForSupplierId}, default)).Returns(Task.FromResult(true));

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
                    new RequirementForCommand(_rdForSupplierId, 1),
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1)
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
                    new RequirementForCommand(_rdForSupplierId, 1)
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
        public void Validate_ShouldBeValid_ForPreArea_WhenOkState()
        {
            var result = _dut.Validate(_createPreAreaTagCommand);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_ForPoArea_WhenOkState()
        {
            var result = _dut.Validate(_createPoAreaTagCommand);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyTagAlreadyExists()
        {
            _tagValidatorMock.Setup(r => r.ExistsAsync(_createPreAreaTagCommand.GetTagNo(), _projectName, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag already exists in scope for project!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenProjectExistsButClosed()
        {
            _projectValidatorMock.Setup(r => r.IsExistingAndClosedAsync(_projectName, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project is closed!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepNotExists()
        {
            _stepValidatorMock.Setup(r => r.ExistsAsync(_stepId, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepIsVoided()
        {
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(_stepId, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyRequirementDefinitionNotExists()
        {
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rdForOtherThanSupplierId, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition doesn't exist!"));
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains(_rdForOtherThanSupplierId.ToString()));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyRequirementDefinitionIsVoided()
        {
            _rdValidatorMock.Setup(r => r.IsVoidedAsync(_rdForOtherThanSupplierId, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition is voided!"));
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains(_rdForOtherThanSupplierId.ToString()));
        }

        #region special validation for PreArea tags

        [TestMethod]
        public void Validate_ShouldFailForNonSupplierStep_ForPreArea_WhenNoRequirementsGiven()
        {
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));
            var command = new CreateAreaTagCommand(
                _projectName,
                TagType.PreArea,
                "DisciplineA",
                "AreaA",
                null,
                null,
                _stepId,
                new List<RequirementForCommand>(),
                "DescriptionA",
                "RemarkA",
                "SA_A");
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must include requirements to be used for other than suppliers!"));
        }

        [TestMethod]
        public void Validate_ShouldBeValidForNonSupplierStep_ForPreArea_WhenRequirementsForOtherGiven()
        {
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));
            _rdValidatorMock.Setup(r => r.UsageCoversForOtherThanSuppliersAsync(new List<int>{_rdForSupplierId, _rdForOtherThanSupplierId}, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_createPreAreaTagCommand);

            Assert.IsTrue(result.IsValid);
        }
        
        [TestMethod]
        public void Validate_ShouldFailForNonSupplierStep_ForPreArea_WhenRequirementForSupplierOnlyGiven()
        {
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));
            _rdValidatorMock.Setup(r => r.UsageCoversForOtherThanSuppliersAsync(new List<int>{_rdForSupplierId, _rdForOtherThanSupplierId}, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.HasAnyForSupplierOnlyUsageAsync(new List<int>{_rdForSupplierId, _rdForOtherThanSupplierId}, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements can not include requirements just for suppliers!"));
        }
        
        [TestMethod]
        public void Validate_ShouldFailForNonSupplierStep_ForPreArea_WhenRequirementForOtherNotGiven()
        {
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));
            _rdValidatorMock.Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(new List<int>{_rdForSupplierId, _rdForOtherThanSupplierId}, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must include requirements to be used for other than suppliers!"));
        }

        [TestMethod]
        public void Validate_ShouldFailForSupplierStep_ForPreArea_WhenNoRequirementsGiven()
        {
            var command = new CreateAreaTagCommand(
                _projectName,
                TagType.PreArea,
                "DisciplineA",
                "AreaA",
                null,
                null,
                _stepId,
                new List<RequirementForCommand>(),
                "DescriptionA",
                "RemarkA",
                "SA_A");
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must include requirements to be used both for supplier and other than suppliers!"));
        }

        [TestMethod]
        public void Validate_ShouldFailForSupplierStep_ForPreArea_WhenRequirementForSupplierNotGiven()
        {
            _rdValidatorMock.Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(new List<int>{_rdForSupplierId, _rdForOtherThanSupplierId}, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must include requirements to be used both for supplier and other than suppliers!"));
        }

        #endregion

        #region Special validation for PoArea tags

        [TestMethod]
        public void Validate_ShouldFailForNonSupplierStep_WhenPoArea()
        {
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_createPoAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Step for a {TagType.PoArea.GetTagNoPrefix()} tag need to be for supplier!"));
        }

        [TestMethod]
        public void Validate_ShouldFailForSupplierStep_ForPoArea_WhenNoRequirementsGiven()
        {
            var command = new CreateAreaTagCommand(
                _projectName,
                TagType.PoArea,
                "DisciplineA",
                "AreaA",
                null,
                null,
                _stepId,
                new List<RequirementForCommand>(),
                "DescriptionA",
                "RemarkA",
                "SA_A");
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must include requirements to be used for supplier!"));
        }

        [TestMethod]
        public void Validate_ShouldFailForSupplierStep_ForPoArea_WhenRequirementForSupplierNotGiven()
        {
            _rdValidatorMock.Setup(r => r.UsageCoversForSuppliersAsync(new List<int>{_rdForSupplierId}, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_createPoAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must include requirements to be used for supplier!"));
        }

        #endregion

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementsNotUnique()
        {
            _rdValidatorMock
                .Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(
                    new List<int> {_rdForSupplierId, _rdForOtherThanSupplierId, _rdForSupplierId}, default))
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
                    new RequirementForCommand(_rdForSupplierId, 1),
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1),
                    new RequirementForCommand(_rdForSupplierId, 1)
                },
                "DescriptionA",
                "RemarkA",
                "SA_A");
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definitions must be unique!"));
        }

        [TestMethod]
        public void Validate_ShouldFailWith1Error_WhenMultipleErrorsInSameRule()
        {
            _stepValidatorMock.Setup(r => r.ExistsAsync(_stepId, default)).Returns(Task.FromResult(false));
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(_stepId, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [TestMethod]
        public void Validate_ShouldFailWith1Error_WhenErrorsInDifferentRules()
        {
            _tagValidatorMock.Setup(r => r.ExistsAsync(_createPreAreaTagCommand.GetTagNo(), _projectName, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rdForOtherThanSupplierId, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_createPreAreaTagCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }
    }
}

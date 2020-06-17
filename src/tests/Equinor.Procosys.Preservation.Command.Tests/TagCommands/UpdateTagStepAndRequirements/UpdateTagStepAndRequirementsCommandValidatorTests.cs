using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.UpdateTagStepAndRequirements;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.UpdateTagStepAndRequirements
{
    [TestClass]
    public class UpdateTagStepAndRequirementsCommandValidatorTests
    {
        private UpdateTagStepAndRequirementsCommandValidator _dut;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IStepValidator> _stepValidatorMock;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<IRequirementDefinitionValidator> _rdValidatorMock;
        private UpdateTagStepAndRequirementsCommand _command;

        private int _supplierStep = 1;

        private int _rd1BothSupplierAndOtherId = 2;
        private int _rd2BothSupplierAndOtherId = 3;
        private int _rd3NotSupplierId = 4;
        private int _tagId = 123;
        private int _voidedTagId = 456;
        private int _notFoundTagId = 789;
        private int _tagIdOnClosedProject = 321;


        [TestInitialize]
        public void Setup_OkState()
        {
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(t => t.ExistsAsync(_tagId, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.ExistsAsync(_voidedTagId, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.ExistsAsync(_tagIdOnClosedProject, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.ExistsAsync(_notFoundTagId, default)).Returns(Task.FromResult(false));
            _tagValidatorMock.Setup(t => t.IsVoidedAsync(_voidedTagId, default)).Returns(Task.FromResult(true));

            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.ExistsAsync(_supplierStep, default)).Returns(Task.FromResult(true));
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_supplierStep, default)).Returns(Task.FromResult(true));

            _projectValidatorMock = new Mock<IProjectValidator>();
            _projectValidatorMock.Setup(p => p.IsClosedForTagAsync(_tagId, default)).Returns(Task.FromResult(false));
            _projectValidatorMock.Setup(p => p.IsClosedForTagAsync(_voidedTagId, default)).Returns(Task.FromResult(false));
            _projectValidatorMock.Setup(p => p.IsClosedForTagAsync(_tagIdOnClosedProject, default)).Returns(Task.FromResult(true));

            _rdValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rd1BothSupplierAndOtherId, default))
                .Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rd2BothSupplierAndOtherId, default))
                .Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rd3NotSupplierId, default)).Returns(Task.FromResult(true));
            _rdValidatorMock
                .Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(
                    new List<int> {_rd1BothSupplierAndOtherId, _rd2BothSupplierAndOtherId, _rd3NotSupplierId}, default))
                .Returns(Task.FromResult(true));
            _rdValidatorMock
                .Setup(
                    r => r.UsageCoversForOtherThanSuppliersAsync(new List<int> {_rd1BothSupplierAndOtherId}, default))
                .Returns(Task.FromResult(true));

            _dut = new UpdateTagStepAndRequirementsCommandValidator(_projectValidatorMock.Object,
                _tagValidatorMock.Object, _stepValidatorMock.Object, _rdValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            // Arrange
            _command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                _supplierStep,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_rd1BothSupplierAndOtherId, 1, true),
                    new UpdateRequirementForCommand(_rd2BothSupplierAndOtherId, 1, false)
                },
                new List<RequirementForCommand> {new RequirementForCommand(_rd3NotSupplierId, 1),},
                null);

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyReqAlreadyExists()
        {
            // Arrange
            _command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                _supplierStep,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_rd1BothSupplierAndOtherId, 1, true),
                    new UpdateRequirementForCommand(_rd2BothSupplierAndOtherId, 1, false)
                },
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rd1BothSupplierAndOtherId, 1),
                    new RequirementForCommand(_rd2BothSupplierAndOtherId, 1)
                },
                null);

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenSupplierStepAndHasNoRequirmentsForSupplier()
        {
            // Arrange
            _rdValidatorMock
                .Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(
                    new List<int> {_rd1BothSupplierAndOtherId, _rd2BothSupplierAndOtherId, _rd3NotSupplierId}, default))
                .Returns(Task.FromResult(false));

            _command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                _supplierStep,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_rd1BothSupplierAndOtherId, 1, true),
                    new UpdateRequirementForCommand(_rd2BothSupplierAndOtherId, 1, false)
                },
                new List<RequirementForCommand> {new RequirementForCommand(_rd3NotSupplierId, 1),},
                null);

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagIsVoided()
        {
            // Arrange
            _command = new UpdateTagStepAndRequirementsCommand(
                _voidedTagId,
                _supplierStep,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_rd1BothSupplierAndOtherId, 1, true),
                    new UpdateRequirementForCommand(_rd2BothSupplierAndOtherId, 1, false)
                },
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rd3NotSupplierId, 1),
                },
                null);

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsFalse(result.IsValid);

        }

        [TestMethod]
        public void Validate_ShouldFail_WhenProjectIsClosed()
        {
            // Arrange
            _command = new UpdateTagStepAndRequirementsCommand(
                _tagIdOnClosedProject,
                _supplierStep,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_rd1BothSupplierAndOtherId, 1, true),
                    new UpdateRequirementForCommand(_rd2BothSupplierAndOtherId, 1, false)
                },
                new List<RequirementForCommand> { new RequirementForCommand(_rd3NotSupplierId, 1), },
                null);

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenDoesntExist()
        {
            // Arrange
            _command = new UpdateTagStepAndRequirementsCommand(
                _notFoundTagId,
                _supplierStep,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_rd1BothSupplierAndOtherId, 1, true),
                    new UpdateRequirementForCommand(_rd2BothSupplierAndOtherId, 1, false)
                },
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rd3NotSupplierId, 1),
                },
                null);

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsFalse(result.IsValid);
        }
    }
}

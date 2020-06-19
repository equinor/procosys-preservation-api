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

        private int _tr1Id = 2;
        private int _tr2Id = 3;
        private int _rd3NotSupplierId = 4;
        private int _tagId = 123;
        private int _voidedTagId = 456;
        private int _notFoundTagId = 789;
        private int _tagIdOnClosedProject = 321;
        private const string RowVersion = "AAAAAAAAD6U=";

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
            _projectValidatorMock.Setup(p => p.IsClosedForTagAsync(_voidedTagId, default))
                .Returns(Task.FromResult(false));
            _projectValidatorMock.Setup(p => p.IsClosedForTagAsync(_tagIdOnClosedProject, default))
                .Returns(Task.FromResult(true));

            _rdValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _rdValidatorMock.Setup(r => r.ExistsAsync(_tr1Id, default))
                .Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.ExistsAsync(_tr2Id, default))
                .Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rd3NotSupplierId, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.BeUniqueRequirements(new List<int>{_tr1Id, _tr2Id}, new List<int>(), default))
                .Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.BeUniqueRequirements(new List<int>(), new List<int>(), default))
                .Returns(Task.FromResult(true));
            _rdValidatorMock
                .Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(
                    new List<int> {_tr1Id, _tr2Id, _rd3NotSupplierId}, default))
                .Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.RequirementUsageIsForAllJourneysAsync(new List<int> { _tr1Id, _tr2Id }, new List<int>(), default))
                .Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.RequirementUsageIsForAllJourneysAsync(new List<int> (), new List<int>(), default))
                .Returns(Task.FromResult(true));
            _rdValidatorMock
                .Setup(
                    r => r.UsageCoversForOtherThanSuppliersAsync(new List<int> {_tr1Id}, default))
                .Returns(Task.FromResult(true));


        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            _dut = new UpdateTagStepAndRequirementsCommandValidator(_projectValidatorMock.Object,
                _tagValidatorMock.Object, _stepValidatorMock.Object, _rdValidatorMock.Object);

            // Arrange
            _command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                _supplierStep,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_tr1Id, 1, true, RowVersion),
                    new UpdateRequirementForCommand(_tr2Id, 1, false, RowVersion)
                },
                new List<RequirementForCommand>(),
                null);

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenEmptyLists()
        {
            _dut = new UpdateTagStepAndRequirementsCommandValidator(_projectValidatorMock.Object,
                _tagValidatorMock.Object, _stepValidatorMock.Object, _rdValidatorMock.Object);

            // Arrange
            _command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                _supplierStep,
                new List<UpdateRequirementForCommand>(),
                new List<RequirementForCommand>(),
                null);

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyRequirementAlreadyExists()
        {
            _dut = new UpdateTagStepAndRequirementsCommandValidator(_projectValidatorMock.Object,
                _tagValidatorMock.Object, _stepValidatorMock.Object, _rdValidatorMock.Object);

            // Arrange
            _command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                _supplierStep,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_tr1Id, 1, true, RowVersion),
                    new UpdateRequirementForCommand(_tr2Id, 1, false, RowVersion)
                },
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_tr1Id, 1), new RequirementForCommand(_tr2Id, 1)
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
            _dut = new UpdateTagStepAndRequirementsCommandValidator(_projectValidatorMock.Object,
                _tagValidatorMock.Object, _stepValidatorMock.Object, _rdValidatorMock.Object);

            // Arrange
            _rdValidatorMock
                .Setup(r => r.UsageCoversBothForSupplierAndOtherAsync(
                    new List<int> {_tr1Id, _tr2Id, _rd3NotSupplierId},
                    default))
                .Returns(Task.FromResult(false));

            _command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                _supplierStep,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_tr1Id, 1, true, RowVersion),
                    new UpdateRequirementForCommand(_tr2Id, 1, false, RowVersion)
                },
                new List<RequirementForCommand> {new RequirementForCommand(_rd3NotSupplierId, 1)},
                null);

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagIsVoided()
        {
            _dut = new UpdateTagStepAndRequirementsCommandValidator(_projectValidatorMock.Object,
                _tagValidatorMock.Object, _stepValidatorMock.Object, _rdValidatorMock.Object);

            // Arrange
            _command = new UpdateTagStepAndRequirementsCommand(
                _voidedTagId,
                _supplierStep,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_tr1Id, 1, true, RowVersion),
                    new UpdateRequirementForCommand(_tr2Id, 1, false, RowVersion)
                },
                new List<RequirementForCommand> {new RequirementForCommand(_rd3NotSupplierId, 1)},
                null);

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenProjectIsClosed()
        {
            _dut = new UpdateTagStepAndRequirementsCommandValidator(_projectValidatorMock.Object,
                _tagValidatorMock.Object, _stepValidatorMock.Object, _rdValidatorMock.Object);

            // Arrange
            _command = new UpdateTagStepAndRequirementsCommand(
                _tagIdOnClosedProject,
                _supplierStep,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_tr1Id, 1, true, RowVersion),
                    new UpdateRequirementForCommand(_tr2Id, 1, false, RowVersion)
                },
                new List<RequirementForCommand> {new RequirementForCommand(_rd3NotSupplierId, 1)},
                null);

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagNotExists()
        {
            _dut = new UpdateTagStepAndRequirementsCommandValidator(_projectValidatorMock.Object,
                _tagValidatorMock.Object, _stepValidatorMock.Object, _rdValidatorMock.Object);

            // Arrange
            _command = new UpdateTagStepAndRequirementsCommand(
                _notFoundTagId,
                _supplierStep,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_tr1Id, 1, true, RowVersion),
                    new UpdateRequirementForCommand(_tr2Id, 1, false, RowVersion)
                },
                new List<RequirementForCommand> {new RequirementForCommand(_rd3NotSupplierId, 1),},
                null);

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsFalse(result.IsValid);
        }
    }
}

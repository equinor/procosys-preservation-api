using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.UpdateTagStepAndRequirements;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
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
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private UpdateTagStepAndRequirementsCommand _addTwoReqsCommand;

        private int _stepId = 1;

        private int _tagReqForSupplierId = 12;
        private int _tagReqForOtherThanSupplierId = 13;
        private int _rdForSupplierId = 2;
        private int _rdForOtherThanSupplierId = 3;
        private int _reqDef3Id = 4;
        private int _tagId = 123;
        private const string RowVersion = "AAAAAAAAD6U=";

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(t => t.ExistsAsync(_tagId, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.HasRequirementAsync(_tagId, _tagReqForSupplierId, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.HasRequirementAsync(_tagId, _tagReqForOtherThanSupplierId, default)).Returns(Task.FromResult(true));
            
            _tagValidatorMock.Setup(t => t.AllRequirementsWillBeUniqueAsync(_tagId, new List<int>{_rdForSupplierId, _rdForOtherThanSupplierId}, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageWillCoverBothForSupplierAndOtherAsync(
                    _tagId,
                    new List<int>(),
                    new List<int>(),
                    new List<int> {_rdForSupplierId, _rdForOtherThanSupplierId},
                    default))
                .Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageWillCoverForOtherThanSuppliersAsync(
                    _tagId,
                    new List<int>(),
                    new List<int>(),
                    new List<int> {_rdForSupplierId, _rdForOtherThanSupplierId},
                    default))
                .Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageWillCoverForSuppliersAsync(
                    _tagId,
                    new List<int>(),
                    new List<int>(),
                    new List<int> {_rdForSupplierId, _rdForOtherThanSupplierId},
                    default))
                .Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.HasStepAsync(_tagId, _stepId, default)).Returns(Task.FromResult(true));

            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.ExistsAsync(_stepId, default)).Returns(Task.FromResult(true));
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(true));

            _projectValidatorMock = new Mock<IProjectValidator>();

            _rdValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rdForSupplierId, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rdForOtherThanSupplierId, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(RowVersion)).Returns(true);

            _dut = new UpdateTagStepAndRequirementsCommandValidator(
                _projectValidatorMock.Object,
                _tagValidatorMock.Object,
                _stepValidatorMock.Object,
                _rdValidatorMock.Object,
                _rowVersionValidatorMock.Object);

            _addTwoReqsCommand = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                null,
                _stepId,
                null, 
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdForSupplierId, 1), 
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1)
                },
                null, 
                RowVersion);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_ForSupplierStep_WhenAddingNewUniqueRequirements()
        {
            // Act
            var result = _dut.Validate(_addTwoReqsCommand);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_ForOtherStep_WhenAddingNewUniqueRequirements()
        {
            // Arrange
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));
            
            // Act
            var result = _dut.Validate(_addTwoReqsCommand);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_ForSupplierStep_WhenNeitherUpdateOrAdd()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.RequirementUsageWillCoverBothForSupplierAndOtherAsync(
                    _tagId,
                    new List<int>(),
                    new List<int>(),
                    new List<int>(),
                    default))
                .Returns(Task.FromResult(true));
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                null,
                _stepId,
                null,
                null,
                null, 
                RowVersion);

            // Act
            var result = _dut.Validate(command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_ForOtherStep_WhenNeitherUpdateOrAdd()
        {
            // Arrange
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));
            _tagValidatorMock.Setup(t => t.RequirementUsageWillCoverForOtherThanSuppliersAsync(
                    _tagId,
                    new List<int>(),
                    new List<int>(),
                    new List<int>(),
                    default))
                .Returns(Task.FromResult(true));
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                null,
                _stepId,
                null,
                null,
                null, 
                RowVersion);

            // Act
            var result = _dut.Validate(command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }
        
        [TestMethod]
        public void Validate_ShouldBeValid_ForSupplierStep_WhenVoidingAllRequirementsButAddingNew()
        {
            // Arrange
            _rdValidatorMock.Setup(r => r.ExistsAsync(_reqDef3Id, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.AllRequirementsWillBeUniqueAsync(_tagId, new List<int>{_reqDef3Id}, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageWillCoverBothForSupplierAndOtherAsync(
                    _tagId,
                    new List<int>(),
                    new List<int> {_tagReqForSupplierId, _tagReqForOtherThanSupplierId},
                    new List<int> {_reqDef3Id},
                    default))
                .Returns(Task.FromResult(true));
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                null,
                _stepId,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_tagReqForSupplierId, 1, true, RowVersion),
                    new UpdateRequirementForCommand(_tagReqForOtherThanSupplierId, 1, true, RowVersion)
                },
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_reqDef3Id, 1)
                },
                null, 
                RowVersion);

            // Act
            var result = _dut.Validate(command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }
        
        [TestMethod]
        public void Validate_ShouldBeValid_ForOtherStep_WhenVoidingAllRequirementsButAddingNew()
        {
            // Arrange
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));
            _rdValidatorMock.Setup(r => r.ExistsAsync(_reqDef3Id, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.AllRequirementsWillBeUniqueAsync(_tagId, new List<int>{_reqDef3Id}, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageWillCoverForOtherThanSuppliersAsync(
                    _tagId,
                    new List<int>(),
                    new List<int> {_tagReqForSupplierId, _tagReqForOtherThanSupplierId},
                    new List<int> {_reqDef3Id}, default))
                .Returns(Task.FromResult(true));
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                null,
                _stepId,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_tagReqForSupplierId, 1, true, RowVersion),
                    new UpdateRequirementForCommand(_tagReqForOtherThanSupplierId, 1, true, RowVersion)
                },
                new List<RequirementForCommand> {new RequirementForCommand(_reqDef3Id, 1)},
                null, 
                RowVersion);

            // Act
            var result = _dut.Validate(command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyRequirementAlreadyExists()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.AllRequirementsWillBeUniqueAsync(_tagId, new List<int>{_rdForSupplierId, _rdForOtherThanSupplierId}, default)).Returns(Task.FromResult(false));

            // Act
            var result = _dut.Validate(_addTwoReqsCommand);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must be unique!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementToUpdateNotExists()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.HasRequirementAsync(_tagId, _tagReqForSupplierId, default)).Returns(Task.FromResult(false));
            _tagValidatorMock.Setup(t => t.AllRequirementsWillBeUniqueAsync(_tagId, new List<int>{_rdForOtherThanSupplierId}, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageWillCoverBothForSupplierAndOtherAsync(
                    _tagId,
                    new List<int>{_tagReqForSupplierId},
                    new List<int>(),
                    new List<int> {_rdForOtherThanSupplierId},
                    default))
                .Returns(Task.FromResult(true));
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                null,
                _stepId,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_tagReqForSupplierId, 1, false, RowVersion)
                },
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1)
                },
                null, 
                RowVersion);

            // Act
            var result = _dut.Validate(command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementToDeleteNotExists()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.HasRequirementAsync(_tagId, _tagReqForSupplierId, default)).Returns(Task.FromResult(false));
            _tagValidatorMock.Setup(t => t.AllRequirementsWillBeUniqueAsync(_tagId, new List<int>{_rdForOtherThanSupplierId}, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageWillCoverBothForSupplierAndOtherAsync(
                    _tagId,
                    new List<int>(),
                    new List<int>(),
                    new List<int> {_rdForOtherThanSupplierId}, default))
                .Returns(Task.FromResult(true));
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                null,
                _stepId,
                null,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1)
                },
                new List<DeleteRequirementForCommand>
                {
                    new DeleteRequirementForCommand(_tagReqForSupplierId, RowVersion)
                },
                RowVersion);

            // Act
            var result = _dut.Validate(command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementToDeleteNotVoided()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.AllRequirementsWillBeUniqueAsync(_tagId, new List<int>{_rdForOtherThanSupplierId}, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageWillCoverBothForSupplierAndOtherAsync(
                    _tagId,
                    new List<int>(),
                    new List<int>(),
                    new List<int> {_rdForOtherThanSupplierId},
                    default))
                .Returns(Task.FromResult(true));
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                null,
                _stepId,
                null,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1)
                },
                new List<DeleteRequirementForCommand>
                {
                    new DeleteRequirementForCommand(_tagReqForSupplierId, RowVersion)
                },
                RowVersion);

            // Act
            var result = _dut.Validate(command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement is not voided!"));
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenRequirementToDelete_IsVoidedInAdvance()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.AllRequirementsWillBeUniqueAsync(_tagId, new List<int>{_rdForOtherThanSupplierId}, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageWillCoverBothForSupplierAndOtherAsync(
                    _tagId,
                    new List<int>(),
                    new List<int>(),
                    new List<int> {_rdForOtherThanSupplierId},
                    default))
                .Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.IsRequirementVoidedAsync(_tagId, _tagReqForSupplierId, default)).Returns(Task.FromResult(true));
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                null,
                _stepId,
                null,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1)
                },
                new List<DeleteRequirementForCommand>
                {
                    new DeleteRequirementForCommand(_tagReqForSupplierId, RowVersion)
                },
                RowVersion);

            // Act
            var result = _dut.Validate(command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenRequirementToDelete_WillBeVoidedInSameCommand()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.AllRequirementsWillBeUniqueAsync(_tagId, new List<int>{_rdForOtherThanSupplierId}, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageWillCoverBothForSupplierAndOtherAsync(
                    _tagId,
                    new List<int>(),
                    new List<int> {_tagReqForSupplierId},
                    new List<int> {_rdForOtherThanSupplierId},
                    default))
                .Returns(Task.FromResult(true));
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                null,
                _stepId,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_tagReqForSupplierId, 1, true, RowVersion)
                }, 
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1)
                },
                new List<DeleteRequirementForCommand>
                {
                    new DeleteRequirementForCommand(_tagReqForSupplierId, RowVersion)
                },
                RowVersion);

            // Act
            var result = _dut.Validate(command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagIsVoided()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.IsVoidedAsync(_tagId, default)).Returns(Task.FromResult(true));

            // Act
            var result = _dut.Validate(_addTwoReqsCommand);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is voided!"));
        }
        
        [TestMethod]
        public void Validate_ShouldFail_WhenChangeToAVoidedStep()
        {
            _tagValidatorMock.Setup(r => r.HasStepAsync(_tagId, _stepId, default)).Returns(Task.FromResult(false));
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(_stepId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_addTwoReqsCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step is voided!"));
        }
        
        [TestMethod]
        public void Validate_ShouldBeValid_WhenExistingStepIsVoided()
        {
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(_stepId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_addTwoReqsCommand);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenProjectIsClosed()
        {
            // Arrange
            _projectValidatorMock.Setup(p => p.IsClosedForTagAsync(_tagId, default)).Returns(Task.FromResult(true));

            // Act
            var result = _dut.Validate(_addTwoReqsCommand);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagNotExists()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.ExistsAsync(_tagId, default)).Returns(Task.FromResult(false));

            // Act
            var result = _dut.Validate(_addTwoReqsCommand);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_ForPoAreaTag_WhenTargetStepNotForSupplier()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.VerifyTagTypeAsync(_tagId, TagType.PoArea, default)).Returns(Task.FromResult(true));
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));

            // Act
            var result = _dut.Validate(_addTwoReqsCommand);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Step for a {TagType.PoArea.GetTagNoPrefix()} tag needs to be for supplier!"));
        }

        [TestMethod]
        public void Validate_ShouldBeValid_ForPoAreaTag_WhenTargetStepForSupplier()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.VerifyTagTypeAsync(_tagId, TagType.PoArea, default)).Returns(Task.FromResult(true));

            // Act
            var result = _dut.Validate(_addTwoReqsCommand);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_ForPoAreaTag_WhenNoRequirementForSupplier()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.VerifyTagTypeAsync(_tagId, TagType.PoArea, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageWillCoverForSuppliersAsync(
                    _tagId,
                    new List<int>(),
                    new List<int>(),
                    new List<int> {_rdForSupplierId, _rdForOtherThanSupplierId},
                    default))
                .Returns(Task.FromResult(false));

            // Act
            var result = _dut.Validate(_addTwoReqsCommand);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must include requirements to be used for supplier!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_ForPoAreaTag_WhenRequirementForOtherThanSupplier()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.VerifyTagTypeAsync(_tagId, TagType.PoArea, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementHasAnyForOtherThanSuppliersUsageAsync(
                    _tagId,
                    new List<int>(),
                    new List<int>(),
                    new List<int> {_rdForSupplierId, _rdForOtherThanSupplierId},
                    default))
                .Returns(Task.FromResult(true));

            // Act
            var result = _dut.Validate(_addTwoReqsCommand);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements can not include requirements for other than suppliers!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";

            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                null,
                _stepId,
                null,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdForSupplierId, 1),
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1)
                },
                null, 
                invalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion)).Returns(false);

            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenDescriptionIsBlankForAreaTag()
        {
            _tagValidatorMock.Setup(t => t.VerifyTagIsAreaTagAsync(_tagId, default)).Returns(Task.FromResult(true));
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                "",
                _stepId,
                null,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdForSupplierId, 1),
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1)
                },
                null, 
                RowVersion);

            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Description can not be blank!"));
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenSettingDescriptionOnAreaTag()
        {
            _tagValidatorMock.Setup(t => t.VerifyTagIsAreaTagAsync(_tagId, default)).Returns(Task.FromResult(true));
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                "Desc",
                _stepId,
                null,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdForSupplierId, 1),
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1)
                },
                null, 
                RowVersion);

            var result = _dut.Validate(command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenSettingDescriptionOnOtherThanAreaTag()
        {
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                "Desc",
                _stepId,
                null,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdForSupplierId, 1),
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1)
                },
                null, 
                RowVersion);

            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag must be an area tag to update description!"));
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenKeepingSameDescriptionOnOtherThanAreaTag()
        {
            var description = "Desc";
            _tagValidatorMock.Setup(t => t.VerifyTagDescriptionAsync(_tagId, description, default)).Returns(Task.FromResult(true));
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                description,
                _stepId,
                null,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdForSupplierId, 1),
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1)
                },
                null, 
                RowVersion);

            var result = _dut.Validate(command);

            Assert.IsTrue(result.IsValid);
        }
    }
}

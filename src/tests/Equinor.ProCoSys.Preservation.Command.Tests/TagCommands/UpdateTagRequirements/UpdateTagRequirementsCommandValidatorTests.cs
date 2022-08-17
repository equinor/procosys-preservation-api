using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTagRequirements;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.UpdateTagRequirements
{
    [TestClass]
    public class UpdateTagRequirementsCommandValidatorTests
    {
        private UpdateTagRequirementsCommandValidator _dut;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<IRequirementDefinitionValidator> _rdValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private UpdateTagRequirementsCommand _addTwoReqsCommand;

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
            _tagValidatorMock.Setup(t => t.IsInASupplierStepAsync(_tagId, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.ExistsAsync(_tagId, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.IsReadyToBeEditedAsync(_tagId, default)).Returns(Task.FromResult(true));
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

            _projectValidatorMock = new Mock<IProjectValidator>();

            _rdValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rdForSupplierId, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.ExistsAsync(_rdForOtherThanSupplierId, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(RowVersion)).Returns(true);

            _dut = new UpdateTagRequirementsCommandValidator(
                _projectValidatorMock.Object,
                _tagValidatorMock.Object,
                _rdValidatorMock.Object,
                _rowVersionValidatorMock.Object);

            _addTwoReqsCommand = new UpdateTagRequirementsCommand(
                _tagId,
                null,
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
        public async Task Validate_ShouldBeValid_ForSupplierStep_WhenAddingNewUniqueRequirements()
        {
            // Act
            var result = await _dut.ValidateAsync(_addTwoReqsCommand);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_ForOtherStep_WhenAddingNewUniqueRequirements()
        {
            // Arrange
            _tagValidatorMock.Setup(r => r.IsInASupplierStepAsync(_tagId, default)).Returns(Task.FromResult(false));

            // Act
            var result = await _dut.ValidateAsync(_addTwoReqsCommand);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_ForSupplierStep_WhenNeitherUpdateOrAdd()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.RequirementUsageWillCoverBothForSupplierAndOtherAsync(
                    _tagId,
                    new List<int>(),
                    new List<int>(),
                    new List<int>(),
                    default))
                .Returns(Task.FromResult(true));
            var command = new UpdateTagRequirementsCommand(
                _tagId,
                null,
                null,
                null,
                null, 
                RowVersion);

            // Act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_ForOtherStep_WhenNeitherUpdateOrAdd()
        {
            // Arrange
            _tagValidatorMock.Setup(r => r.IsInASupplierStepAsync(_tagId, default)).Returns(Task.FromResult(false));
            _tagValidatorMock.Setup(t => t.RequirementUsageWillCoverForOtherThanSuppliersAsync(
                    _tagId,
                    new List<int>(),
                    new List<int>(),
                    new List<int>(),
                    default))
                .Returns(Task.FromResult(true));
            var command = new UpdateTagRequirementsCommand(
                _tagId,
                null,
                null,
                null,
                null, 
                RowVersion);

            // Act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }
        
        [TestMethod]
        public async Task Validate_ShouldBeValid_ForSupplierStep_WhenVoidingAllRequirementsButAddingNew()
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
            var command = new UpdateTagRequirementsCommand(
                _tagId,
                null,
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
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }
        
        [TestMethod]
        public async Task Validate_ShouldBeValid_ForOtherStep_WhenVoidingAllRequirementsButAddingNew()
        {
            // Arrange
            _tagValidatorMock.Setup(r => r.IsInASupplierStepAsync(_tagId, default)).Returns(Task.FromResult(false));
            _rdValidatorMock.Setup(r => r.ExistsAsync(_reqDef3Id, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.AllRequirementsWillBeUniqueAsync(_tagId, new List<int>{_reqDef3Id}, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageWillCoverForOtherThanSuppliersAsync(
                    _tagId,
                    new List<int>(),
                    new List<int> {_tagReqForSupplierId, _tagReqForOtherThanSupplierId},
                    new List<int> {_reqDef3Id}, default))
                .Returns(Task.FromResult(true));
            var command = new UpdateTagRequirementsCommand(
                _tagId,
                null,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_tagReqForSupplierId, 1, true, RowVersion),
                    new UpdateRequirementForCommand(_tagReqForOtherThanSupplierId, 1, true, RowVersion)
                },
                new List<RequirementForCommand> {new RequirementForCommand(_reqDef3Id, 1)},
                null, 
                RowVersion);

            // Act
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenAnyRequirementAlreadyExists()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.AllRequirementsWillBeUniqueAsync(_tagId, new List<int>{_rdForSupplierId, _rdForOtherThanSupplierId}, default)).Returns(Task.FromResult(false));

            // Act
            var result = await _dut.ValidateAsync(_addTwoReqsCommand);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must be unique!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementToUpdateNotExists()
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
            var command = new UpdateTagRequirementsCommand(
                _tagId,
                null,
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
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementToDeleteNotExists()
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
            var command = new UpdateTagRequirementsCommand(
                _tagId,
                null,
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
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenRequirementToDeleteNotVoided()
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
            var command = new UpdateTagRequirementsCommand(
                _tagId,
                null,
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
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement is not voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenRequirementToDelete_IsVoidedInAdvance()
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
            var command = new UpdateTagRequirementsCommand(
                _tagId,
                null,
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
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenRequirementToDelete_WillBeVoidedInSameCommand()
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
            var command = new UpdateTagRequirementsCommand(
                _tagId,
                null,
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
            var result = await _dut.ValidateAsync(command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenTagIsVoided()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.IsVoidedAsync(_tagId, default)).Returns(Task.FromResult(true));

            // Act
            var result = await _dut.ValidateAsync(_addTwoReqsCommand);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is voided!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenProjectIsClosed()
        {
            // Arrange
            _projectValidatorMock.Setup(p => p.IsClosedForTagAsync(_tagId, default)).Returns(Task.FromResult(true));

            // Act
            var result = await _dut.ValidateAsync(_addTwoReqsCommand);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project for tag is closed!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenTagNotExists()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.ExistsAsync(_tagId, default)).Returns(Task.FromResult(false));

            // Act
            var result = await _dut.ValidateAsync(_addTwoReqsCommand);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't exist!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenTagCantBeEdited()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.IsReadyToBeEditedAsync(_tagId, default)).Returns(Task.FromResult(false));

            // Act
            var result = await _dut.ValidateAsync(_addTwoReqsCommand);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag can't be edited!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_ForPoAreaTag_WhenNoRequirementForSupplier()
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
            var result = await _dut.ValidateAsync(_addTwoReqsCommand);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must include requirements to be used for supplier!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_ForPoAreaTag_WhenRequirementForOtherThanSupplier()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.VerifyTagTypeAsync(_tagId, TagType.PoArea, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementWillGetAnyForOtherThanSuppliersUsageAsync(
                    _tagId,
                    new List<int>(),
                    new List<int>(),
                    new List<int> {_rdForSupplierId, _rdForOtherThanSupplierId},
                    default))
                .Returns(Task.FromResult(true));

            // Act
            var result = await _dut.ValidateAsync(_addTwoReqsCommand);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements can not include requirements for other than suppliers!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenInvalidRowVersion()
        {
            const string invalidRowVersion = "String";

            var command = new UpdateTagRequirementsCommand(
                _tagId,
                null,
                null,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdForSupplierId, 1),
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1)
                },
                null, 
                invalidRowVersion);
            _rowVersionValidatorMock.Setup(r => r.IsValid(invalidRowVersion)).Returns(false);

            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenDescriptionIsBlankForAreaTag()
        {
            _tagValidatorMock.Setup(t => t.VerifyTagIsAreaTagAsync(_tagId, default)).Returns(Task.FromResult(true));
            var command = new UpdateTagRequirementsCommand(
                _tagId,
                "",
                null,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdForSupplierId, 1),
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1)
                },
                null, 
                RowVersion);

            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Description can not be blank!"));
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenSettingDescriptionOnAreaTag()
        {
            _tagValidatorMock.Setup(t => t.VerifyTagIsAreaTagAsync(_tagId, default)).Returns(Task.FromResult(true));
            var command = new UpdateTagRequirementsCommand(
                _tagId,
                "Desc",
                null,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdForSupplierId, 1),
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1)
                },
                null, 
                RowVersion);

            var result = await _dut.ValidateAsync(command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenSettingDescriptionOnOtherThanAreaTag()
        {
            var command = new UpdateTagRequirementsCommand(
                _tagId,
                "Desc",
                null,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdForSupplierId, 1),
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1)
                },
                null, 
                RowVersion);

            var result = await _dut.ValidateAsync(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag must be an area tag to update description!"));
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenKeepingSameDescriptionOnOtherThanAreaTag()
        {
            var description = "Desc";
            _tagValidatorMock.Setup(t => t.VerifyTagDescriptionAsync(_tagId, description, default)).Returns(Task.FromResult(true));
            var command = new UpdateTagRequirementsCommand(
                _tagId,
                description,
                null,
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_rdForSupplierId, 1),
                    new RequirementForCommand(_rdForOtherThanSupplierId, 1)
                },
                null, 
                RowVersion);

            var result = await _dut.ValidateAsync(command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldIncludeTagNoInMessage()
        {
            _tagValidatorMock.Setup(r => r.IsVoidedAsync(_tagId, default)).Returns(Task.FromResult(true));
            var tagNo = "Tag X";
            _tagValidatorMock.Setup(r => r.GetTagDetailsAsync(_tagId, default)).Returns(Task.FromResult(tagNo));

            var result = await _dut.ValidateAsync(_addTwoReqsCommand);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains(tagNo));
        }
    }
}

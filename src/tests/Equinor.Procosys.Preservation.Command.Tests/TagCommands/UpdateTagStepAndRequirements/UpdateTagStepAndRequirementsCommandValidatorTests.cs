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
        private UpdateTagStepAndRequirementsCommand _addTwoReqsCommand;

        private int _stepId = 1;

        private int _tagReqForAll1Id = 12;
        private int _tagReqForAll2Id = 13;
        private int _tagReqForOtherId = 14;
        private int _tagReqForSupplierId = 15;
        private int _reqDefForAll1Id = 2;
        private int _reqDefForAll2Id = 3;
        private int _reqDef3Id = 4;
        private int _tagId = 123;
        private const string RowVersion = "AAAAAAAAD6U=";

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(t => t.ExistsAsync(_tagId, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.HasRequirementAsync(_tagId, _tagReqForAll1Id, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.HasRequirementAsync(_tagId, _tagReqForAll2Id, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.HasRequirementAsync(_tagId, _tagReqForOtherId, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.HasRequirementAsync(_tagId, _tagReqForSupplierId, default)).Returns(Task.FromResult(true));
            
            _tagValidatorMock.Setup(t => t.AllRequirementsWillBeUniqueAsync(_tagId, new List<int>{_reqDefForAll1Id, _reqDefForAll2Id}, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.UsageCoversBothForSupplierAndOtherAsync(_tagId, new List<int>(), new List<int>{_reqDefForAll1Id, _reqDefForAll2Id}, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.UsageCoversForOtherThanSuppliersAsync(_tagId, new List<int>(), new List<int>{_reqDefForAll1Id, _reqDefForAll2Id}, default)).Returns(Task.FromResult(true));

            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.ExistsAsync(_stepId, default)).Returns(Task.FromResult(true));
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(true));

            _projectValidatorMock = new Mock<IProjectValidator>();

            _rdValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _rdValidatorMock.Setup(r => r.ExistsAsync(_reqDefForAll1Id, default)).Returns(Task.FromResult(true));
            _rdValidatorMock.Setup(r => r.ExistsAsync(_reqDefForAll2Id, default)).Returns(Task.FromResult(true));

            _dut = new UpdateTagStepAndRequirementsCommandValidator(
                _projectValidatorMock.Object,
                _tagValidatorMock.Object,
                _stepValidatorMock.Object,
                _rdValidatorMock.Object);

            _addTwoReqsCommand = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                _stepId,
                new List<UpdateRequirementForCommand>(), 
                new List<RequirementForCommand>
                {
                    new RequirementForCommand(_reqDefForAll1Id, 1), 
                    new RequirementForCommand(_reqDefForAll2Id, 1)
                },
                null);
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
            _tagValidatorMock.Setup(t => t.UsageCoversBothForSupplierAndOtherAsync(_tagId, new List<int>(), new List<int>(), default)).Returns(Task.FromResult(true));
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                _stepId,
                new List<UpdateRequirementForCommand>(),
                new List<RequirementForCommand>(),
                null);

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
            _tagValidatorMock.Setup(t => t.UsageCoversForOtherThanSuppliersAsync(_tagId, new List<int>(), new List<int>(), default)).Returns(Task.FromResult(true));
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                _stepId,
                new List<UpdateRequirementForCommand>(),
                new List<RequirementForCommand>(),
                null);

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
            _tagValidatorMock.Setup(t => t.UsageCoversBothForSupplierAndOtherAsync(_tagId, new List<int>{_tagReqForAll1Id, _tagReqForAll2Id}, new List<int>{_reqDef3Id}, default)).Returns(Task.FromResult(true));
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                _stepId,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_tagReqForAll1Id, 1, true, RowVersion),
                    new UpdateRequirementForCommand(_tagReqForAll2Id, 1, true, RowVersion)
                },
                new List<RequirementForCommand> {new RequirementForCommand(_reqDef3Id, 1)},
                null);

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
            _tagValidatorMock.Setup(t => t.UsageCoversForOtherThanSuppliersAsync(_tagId, new List<int>{_tagReqForAll1Id, _tagReqForAll2Id}, new List<int>{_reqDef3Id}, default)).Returns(Task.FromResult(true));
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                _stepId,
                new List<UpdateRequirementForCommand>
                {
                    new UpdateRequirementForCommand(_tagReqForAll1Id, 1, true, RowVersion),
                    new UpdateRequirementForCommand(_tagReqForAll2Id, 1, true, RowVersion)
                },
                new List<RequirementForCommand> {new RequirementForCommand(_reqDef3Id, 1)},
                null);

            // Act
            var result = _dut.Validate(command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }
        
        [TestMethod]
        public void Validate_ShouldFail_ForOtherStep_WhenAddingRequirementForSupplierOnly()
        {
            // Arrange
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(_stepId, default)).Returns(Task.FromResult(false));
            _rdValidatorMock.Setup(r => r.ExistsAsync(_reqDef3Id, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.AllRequirementsWillBeUniqueAsync(_tagId, new List<int>{_reqDef3Id}, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.UsageCoversForOtherThanSuppliersAsync(_tagId, new List<int>(), new List<int>{_reqDef3Id}, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.HasAnyForSupplierOnlyUsageAsync(_tagId, new List<int>(), new List<int>{_reqDef3Id}, default)).Returns(Task.FromResult(true));
            var command = new UpdateTagStepAndRequirementsCommand(
                _tagId,
                _stepId,
                new List<UpdateRequirementForCommand>(), 
                new List<RequirementForCommand> {new RequirementForCommand(_reqDef3Id, 1)},
                null);

            // Act
            var result = _dut.Validate(command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements can't include requirements just for suppliers!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyRequirementAlreadyExists()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.AllRequirementsWillBeUniqueAsync(_tagId, new List<int>{_reqDefForAll1Id, _reqDefForAll2Id}, default)).Returns(Task.FromResult(false));

            // Act
            var result = _dut.Validate(_addTwoReqsCommand);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements must be unique!"));
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
    }
}

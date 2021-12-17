using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.UpdateTagStep;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.StepValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.UpdateTagStep
{
    [TestClass]
    public class UpdateTagStepCommandValidatorTests
    {
        private const int StepId = 1;
        private const int TagId1 = 7;
        private const int TagId2 = 8;
        private const string RowVersion1 = "AAAAAAAAABA=";
        private const string RowVersion2 = "AAAAAAAABBA=";

        private UpdateTagStepCommandValidator _dut;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IStepValidator> _stepValidatorMock;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private UpdateTagStepCommand _command;

        private List<int> _tagIds;
        private List<IdAndRowVersion> _tagIdsWithRowVersion;

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagIds = new List<int> { TagId1, TagId2 };
            _tagIdsWithRowVersion = new List<IdAndRowVersion>
            {
                new IdAndRowVersion(TagId1, RowVersion1),
                new IdAndRowVersion(TagId2, RowVersion2)
            };
            _projectValidatorMock = new Mock<IProjectValidator>();
            _projectValidatorMock.Setup(p => p.AllTagsInSameProjectAsync(_tagIds, default)).Returns(Task.FromResult(true));

            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(t => t.ExistsAsync(TagId1, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.ExistsAsync(TagId2, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.IsReadyToBeEditedAsync(TagId1, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.IsReadyToBeEditedAsync(TagId2, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageCoversBothForSupplierAndOtherAsync(TagId1, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageCoversBothForSupplierAndOtherAsync(TagId2, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageCoversForSuppliersAsync(TagId1, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageCoversForSuppliersAsync(TagId2, default)).Returns(Task.FromResult(true));

            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(StepId, default)).Returns(Task.FromResult(true));
            _stepValidatorMock.Setup(r => r.ExistsAsync(StepId, default)).Returns(Task.FromResult(true));

            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(RowVersion1)).Returns(true);
            _rowVersionValidatorMock.Setup(r => r.IsValid(RowVersion2)).Returns(true);

            _dut = new UpdateTagStepCommandValidator(
                _projectValidatorMock.Object,
                _tagValidatorMock.Object,
                _stepValidatorMock.Object,
                _rowVersionValidatorMock.Object);

            _command = new UpdateTagStepCommand(_tagIdsWithRowVersion, StepId);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_ForStdTag_WhenTargetStepForSupplier()
        {
            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_ForStdTag_WhenTargetStepForSupplier_AndRequirementNotCoversBothSupplierAndOther()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.RequirementUsageCoversBothForSupplierAndOtherAsync(TagId1, default)).Returns(Task.FromResult(false));

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Requirements for tag must include requirements to be used both for supplier and other than suppliers!"));
        }

        [TestMethod]
        public void Validate_ShouldBeValid_ForStdTag_WhenTargetStepNotForSupplier()
        {
            // Arrange
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(StepId, default)).Returns(Task.FromResult(false));
            _tagValidatorMock.Setup(t => t.RequirementUsageCoversForOtherThanSuppliersAsync(TagId1, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageCoversForOtherThanSuppliersAsync(TagId2, default)).Returns(Task.FromResult(true));

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_ForStdTag_WhenTargetStepNotForSupplier_AndRequirementNotCoversOther()
        {
            // Arrange
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(StepId, default)).Returns(Task.FromResult(false));

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Requirements for tag must include requirements to be used for other than suppliers!"));
        }

        [TestMethod]
        public void Validate_ShouldBeValid_ForPoAreaTag_WhenTargetStepForSupplier()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.VerifyTagTypeAsync(TagId1, TagType.PoArea, default)).Returns(Task.FromResult(true));

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_ForPoAreaTag_WhenTargetStepNotForSupplier()
        {
            // Arrange
            _stepValidatorMock.Setup(r => r.IsForSupplierAsync(StepId, default)).Returns(Task.FromResult(false));
            _tagValidatorMock.Setup(t => t.VerifyTagTypeAsync(TagId1, TagType.PoArea, default)).Returns(Task.FromResult(true));

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Step for a {TagType.PoArea.GetTagNoPrefix()} tag needs to be for supplier!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_ForPoAreaTag_WhenRequirementNotCoversSupplier()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.VerifyTagTypeAsync(TagId1, TagType.PoArea, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementUsageCoversForSuppliersAsync(TagId1, default)).Returns(Task.FromResult(false));

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements for tag must include requirements to be used for supplier!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_ForPoAreaTag_WhenRequirementHasAnyForOtherThanSupplier()
        {
            // Arrange
            _tagValidatorMock.Setup(t => t.VerifyTagTypeAsync(TagId1, TagType.PoArea, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(t => t.RequirementHasAnyForOtherThanSuppliersUsageAsync(TagId1, default)).Returns(Task.FromResult(true));

            // Act
            var result = _dut.Validate(_command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirements for tag can not include requirements for other than suppliers!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenNoTagsGiven()
        {
            var command = new UpdateTagStepCommand(new List<IdAndRowVersion>(), StepId);

            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("At least 1 tag must be given!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagsNotUnique()
        {
            var command = new UpdateTagStepCommand(
                new List<IdAndRowVersion> { 
                    new IdAndRowVersion(1, null), 
                    new IdAndRowVersion(1, null) }, StepId);

            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tags must be unique!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepNotExists()
        {
            _stepValidatorMock.Setup(r => r.ExistsAsync(StepId, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepIsVoided()
        {
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(StepId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyTagNotExists()
        {
            _tagValidatorMock.Setup(r => r.ExistsAsync(TagId2, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag doesn't exist!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyTagCantBeEdited()
        {
            _tagValidatorMock.Setup(r => r.IsReadyToBeEditedAsync(TagId2, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag can't be edited!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyTagIsVoided()
        {
            _tagValidatorMock.Setup(r => r.IsVoidedAsync(TagId1, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenProjectForAnyTagIsClosed()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(TagId1, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project is closed!"));
        }
        
        [TestMethod]
        public void Validate_ShouldFail_WhenChangeToAVoidedStep()
        {
            _stepValidatorMock.Setup(r => r.IsVoidedAsync(StepId, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagsInDifferentProjects()
        {
            _projectValidatorMock.Setup(r => r.AllTagsInSameProjectAsync(_tagIds, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tags must be in same project!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenInvalidRowVersion()
        {
            _rowVersionValidatorMock.Setup(r => r.IsValid(RowVersion2)).Returns(false);

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Not a valid row version!"));
        }

        [TestMethod]
        public void Validate_ShouldFailWith1Error_WhenMultipleErrorsInSameRule()
        {
            _projectValidatorMock.Setup(p => p.AllTagsInSameProjectAsync(_tagIds, default)).Returns(Task.FromResult(false));
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(TagId1, default)).Returns(Task.FromResult(true));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tags must be in same project!"));
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.Reschedule;
using Equinor.Procosys.Preservation.Command.Validators;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.Reschedule
{
    [TestClass]
    public class RescheduleCommandValidatorTests
    {
        private const int TagId1 = 7;
        private const int TagId2 = 8;
        private const string RowVersion1 = "AAAAAAAAABA=";
        private const string RowVersion2 = "AAAAAAAABBA=";
        private RescheduleCommandValidator _dut;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IRowVersionValidator> _rowVersionValidatorMock;
        private RescheduleCommand _command;

        private List<int> _tagIds;
        private List<IdAndRowVersion> _tagIdsWithRowVersion;

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagIds = new List<int> {TagId1, TagId2};
            _tagIdsWithRowVersion = new List<IdAndRowVersion>
            {
                new IdAndRowVersion(TagId1, RowVersion1),
                new IdAndRowVersion(TagId2, RowVersion2)
            };
            _projectValidatorMock = new Mock<IProjectValidator>();
            _projectValidatorMock.Setup(p => p.AllTagsInSameProjectAsync(_tagIds, default)).Returns(Task.FromResult(true));
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.ExistsAsync(TagId1, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.ExistsAsync(TagId2, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.IsReadyToBeRescheduledAsync(TagId1, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.IsReadyToBeRescheduledAsync(TagId2, default)).Returns(Task.FromResult(true));
            _command = new RescheduleCommand(_tagIdsWithRowVersion, 1, RescheduledDirection.Later, "Comment");
            _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
            _rowVersionValidatorMock.Setup(r => r.IsValid(RowVersion1)).Returns(true);
            _rowVersionValidatorMock.Setup(r => r.IsValid(RowVersion2)).Returns(true);

            _dut = new RescheduleCommandValidator(
                _projectValidatorMock.Object,
                _tagValidatorMock.Object,
                _rowVersionValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }
        
        [TestMethod]
        public void Validate_ShouldFail_WhenWeeksToLow()
        {
            var command = new RescheduleCommand(_tagIdsWithRowVersion, 0, RescheduledDirection.Later, "Comment");
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Rescheduling must be in range of 1 to {RescheduleCommandValidator.MaxRescheduleWeeks} week(s)!"));
        }
        
        [TestMethod]
        public void Validate_ShouldFail_WhenWeeksToHigh()
        {
            var command = new RescheduleCommand(_tagIdsWithRowVersion, RescheduleCommandValidator.MaxRescheduleWeeks + 1, RescheduledDirection.Later, "Comment");
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Rescheduling must be in range of 1 to {RescheduleCommandValidator.MaxRescheduleWeeks} week(s)!"));
        }
        
        [TestMethod]
        public void Validate_ShouldFail_WhenNoTagsGiven()
        {
            var command = new RescheduleCommand(new List<IdAndRowVersion>(), 1, RescheduledDirection.Later, "Comment");
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("At least 1 tag must be given!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagsNotUnique()
        {
            var command = new RescheduleCommand(
                new List<IdAndRowVersion>
                {
                    new IdAndRowVersion(1, null), new IdAndRowVersion(1, null)
                },1, RescheduledDirection.Later, "Comment");
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tags must be unique!"));
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
        public void Validate_ShouldFail_WhenNotReadyToBeRescheduled()
        {
            _tagValidatorMock.Setup(r => r.IsReadyToBeRescheduledAsync(TagId1, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag can not be rescheduled!"));
        }

        [TestMethod]
        public void Validate_ShouldFailWith1Error_WhenMultipleErrorsInSameRule()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(TagId1, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.ExistsAsync(TagId2, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project is closed!"));
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
    }
}

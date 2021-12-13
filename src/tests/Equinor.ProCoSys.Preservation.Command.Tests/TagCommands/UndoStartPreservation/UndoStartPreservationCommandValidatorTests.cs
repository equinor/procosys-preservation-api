using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.UndoStartPreservation;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.UndoStartPreservation
{
    [TestClass]
    public class UndoStartPreservationCommandValidatorTests
    {
        private UndoStartPreservationCommandValidator _dut;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<ITagValidator> _tagValidatorMock;
        private UndoStartPreservationCommand _command;

        private const int TagId1 = 7;
        private const int TagId2 = 8;
        private List<int> _tagIds;

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagIds = new List<int> {TagId1, TagId2};
            _projectValidatorMock = new Mock<IProjectValidator>();
            _projectValidatorMock.Setup(p => p.AllTagsInSameProjectAsync(_tagIds, default)).Returns(Task.FromResult(true));
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.ExistsAsync(TagId1, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.ExistsAsync(TagId2, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.IsReadyToUndoStartedAsync(TagId1, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.IsReadyToUndoStartedAsync(TagId2, default)).Returns(Task.FromResult(true));
            _command = new UndoStartPreservationCommand(_tagIds);

            _dut = new UndoStartPreservationCommandValidator(_projectValidatorMock.Object, _tagValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }
        
        [TestMethod]
        public void Validate_ShouldFail_WhenNoTagsGiven()
        {
            var command = new UndoStartPreservationCommand(new List<int>());
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("At least 1 tag must be given!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenTagsNotUnique()
        {
            var command = new UndoStartPreservationCommand(new List<int>{1, 1});
            
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
        public void Validate_ShouldFail_WhenProjectForFirstTagIsClosed()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(_tagIds.First(), default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project is closed!"));
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
        public void Validate_ShouldFail_WhenNotStarted()
        {
            _tagValidatorMock.Setup(r => r.IsReadyToUndoStartedAsync(TagId1, default)).Returns(Task.FromResult(false));

            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith($"Undo preservation start on tag can not be done!"));
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

        [TestMethod]
        public void Validate_ShouldFailWith1Error_WhenErrorsInDifferentRules()
        {
            var command = new UndoStartPreservationCommand(new List<int> { 1, 1 });
            _tagValidatorMock.Setup(r => r.ExistsAsync(TagId2, default)).Returns(Task.FromResult(false));
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tags must be unique!"));
        }
    }
}

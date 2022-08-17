using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.DuplicateAreaTag;
using Equinor.ProCoSys.Preservation.Command.Validators.ProjectValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.DuplicateAreaTag
{
    [TestClass]
    public class DuplicateAreaTagCommandValidatorTests
    {
        private const int TagId = 7;
        private DuplicateAreaTagCommandValidator _dut;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<ITagValidator> _tagValidatorMock;
        private DuplicateAreaTagCommand _command;

        [TestInitialize]
        public void Setup_OkState()
        {
            _projectValidatorMock = new Mock<IProjectValidator>();
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.ExistsAsync(TagId, default)).Returns(Task.FromResult(true));
            _tagValidatorMock.Setup(r => r.IsReadyToBeDuplicatedAsync(TagId, default)).Returns(Task.FromResult(true));

            _command = new DuplicateAreaTagCommand(TagId,TagType.SiteArea, "D", "A", "-01", "Desc", "Rem", "SA");

            _dut = new DuplicateAreaTagCommandValidator(_tagValidatorMock.Object, _projectValidatorMock.Object);
        }

        [TestMethod]
        public async Task Validate_ShouldBeValid_WhenOkState()
        {
            var result = await _dut.ValidateAsync(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenSourceTagNotExists()
        {
            _tagValidatorMock.Setup(r => r.ExistsAsync(TagId, default)).Returns(Task.FromResult(false));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Source tag doesn't exist!"));
        }
        
        [TestMethod]
        public async Task Validate_ShouldFail_WhenTargetTagAlreadyExists()
        {
            _tagValidatorMock.Setup(r => r.ExistsAsync(_command.GetTagNo(), TagId, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag already exists in scope for project!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenProjectForTagIsClosed()
        {
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(TagId, default)).Returns(Task.FromResult(true));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project is closed!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFail_WhenNotReadyToBeDuplicated()
        {
            _tagValidatorMock.Setup(r => r.IsReadyToBeDuplicatedAsync(TagId, default)).Returns(Task.FromResult(false));
            
            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Source tag can not be duplicated!"));
        }

        [TestMethod]
        public async Task Validate_ShouldFailWith1Error_WhenMultipleErrorsInSameRule()
        {
            _tagValidatorMock.Setup(r => r.IsReadyToBeDuplicatedAsync(TagId, default)).Returns(Task.FromResult(false));
            _projectValidatorMock.Setup(r => r.IsClosedForTagAsync(TagId, default)).Returns(Task.FromResult(true));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project is closed!"));
        }

        [TestMethod]
        public async Task Validate_ShouldIncludeTagNoInMessage()
        {
            _tagValidatorMock.Setup(r => r.IsReadyToBeDuplicatedAsync(TagId, default)).Returns(Task.FromResult(false));
            var tagNo = "Tag X";
            _tagValidatorMock.Setup(r => r.GetTagDetailsAsync(TagId, default)).Returns(Task.FromResult(tagNo));

            var result = await _dut.ValidateAsync(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains(tagNo));
        }
    }
}

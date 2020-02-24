using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTag;
using Equinor.Procosys.Preservation.Command.Validators.ProjectValidators;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinition;
using Equinor.Procosys.Preservation.Command.Validators.Step;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.CreateTag
{
    [TestClass]
    public class CreateTagCommandValidatorTests
    {
        private CreateTagCommandValidator _dut;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IStepValidator> _stepValidatorMock;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<IRequirementDefinitionValidator> _rdValidatorMock;
        private CreateTagCommand _command;

        private string _tagNo1 = "Tag1";
        private string _tagNo2 = "Tag2";
        private string _projectName = "Project";
        private int _stepId = 1;
        private int _rd1Id = 2;
        private int _rd2Id = 3;

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagValidatorMock = new Mock<ITagValidator>();

            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.Exists(_stepId)).Returns(true);

            _projectValidatorMock = new Mock<IProjectValidator>();

            _rdValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _rdValidatorMock.Setup(r => r.Exists(_rd1Id)).Returns(true);
            _rdValidatorMock.Setup(r => r.Exists(_rd2Id)).Returns(true);

            _command = new CreateTagCommand(
                new List<string>{_tagNo1, _tagNo2}, 
                _projectName,
                _stepId,
                new List<Requirement>
                {
                    new Requirement(_rd1Id, 1),
                    new Requirement(_rd2Id, 1)
                },
                null);

            _dut = new CreateTagCommandValidator(
                _tagValidatorMock.Object, 
                _stepValidatorMock.Object, 
                _projectValidatorMock.Object,
                _rdValidatorMock.Object);
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenOkState()
        {
            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyTagAlreadyExists()
        {
            _tagValidatorMock.Setup(r => r.Exists(_tagNo2, _projectName)).Returns(true);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag already exists in scope for project!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenProjectExistsButClosed()
        {
            _projectValidatorMock.Setup(r => r.IsExistingAndClosedAsync(_projectName, default)).Returns(Task.FromResult(true));
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Project is closed!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepNotExists()
        {
            _stepValidatorMock.Setup(r => r.Exists(_stepId)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step doesn't exists!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenStepIsVoided()
        {
            _stepValidatorMock.Setup(r => r.IsVoided(_stepId)).Returns(true);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Step is voided!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyRequirementDefinitionNotExists()
        {
            _rdValidatorMock.Setup(r => r.Exists(_rd2Id)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition doesn't exists!"));
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains(_rd2Id.ToString()));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenAnyRequirementDefinitionIsVoided()
        {
            _rdValidatorMock.Setup(r => r.IsVoided(_rd2Id)).Returns(true);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definition is voided!"));
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains(_rd2Id.ToString()));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenNoRequirementsGiven()
        {
            var command = new CreateTagCommand(
                new List<string>{_tagNo1}, 
                _projectName,
                _stepId,
                new List<Requirement>(),
                null);
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("At least 1 requirement must be given!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenNoTagNosGiven()
        {
            var command = new CreateTagCommand(
                new List<string>(), 
                _projectName,
                _stepId,
                new List<Requirement>{new Requirement(_rd1Id, 1)},
                null);
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("At least 1 TagNo must be given!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenNoTagNosNotUnique()
        {
            var command = new CreateTagCommand(
                new List<string>{"X","x"}, 
                _projectName,
                _stepId,
                new List<Requirement>{new Requirement(_rd1Id, 1)},
                null);
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("TagNos must be unique!"));
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenRequirementsNotUnique()
        {
            var command = new CreateTagCommand(
                new List<string>{_tagNo1}, 
                _projectName,
                _stepId,
                new List<Requirement>
                {
                    new Requirement(_rd1Id, 1),
                    new Requirement(_rd1Id, 1)
                },
                null);
            
            var result = _dut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Requirement definitions must be unique!"));
        }

        [TestMethod]
        public void Validate_ShouldFailWith1Error_When2ErrorsWithinSameRule()
        {
            _stepValidatorMock.Setup(r => r.Exists(_stepId)).Returns(false);
            _stepValidatorMock.Setup(r => r.IsVoided(_stepId)).Returns(true);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [TestMethod]
        public void Validate_ShouldFailWith2Errors_WhenErrorsInDifferentRules()
        {
            _tagValidatorMock.Setup(r => r.Exists(_tagNo1, _projectName)).Returns(true);
            _rdValidatorMock.Setup(r => r.Exists(_rd2Id)).Returns(false);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Errors.Count);
        }
    }
}

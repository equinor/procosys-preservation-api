using System.Collections.Generic;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTag;
using Equinor.Procosys.Preservation.Command.Validators.Project;
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

        private string _tagNo = "Tag";
        private string _projectName = "Project";
        private int _stepId = 1;
        private int _rd1Id = 2;
        private int _rd2Id = 3;

        [TestInitialize]
        public void Setup_OkState()
        {
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.Exists(_tagNo, _projectName)).Returns(false);

            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.Exists(_stepId)).Returns(true);

            _projectValidatorMock = new Mock<IProjectValidator>();
            _projectValidatorMock.Setup(r => r.Exists(_projectName)).Returns(true);
            _projectValidatorMock.Setup(r => r.IsClosed(_projectName)).Returns(false);

            _rdValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _rdValidatorMock.Setup(r => r.Exists(_rd1Id)).Returns(true);
            _rdValidatorMock.Setup(r => r.Exists(_rd2Id)).Returns(true);

            _command = new CreateTagCommand(
                _tagNo,
                _projectName,
                _stepId,
                new List<Requirement>
                {
                    new Requirement(_rd1Id, 1),
                    new Requirement(_rd2Id, 1)
                });

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
        public void Validate_ShouldFail_WhenTagAlreadyExists()
        {
            _tagValidatorMock.Setup(r => r.Exists(_tagNo, _projectName)).Returns(true);
            
            var result = _dut.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag already exists in scope for project!"));
        }

        [TestMethod]
        public void Validate_ShouldBeValid_WhenProjectExistsAndProjectNotClosed()
        {
            _projectValidatorMock.Setup(r => r.Exists(_projectName)).Returns(true);

            var result = _dut.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void Validate_ShouldFail_WhenProjectExistsButClosed()
        {
            _projectValidatorMock.Setup(r => r.Exists(_projectName)).Returns(true);
            _projectValidatorMock.Setup(r => r.IsClosed(_projectName)).Returns(true);
            
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
    }
}

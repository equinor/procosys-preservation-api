using System.Collections.Generic;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateTag;
using Equinor.Procosys.Preservation.Command.Validators.Project;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinition;
using Equinor.Procosys.Preservation.Command.Validators.Step;
using Equinor.Procosys.Preservation.Command.Validators.Tag;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands
{
    [TestClass]
    public class CreateTagCommandValidatorTests
    {
        private CreateTagCommandValidator _validator;
        private Mock<ITagValidator> _tagValidatorMock;
        private Mock<IStepValidator> _stepValidatorMock;
        private Mock<IProjectValidator> _projectValidatorMock;
        private Mock<IRequirementDefinitionValidator> _rdValidatorMock;
        private CreateTagCommand _command;

        private string _tagNo = "Tag";
        private string _projectNo = "Project";
        private int _stepId = 1;
        private int _rd1Id = 2;
        private int _rd2Id = 3;

        [TestInitialize]
        public void Setup()
        {
            _tagValidatorMock = new Mock<ITagValidator>();
            _tagValidatorMock.Setup(r => r.Exists(_tagNo, _projectNo)).Returns(false);

            _stepValidatorMock = new Mock<IStepValidator>();
            _stepValidatorMock.Setup(r => r.Exists(_stepId)).Returns(true);

            _projectValidatorMock = new Mock<IProjectValidator>();
            _projectValidatorMock.Setup(r => r.Exists(_projectNo)).Returns(true);
            _projectValidatorMock.Setup(r => r.IsClosed(_projectNo)).Returns(false);

            _rdValidatorMock = new Mock<IRequirementDefinitionValidator>();
            _rdValidatorMock.Setup(r => r.Exists(_rd1Id)).Returns(true);
            _rdValidatorMock.Setup(r => r.Exists(_rd2Id)).Returns(true);

            _validator = new CreateTagCommandValidator(
                _tagValidatorMock.Object, 
                _stepValidatorMock.Object, 
                _projectValidatorMock.Object,
                _rdValidatorMock.Object);

            _command = new CreateTagCommand(
                _tagNo,
                _projectNo,
                _stepId,
                new List<RequirementDto>
                {
                    new RequirementDto(_rd1Id, 1),
                    new RequirementDto(_rd2Id, 1)
                });
        }

        [TestMethod]
        public void WhenValidate_ShouldBeOk_WhenOkState()
        {
            var result = _validator.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void WhenValidate_ShouldFail_WhenTagAlreadyExists()
        {
            _tagValidatorMock.Setup(r => r.Exists(_tagNo, _projectNo)).Returns(true);
            var result = _validator.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Tag already exists in scope for project!"));
        }

        [TestMethod]
        public void WhenValidate_ShouldBeOk_WhenProjectExists_AndProjectNotClosed()
        {
            _projectValidatorMock.Setup(r => r.Exists(_projectNo)).Returns(true);
            var result = _validator.Validate(_command);

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void WhenValidate_ShouldFail_WhenProjectExists_ButClosed()
        {
            _projectValidatorMock.Setup(r => r.Exists(_projectNo)).Returns(true);
            _projectValidatorMock.Setup(r => r.IsClosed(_projectNo)).Returns(true);
            var result = _validator.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains("Project is closed!"));
        }

        [TestMethod]
        public void WhenValidate_ShouldFail_WhenStepNotExists()
        {
            _stepValidatorMock.Setup(r => r.Exists(_stepId)).Returns(false);
            var result = _validator.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains("Step don't exists!"));
        }

        [TestMethod]
        public void WhenValidate_ShouldFail_WhenAnyRequirementDefinitionNotExists()
        {
            _rdValidatorMock.Setup(r => r.Exists(_rd2Id)).Returns(false);
            var result = _validator.Validate(_command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains("Requirement definition don't exists!"));
            Assert.IsTrue(result.Errors[0].ErrorMessage.Contains(_rd2Id.ToString()));
        }
    }
}

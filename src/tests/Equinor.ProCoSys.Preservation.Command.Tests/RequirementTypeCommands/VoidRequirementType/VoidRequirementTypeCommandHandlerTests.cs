using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.VoidRequirementType;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.RequirementTypeCommands.VoidRequirementType
{
    [TestClass]
    public class VoidRequirementTypeCommandHandlerTests : CommandHandlerTestsBase
    {
        private RequirementType _requirementType;
        private VoidRequirementTypeCommand _command;
        private VoidRequirementTypeCommandHandler _dut;
        private readonly string _rowVersion = "AAAAAAAAABA=";
        RequirementTypeIcon _reqIconOther = RequirementTypeIcon.Other;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var requirementTypeId = 1;
            var reqTypeRepositoryMock = new Mock<IRequirementTypeRepository>();

            _requirementType = new RequirementType(TestPlant, "TestCode", "RequirementTypeTitle", _reqIconOther, 10);
            _requirementType.SetProtectedIdForTesting(requirementTypeId);

            reqTypeRepositoryMock.Setup(m => m.GetByIdAsync(requirementTypeId))
                .Returns(Task.FromResult(_requirementType));

            _command = new VoidRequirementTypeCommand(requirementTypeId, _rowVersion);

            _dut = new VoidRequirementTypeCommandHandler(
                reqTypeRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingVoidRequirementTypeCommand_ShouldVoidRequirementType()
        {
            // Arrange
            Assert.IsFalse(_requirementType.IsVoided);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsTrue(_requirementType.IsVoided);
        }

        [TestMethod]
        public async Task HandlingVoidRequirementTypeCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _requirementType.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingVoidRequirementTypeCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}

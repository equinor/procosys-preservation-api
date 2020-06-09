using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.VoidRequirementDefinition;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.VoidRequirementDefinition
{
    [TestClass]
    public class VoidRequirementDefinitionCommandHandlerTests : CommandHandlerTestsBase
    {
        private RequirementDefinition _requirementDefinition;
        private VoidRequirementDefinitionCommand _command;
        private VoidRequirementDefinitionCommandHandler _dut;
        private readonly string _rowVersion = "AAAAAAAAABA=";

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var reqTypeRepositoryMock = new Mock<IRequirementTypeRepository>();
            var requirementTypeId = 1;
            var requirementDefinitionId = 2;
            var requirementType = new RequirementType(TestPlant, "TestCode", "ReqTypeTitle", 99);
            requirementType.SetProtectedIdForTesting(requirementTypeId);

            _requirementDefinition = new RequirementDefinition(TestPlant, "ReqDefinitionTitle", 4, RequirementUsage.ForAll, 88);
            _requirementDefinition.SetProtectedIdForTesting(requirementDefinitionId);
            requirementType.AddRequirementDefinition(_requirementDefinition);

            reqTypeRepositoryMock.Setup(m => m.GetByIdAsync(requirementTypeId))
                .Returns(Task.FromResult(requirementType));

            _command = new VoidRequirementDefinitionCommand(requirementTypeId, requirementDefinitionId, _rowVersion, TestUserOid);
            _dut = new VoidRequirementDefinitionCommandHandler(
                reqTypeRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingVoidRequirementDefinitionCommand_ShouldVoidRequirementDefinition()
        {
            // Arrange
            Assert.IsFalse(_requirementDefinition.IsVoided);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsTrue(_requirementDefinition.IsVoided);
        }

        [TestMethod]
        public async Task HandlingVoidRequirementDefinitionCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _requirementDefinition.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingVoidRequirementDefinitionCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(_command.CurrentUserOid, default), Times.Once);
        }
    }
}

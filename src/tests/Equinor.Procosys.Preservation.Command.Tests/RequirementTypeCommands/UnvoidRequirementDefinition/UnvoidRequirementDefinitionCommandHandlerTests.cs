using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementDefinition;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.UnvoidRequirementDefinition
{
    [TestClass]
    public class UnvoidRequirementDefinitionCommandHandlerTests : CommandHandlerTestsBase
    {
        private RequirementDefinition _requirementDefinition;
        private UnvoidRequirementDefinitionCommand _command;
        private UnvoidRequirementDefinitionCommandHandler _dut;
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

            _requirementDefinition = new RequirementDefinition(TestPlant, "ReqDefinitionTitle", 4, 88);
            _requirementDefinition.SetProtectedIdForTesting(requirementDefinitionId);
            _requirementDefinition.Void();

            requirementType.AddRequirementDefinition(_requirementDefinition);

            reqTypeRepositoryMock.Setup(m => m.GetByIdAsync(requirementTypeId))
                .Returns(Task.FromResult(requirementType));

            _command = new UnvoidRequirementDefinitionCommand(requirementTypeId, requirementDefinitionId, _rowVersion);

            _dut = new UnvoidRequirementDefinitionCommandHandler(
                reqTypeRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUnvoidRequirementDefinitionCommand_ShouldUnvoidRequirementDefinition()
        {
            // Arrange
            Assert.IsTrue(_requirementDefinition.IsVoided);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsFalse(_requirementDefinition.IsVoided);
        }

        [TestMethod]
        public async Task HandlingUnvoidRequirementDefinitionCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _requirementDefinition.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingUnvoidRequirementDefinitionCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}

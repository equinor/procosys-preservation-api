using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.UpdateRequirementDefinition;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.UpdateRequirementDefinition
{
    [TestClass]
    public class UpdateRequirementDefinitionCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly string _oldTitle = "TitleOld";
        private readonly string _newTitle = "TitleNew";
        private readonly int _oldSortKey = 1;
        private readonly int _newSortKey = 2;
        private readonly RequirementUsage _oldUsage = RequirementUsage.ForAll;
        private readonly RequirementUsage _newUsage = RequirementUsage.ForOtherThanSuppliers;
        private readonly int _oldDefaultWeeks = 4;
        private readonly int _newDefaultWeeks = 2;
        private readonly string _rowVersion = "AAAAAAAAABA=";

        private UpdateRequirementDefinitionCommand _command;
        private UpdateRequirementDefinitionCommandHandler _dut;
        private RequirementType _requirementType;
        private RequirementDefinition _requirementDefinition;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var requirementTypeId = 1;
            var requirementDefinitionId = 2;

            var requirementTypeRepositoryMock = new Mock<IRequirementTypeRepository>();

            _requirementType = new RequirementType(TestPlant, "Req type code", "Req type title", RequirementTypeIcon.Other, 10);
            _requirementType.SetProtectedIdForTesting(requirementTypeId);

            _requirementDefinition = new RequirementDefinition(TestPlant, _oldTitle, _oldDefaultWeeks, _oldUsage, _oldSortKey);
            _requirementDefinition.SetProtectedIdForTesting(requirementDefinitionId);
            _requirementType.AddRequirementDefinition(_requirementDefinition);

            requirementTypeRepositoryMock.Setup(j => j.GetByIdAsync(requirementTypeId))
                .Returns(Task.FromResult(_requirementType));

            _command = new UpdateRequirementDefinitionCommand(
                requirementTypeId,
                requirementDefinitionId,
                _newSortKey,
                _newUsage,
                _newTitle,
                _newDefaultWeeks,
                _rowVersion,
                new List<UpdateFieldsForCommand>(),
                new List<FieldsForCommand>()
                );

            _dut = new UpdateRequirementDefinitionCommandHandler(
                requirementTypeRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object);
        }

        [TestMethod]
        public async Task HandlingUpdateRequirementDefinitionCommand_ShouldUpdateRequirementDefinition()
        {
            // Arrange
            Assert.AreEqual(_oldTitle, _requirementDefinition.Title);
            Assert.AreEqual(_oldSortKey, _requirementDefinition.SortKey);
            Assert.AreEqual(_oldUsage, _requirementDefinition.Usage);
            Assert.AreEqual(_oldDefaultWeeks, _requirementDefinition.DefaultIntervalWeeks);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_newTitle, _requirementDefinition.Title);
            Assert.AreEqual(_newSortKey, _requirementDefinition.SortKey);
            Assert.AreEqual(_newUsage, _requirementDefinition.Usage);
            Assert.AreEqual(_newDefaultWeeks, _requirementDefinition.DefaultIntervalWeeks);
        }

        [TestMethod]
        public async Task HandlingUpdateRequirementDefinitionCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _requirementDefinition.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingUpdateRequirementDefinitionCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        // todo test updating and voiding of fields
    }
}

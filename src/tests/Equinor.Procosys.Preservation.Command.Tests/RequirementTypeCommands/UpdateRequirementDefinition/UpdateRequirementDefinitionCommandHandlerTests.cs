using System.Collections.Generic;
using System.Linq;
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
        private readonly int _newSortKey1 = 2;
        private readonly int _newSortKey2 = 2;
        private readonly RequirementUsage _oldUsage = RequirementUsage.ForAll;
        private readonly RequirementUsage _newUsage = RequirementUsage.ForOtherThanSuppliers;
        private readonly int _oldDefaultWeeks = 4;
        private readonly int _newDefaultWeeks = 2;
        private readonly string _rowVersion = "AAAAAAAAABA=";

        private UpdateRequirementDefinitionCommand _command;
        private UpdateRequirementDefinitionCommandHandler _dut;
        private RequirementType _requirementType;
        private RequirementDefinition _requirementDefinition;
        private readonly string _oldLabel = "OldLabel";
        private readonly string _newLabel1 = "NewLabel1";
        private readonly string _newLabel2 = "NewLabel2";
        private readonly string _oldUnit = "OldUnit";
        private readonly string _newUnit1 = "NewUnit1";
        private readonly string _newUnit2 = "NewUnit2";
        private readonly bool? _oldShowPrevious = false;
        private readonly bool? _newShowPrevious1 = true;
        private readonly bool? _newShowPrevious2 = false;
        private readonly FieldType _oldFieldType = FieldType.Number;
        private readonly FieldType _newFieldType2 = FieldType.Attachment;
        private Field _field;
        private int _fieldId;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var requirementTypeId = 1;
            var requirementDefinitionId = 2;
            _fieldId = 3;

            var requirementTypeRepositoryMock = new Mock<IRequirementTypeRepository>();

            _requirementType = new RequirementType(TestPlant, "Req type code", "Req type title", RequirementTypeIcon.Other, 10);
            _requirementType.SetProtectedIdForTesting(requirementTypeId);

            _requirementDefinition = new RequirementDefinition(TestPlant, _oldTitle, _oldDefaultWeeks, _oldUsage, _oldSortKey);
            _requirementDefinition.SetProtectedIdForTesting(requirementDefinitionId);

            _field = new Field(TestPlant, _oldLabel, _oldFieldType, _oldSortKey, _oldUnit, _oldShowPrevious);
            _field.SetProtectedIdForTesting(_fieldId);

            _requirementDefinition.AddField(_field);
            _requirementType.AddRequirementDefinition(_requirementDefinition);

            requirementTypeRepositoryMock.Setup(j => j.GetByIdAsync(requirementTypeId))
                .Returns(Task.FromResult(_requirementType));

            _command = new UpdateRequirementDefinitionCommand(
                requirementTypeId,
                requirementDefinitionId,
                _newSortKey1,
                _newUsage,
                _newTitle,
                _newDefaultWeeks,
                _rowVersion,
                new List<UpdateFieldsForCommand>
                {
                    new UpdateFieldsForCommand(_fieldId, _newLabel1, FieldType.Number, _newSortKey1, true, _rowVersion, _newUnit1, _newShowPrevious1)
                },
                new List<FieldsForCommand>
                {
                    new FieldsForCommand(_newLabel2, _newFieldType2, _newSortKey2, _newUnit2, _newShowPrevious2)
                }
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
            Assert.AreEqual(_newSortKey1, _requirementDefinition.SortKey);
            Assert.AreEqual(_newUsage, _requirementDefinition.Usage);
            Assert.AreEqual(_newDefaultWeeks, _requirementDefinition.DefaultIntervalWeeks);
        }

        [TestMethod]
        public async Task HandlingUpdateRequirementDefinitionCommand_ShouldUpdateFieldOnRequirementDefinition()
        {
            // Arrange
            Assert.AreEqual(1, _requirementDefinition.Fields.Count);
            var field = _requirementDefinition.Fields.Single(f => f.Id == _fieldId);
            Assert.AreEqual(_oldLabel, field.Label);
            Assert.AreEqual(_oldFieldType, field.FieldType); // FieldType can't be changed
            Assert.AreEqual(_oldSortKey, field.SortKey);
            Assert.AreEqual(_oldUnit, field.Unit);
            Assert.AreEqual(_oldShowPrevious, field.ShowPrevious);
            Assert.IsFalse(field.IsVoided);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(2, _requirementDefinition.Fields.Count);

            field = _requirementDefinition.Fields.Single(f => f.Id == _fieldId);
            Assert.AreEqual(_newLabel1, field.Label);
            Assert.AreEqual(_oldFieldType, field.FieldType); // FieldType can't be changed
            Assert.AreEqual(_newSortKey1, field.SortKey);
            Assert.AreEqual(_newUnit1, field.Unit);
            Assert.AreEqual(_newShowPrevious1, field.ShowPrevious);
            Assert.IsTrue(field.IsVoided);
        }

        [TestMethod]
        public async Task HandlingUpdateRequirementDefinitionCommand_ShouldAddFieldToRequirementDefinition()
        {
            // Arrange
            Assert.AreEqual(1, _requirementDefinition.Fields.Count);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(2, _requirementDefinition.Fields.Count);

            var field = _requirementDefinition.Fields.Single(f => f.Id != _fieldId);
            Assert.AreEqual(_newLabel2, field.Label);
            Assert.AreEqual(_newFieldType2, field.FieldType); // FieldType can't be changed
            Assert.AreEqual(_newSortKey2, field.SortKey);
            Assert.AreEqual(_newUnit2, field.Unit);
            Assert.AreEqual(_newShowPrevious2, field.ShowPrevious);
            Assert.IsFalse(field.IsVoided);
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
    }
}

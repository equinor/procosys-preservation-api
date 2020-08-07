using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.CreateRequirementDefinition;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.CreateRequirementDefinition
{
    [TestClass]
    public class CreateRequirementDefinitionCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string RequirementDefinitionTitle = "TestRequirementDefinition Title";
        private RequirementUsage Usage = RequirementUsage.ForAll;
        private const int DefaultWeeks = 4;
        private Mock<IRequirementTypeRepository> _reqTypeRepositoryMock;
        private RequirementType _reqTypeAdded;
        private CreateRequirementDefinitionCommand _command;
        private CreateRequirementDefinitionCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _reqTypeRepositoryMock = new Mock<IRequirementTypeRepository>();
            _reqTypeAdded = new RequirementType(TestPlant, "Code", "Title", RequirementTypeIcon.Other, 10);
            _reqTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .Returns(Task.FromResult(_reqTypeAdded));

            _command = new CreateRequirementDefinitionCommand(1, 10, Usage, RequirementDefinitionTitle, DefaultWeeks);

            _dut = new CreateRequirementDefinitionCommandHandler(
                _reqTypeRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object
            );
        }

        [TestMethod]
        public async Task HandlingCreateReqDefinitionCommand_ShouldAddReqDefinitionToRepositoryWithoutField()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            var reqDef = _reqTypeAdded.RequirementDefinitions.First();
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.AreEqual(RequirementDefinitionTitle, reqDef.Title);
            Assert.AreEqual(DefaultWeeks, reqDef.DefaultIntervalWeeks);
            Assert.AreEqual(Usage, reqDef.Usage);
            Assert.AreEqual(0, reqDef.Fields.Count);
        }

        [TestMethod]
        public async Task HandlingCreateReqDefinitionCommand_ShouldAddReqDefinitionToRepositoryWithFields()
        {
            // Arrange
            _command = new CreateRequirementDefinitionCommand(1, 10, Usage, RequirementDefinitionTitle, DefaultWeeks, 
                new List<FieldsForCommand>
                {
                    new FieldsForCommand("Label", FieldType.CheckBox, 99, "U", true)
                });

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            var reqDef = _reqTypeAdded.RequirementDefinitions.First();
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(1, reqDef.Fields.Count);

            var field = reqDef.Fields.Single();
            Assert.AreEqual("Label", field.Label);
            Assert.AreEqual(FieldType.CheckBox, field.FieldType);
            Assert.AreEqual(99, field.SortKey);
            Assert.AreEqual("U", field.Unit);
            Assert.IsTrue(field.ShowPrevious.HasValue);
            Assert.IsTrue(field.ShowPrevious.Value);
        }

        [TestMethod]
        public async Task HandlingCreateReqDefinitionCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}

using System.Linq;
using System.Threading.Tasks;
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
        public async Task HandlingCreateReqDefinitionCommand_ShouldAddReqDefinitionToRepository()
        {
            // Act
            var result = await _dut.Handle(_command, default);
            var reqDef = _reqTypeAdded.RequirementDefinitions.First();

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.AreEqual(RequirementDefinitionTitle, reqDef.Title);
            Assert.AreEqual(DefaultWeeks, reqDef.DefaultIntervalWeeks);
            Assert.AreEqual(Usage, reqDef.Usage);
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

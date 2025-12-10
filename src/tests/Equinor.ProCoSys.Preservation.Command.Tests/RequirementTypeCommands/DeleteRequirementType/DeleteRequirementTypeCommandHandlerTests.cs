using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.DeleteRequirementType;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.RequirementTypeCommands.DeleteRequirementType
{
    [TestClass]
    public class DeleteRequirementTypeCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int RequirementTypeId = 12;
        private const string RowVersion = "AAAAAAAAABA=";
        private Mock<IRequirementTypeRepository> _requirementTypeRepositoryMock;
        private RequirementType _requirementType;
        private DeleteRequirementTypeCommand _command;
        private DeleteRequirementTypeCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _requirementTypeRepositoryMock = new Mock<IRequirementTypeRepository>();
            _requirementType = new RequirementType(TestPlant, "Code", "Title", RequirementTypeIcon.Other, 10);
            _requirementTypeRepositoryMock
                .Setup(x => x.GetByIdAsync(RequirementTypeId))
                    .Returns(Task.FromResult(_requirementType));
            _command = new DeleteRequirementTypeCommand(RequirementTypeId, RowVersion);

            _dut = new DeleteRequirementTypeCommandHandler(
                _requirementTypeRepositoryMock.Object,
                UnitOfWorkMock.Object
            );
        }

        [TestMethod]
        public async Task HandlingDeleteRequirementTypeCommand_ShouldDeleteRequirementTypeFromRepository()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            _requirementTypeRepositoryMock.Verify(r => r.Remove(_requirementType), Times.Once);
        }

        [TestMethod]
        public async Task HandlingDeleteRequirementTypeCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [TestMethod]
        public async Task HandlingDeleteRequirementTypeCommand_ShouldAddDeletionEvent()
        {
            // Act
            await _dut.Handle(_command, default);
            var eventTypes = _requirementType.DomainEvents.Select(e => e.GetType()).ToList();

            // Assert
            CollectionAssert.Contains(eventTypes, typeof(DeletedEvent<RequirementType>));
        }
    }
}

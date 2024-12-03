using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.UpdateRequirementType;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.RequirementTypeCommands.UpdateRequirementType
{
    [TestClass]
    public class UpdateRequirementTypeCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly string _oldTitle = "TitleOld";
        private readonly string _newTitle = "TitleNew";
        private readonly string _oldCode = "CodeOld";
        private readonly string _newCode = "CodeNew";
        private readonly RequirementTypeIcon _oldIcon = RequirementTypeIcon.Other;
        private readonly RequirementTypeIcon _newIcon = RequirementTypeIcon.Area;
        private readonly int _oldSortKey = 1;
        private readonly int _newSortKey = 2;
        private readonly string _rowVersion = "AAAAAAAAABA=";

        private UpdateRequirementTypeCommand _command;
        private UpdateRequirementTypeCommandHandler _dut;
        private RequirementType _requirementType;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var testRequirementTypeId = 1;
            var requirementTypeRepositoryMock = new Mock<IRequirementTypeRepository>();
            _requirementType = new RequirementType(TestPlant, _oldCode, _oldTitle, _oldIcon, _oldSortKey);
            requirementTypeRepositoryMock.Setup(j => j.GetByIdAsync(testRequirementTypeId))
                .Returns(Task.FromResult(_requirementType));
            _command = new UpdateRequirementTypeCommand(testRequirementTypeId, _rowVersion, _newSortKey ,_newTitle, _newCode, _newIcon);

            _dut = new UpdateRequirementTypeCommandHandler(
                requirementTypeRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUpdateRequirementTypeCommand_ShouldUpdateRequirementType()
        {
            // Arrange
            Assert.AreEqual(_oldTitle, _requirementType.Title);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_newTitle, _requirementType.Title);
        }

        [TestMethod]
        public async Task HandlingUpdateRequirementTypeCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _requirementType.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingUpdateRequirementTypeCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}

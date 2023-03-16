using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.UnvoidRequirementType;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.RequirementTypeCommands.UnvoidRequirementType
{
    [TestClass]
    public class UnvoidRequirementTypeCommandHandlerTests : CommandHandlerTestsBase
    {
        private RequirementType _requirementType;
        private UnvoidRequirementTypeCommand _command;
        private UnvoidRequirementTypeCommandHandler _dut;
        private readonly string _rowVersion = "AAAAAAAAABA=";

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var requirementTypeId = 1;
            var reqTypeRepositoryMock = new Mock<IRequirementTypeRepository>();

            _requirementType = new RequirementType(TestPlant, "TestCode", "RequirementTypeTitle", RequirementTypeIcon.Other, 10);
            _requirementType.IsVoided = true;

            reqTypeRepositoryMock
                .Setup(r => r.GetByIdAsync(requirementTypeId))
                .Returns(Task.FromResult(_requirementType));

            _command = new UnvoidRequirementTypeCommand(requirementTypeId, _rowVersion);

            _dut = new UnvoidRequirementTypeCommandHandler(
                reqTypeRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUnvoidRequirementTypeCommand_ShouldUnvoidRequirementType()
        {
            // Arrange
            Assert.IsTrue(_requirementType.IsVoided);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsFalse(_requirementType.IsVoided);
        }

        [TestMethod]
        public async Task HandlingUnvoidRequirementTypeCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _requirementType.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingUnvoidRequirementTypeCommand_ShouldSave()
        {
            await _dut.Handle(_command, default);

            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}

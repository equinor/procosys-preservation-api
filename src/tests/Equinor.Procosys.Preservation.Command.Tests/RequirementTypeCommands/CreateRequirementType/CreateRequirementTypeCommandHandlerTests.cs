using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.CreateRequirementType;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.CreateRequirementType
{
    [TestClass]
    public class CreateRequirementTypeCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string TestRequirementTypeTitle = "TestRequirementType Title";
        private const string TestRequirementTypeCode = "TestRequirementType Code";
        private Mock<IRequirementTypeRepository> _reqTypeRepositoryMock;
        private RequirementType _reqTypeAdded;
        private CreateRequirementTypeCommand _command;
        private CreateRequirementTypeCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _reqTypeRepositoryMock = new Mock<IRequirementTypeRepository>();
            _reqTypeRepositoryMock
                .Setup(x => x.Add(It.IsAny<RequirementType>()))
                .Callback<RequirementType>(x =>
                {
                    _reqTypeAdded = x;
                });

            _command = new CreateRequirementTypeCommand(10, TestRequirementTypeCode, TestRequirementTypeTitle, RequirementTypeIcon.Other);

            _dut = new CreateRequirementTypeCommandHandler(
                _reqTypeRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object
            );
        }

        [TestMethod]
        public async Task HandlingCreateReqTypeCommand_ShouldAddReqTypeToRepository()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Data);
            Assert.AreEqual(0, _reqTypeAdded.Id);
            Assert.AreEqual(TestRequirementTypeTitle, _reqTypeAdded.Title);
            Assert.AreEqual(TestRequirementTypeCode, _reqTypeAdded.Code);
        }

        [TestMethod]
        public async Task HandlingCreateReqTypeCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}

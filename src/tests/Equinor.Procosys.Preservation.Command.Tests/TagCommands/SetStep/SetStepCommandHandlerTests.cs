using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.TagCommands.SetStep;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.SetStep
{
    [TestClass]
    public class SetStepCommandHandlerTests : CommandHandlerTestsBase
    {
        private const int StepId = 11;
        private const int TagId = 99;

        private Mock<Tag> _tagMock;
        private Mock<Step> _stepMock;
        private Mock<IJourneyRepository> _journeyRepositoryMock;
        private Mock<ITagRepository> _tagRepositoryMock;
        private SetStepCommand _command;
        private SetStepCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _tagMock = new Mock<Tag>();
            _tagMock.SetupGet(x => x.Id).Returns(TagId);
            _stepMock = new Mock<Step>();
            _stepMock.SetupGet(x => x.Id).Returns(StepId);

            _tagRepositoryMock = new Mock<ITagRepository>();
            _tagRepositoryMock
                .Setup(x => x.GetByIdAsync(TagId))
                .Returns(Task.FromResult(_tagMock.Object));

            _journeyRepositoryMock = new Mock<IJourneyRepository>();
            _journeyRepositoryMock
                .Setup(x => x.GetStepByStepIdAsync(StepId))
                .Returns(Task.FromResult(_stepMock.Object));

            _command = new SetStepCommand(TagId, StepId);
            
            _dut = new SetStepCommandHandler(
                _tagRepositoryMock.Object,
                _journeyRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingSetStepCommand_ShouldAddTagToRepository()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            _tagMock.Verify(t => t.SetStep(_stepMock.Object), Times.Once);
        }

        [TestMethod]
        public async Task HandlingSetStepCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}

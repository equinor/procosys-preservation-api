using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.MiscCommands.Clone;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.MiscCommands.Clone
{
    [TestClass]
    public class CloneCommandHandlerTests : CommandHandlerTestsBase
    {
        private readonly string _sourcePlant = "SOURCE";
        private CloneCommand _command;
        private CloneCommandHandler _dut;
        private readonly PlantProvider _plantProvider = new PlantProvider(TestPlant);
        private ModeRepository _modeRepository;
        private readonly List<Mode> _sourceModes = new List<Mode>();

        [TestInitialize]
        public void Setup()
        {
            _modeRepository = new ModeRepository(_plantProvider, _sourceModes);
            _sourceModes.Add(new Mode(TestPlant, "ModeA"));

            var responsibleRepositoryMock = new Mock<IResponsibleRepository>();
            responsibleRepositoryMock
                .Setup(r => r.GetAllAsync())
                .Returns(Task.FromResult(new List<Responsible>()));
            
            var requirementTypeRepositoryMock = new Mock<IRequirementTypeRepository>();
            requirementTypeRepositoryMock
                .Setup(r => r.GetAllAsync())
                .Returns(Task.FromResult(new List<RequirementType>()));

            var tagFunctionRepositoryMock = new Mock<ITagFunctionRepository>();
            tagFunctionRepositoryMock
                .Setup(r => r.GetAllAsync())
                .Returns(Task.FromResult(new List<TagFunction>()));

            _command = new CloneCommand(_sourcePlant, TestPlant);
            _dut = new CloneCommandHandler(
                _plantProvider,
                UnitOfWorkMock.Object,
                _modeRepository,
                responsibleRepositoryMock.Object,
                requirementTypeRepositoryMock.Object,
                tagFunctionRepositoryMock.Object);
        }

        [TestMethod]
        public async Task HandlingCloneCommand_ShouldCloneModes()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            var modeAdded = _modeRepository.GetAllAsync().Result.Single();
            Assert.IsNotNull(modeAdded);
            Assert.AreEqual(TestPlant, modeAdded.Plant);
            Assert.AreEqual(_sourceModes.First().Title, modeAdded.Title);
        }

        [TestMethod]
        public async Task HandlingCloneCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}

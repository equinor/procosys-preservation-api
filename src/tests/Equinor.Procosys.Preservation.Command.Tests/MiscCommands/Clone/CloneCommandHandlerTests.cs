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
        private ResponsibleRepository _responsibleRepository;
        private readonly List<Responsible> _sourceResponsibles = new List<Responsible>();

        [TestInitialize]
        public void Setup()
        {
            _modeRepository = new ModeRepository(_plantProvider, _sourceModes);
            _sourceModes.Add(new Mode(TestPlant, "ModeA"));
            _sourceModes.Add(new Mode(TestPlant, "ModeB"));

            _responsibleRepository = new ResponsibleRepository(_plantProvider, _sourceResponsibles);
            _sourceResponsibles.Add(new Responsible(TestPlant, "ResponsibleCodeA", "ResponsibleTitleA"));
            _sourceResponsibles.Add(new Responsible(TestPlant, "ResponsibleCodeB", "ResponsibleTitleB"));
            
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
                _responsibleRepository,
                requirementTypeRepositoryMock.Object,
                tagFunctionRepositoryMock.Object);
        }

        [TestMethod]
        public async Task HandlingCloneCommand_ShouldCloneModes()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            AssertClonedModes(_sourceModes, _modeRepository.GetAllAsync().Result);
        }

        [TestMethod]
        public async Task HandlingCloneCommand_ShouldCloneResposibles()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            AssertClonedResponsibles(_sourceResponsibles, _responsibleRepository.GetAllAsync().Result);
        }

        [TestMethod]
        public async Task HandlingCloneCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        private void AssertClonedModes(List<Mode> sourceModes, List<Mode> result)
        {
            Assert.AreEqual(sourceModes.Count, result.Count);
            for (var i = 0; i < sourceModes.Count; i++)
            {
                var source = sourceModes.ElementAt(i);
                var clone = result.ElementAt(i);
                Assert.IsNotNull(clone);
                Assert.AreEqual(TestPlant, clone.Plant);
                Assert.AreEqual(source.Title, clone.Title);
            }
        }

        private void AssertClonedResponsibles(List<Responsible> sourceResponsibles, List<Responsible> result)
        {
            Assert.AreEqual(sourceResponsibles.Count, result.Count);
            for (var i = 0; i < sourceResponsibles.Count; i++)
            {
                var source = sourceResponsibles.ElementAt(i);
                var clone = result.ElementAt(i);
                Assert.IsNotNull(clone);
                Assert.AreEqual(TestPlant, clone.Plant);
                Assert.AreEqual(source.Code, clone.Code);
                Assert.AreEqual(source.Title, clone.Title);
            }
        }
    }
}

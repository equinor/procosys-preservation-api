using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.SyncCommands.SyncResponsibles;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Responsible;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.SyncCommands.SyncResponsibles
{
    [TestClass]
    public class SyncResponsiblesCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string ResponsibleCode1 = "Responsible1";
        private const string ResponsibleCode2 = "Responsible2";
        private const string OldResponsibleDescription1 = "OldResponsible1Description";
        private const string OldResponsibleDescription2 = "OldResponsible2Description";
        private const string NewResponsibleDescription1 = "NewResponsible1Description";
        private const string NewResponsibleDescription2 = "NewResponsible2Description";

        private Mock<IResponsibleRepository> _responsibleRepositoryMock;
        private Mock<IResponsibleApiService> _responsibleApiServiceMock;

        private SyncResponsiblesCommand _command;
        private SyncResponsiblesCommandHandler _dut;
        private Responsible _responsible1;
        private Responsible _responsible2;
        private PCSResponsible _mainResponsible1;
        private PCSResponsible _mainResponsible2;

        [TestInitialize]
        public void Setup()
        {
            // Assert responsibles in preservation
            _responsible1 = new Responsible(TestPlant, ResponsibleCode1, OldResponsibleDescription1);
            _responsible2 = new Responsible(TestPlant, ResponsibleCode2, OldResponsibleDescription2);

            _responsibleRepositoryMock = new Mock<IResponsibleRepository>();
            _responsibleRepositoryMock
                .Setup(p => p.GetAllAsync())
                .Returns(Task.FromResult(new List<Responsible> {_responsible1, _responsible2}));

            // Assert responsibles in main
            _mainResponsible1 = new PCSResponsible
            {
                Code = ResponsibleCode1,
                Description = NewResponsibleDescription1
            };
            _mainResponsible2 = new PCSResponsible
            {
                Code = ResponsibleCode2,
                Description = NewResponsibleDescription2
            };

            _responsibleApiServiceMock = new Mock<IResponsibleApiService>();
            _responsibleApiServiceMock
                .Setup(x => x.TryGetResponsibleAsync(TestPlant, ResponsibleCode1))
                .Returns(Task.FromResult(_mainResponsible1));
            _responsibleApiServiceMock
                .Setup(x => x.TryGetResponsibleAsync(TestPlant, ResponsibleCode2))
                .Returns(Task.FromResult(_mainResponsible2));


            _command = new SyncResponsiblesCommand();

            _dut = new SyncResponsiblesCommandHandler(
                _responsibleRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object,
                _responsibleApiServiceMock.Object);
        }

        [TestMethod]
        public async Task HandlingResponsiblesCommand_ShouldUpdateResponsibles()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            AssertResponsibleProperties(_mainResponsible1, _responsible1);
            AssertResponsibleProperties(_mainResponsible2, _responsible2);
        }

        [TestMethod]
        public async Task HandlingResponsiblesCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        private void AssertResponsibleProperties(PCSResponsible mainResponsible, Responsible responsible)
            => Assert.AreEqual(mainResponsible.Description, responsible.Description);
    }
}

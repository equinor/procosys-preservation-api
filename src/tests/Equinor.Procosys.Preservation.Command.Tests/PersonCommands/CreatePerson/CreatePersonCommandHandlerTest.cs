using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.PersonCommands.CreatePerson;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.PersonCommands.CreatePerson
{
    [TestClass]
    public class CreatePersonCommandHandlerTest : CommandHandlerTestsBase
    {
        private Mock<IPersonRepository> _personRepositoryMock;
        private Person _personAdded;
        private CreatePersonCommand _command;
        private CreatePersonCommandHandler _dut;
        private readonly Guid _oid = new Guid("8e0e5be1-9546-49f6-9953-f1ac0412e87d");
        private const string FirstName = "Espen";
        private const string LastName = "Askeladd";

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _personRepositoryMock = new Mock<IPersonRepository>();
            _personRepositoryMock
                .Setup(x => x.Add(It.IsAny<Person>()))
                .Callback<Person>(x =>
                {
                    _personAdded = x;
                });

            _command = new CreatePersonCommand(_oid, FirstName, LastName);

            _dut = new CreatePersonCommandHandler(_personRepositoryMock.Object, UnitOfWorkMock.Object);
        }

        
        [TestMethod]
        public async Task HandlingCreatePersonCommand_ShouldAddPersonToRepository()
        {
            // Act
            var result = await _dut.Handle(_command, default);
            
            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(Unit.Value, result.Data);
            Assert.AreEqual(_oid, _personAdded.Oid);
            Assert.AreEqual(FirstName, _personAdded.FirstName);
            Assert.AreEqual(LastName, _personAdded.LastName);
        }

        [TestMethod]
        public async Task HandlingCreatePersonCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}

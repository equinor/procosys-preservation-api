using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.PersonCommands.CreatePerson;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Equinor.ProCoSys.Preservation.MainApi.Person;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.PersonCommands.CreatePerson
{
    [TestClass]
    public class CreatePersonCommandHandlerTest : CommandHandlerTestsBase
    {
        private Mock<IAuthenticator> _authenticatorMock;
        private Mock<IPersonRepository> _personRepositoryMock;
        private Mock<IPersonApiService> _personApiServiceMock;
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
            _authenticatorMock = new Mock<IAuthenticator>();
            _personRepositoryMock = new Mock<IPersonRepository>();
            _personRepositoryMock
                .Setup(x => x.Add(It.IsAny<Person>()))
                .Callback<Person>(x =>
                {
                    _personAdded = x;
                });
            _personApiServiceMock = new Mock<IPersonApiService>();
            _personApiServiceMock.Setup(p => p.TryGetPersonByOidAsync(_oid.ToString("D")))
                .Returns(Task.FromResult(new PCSPerson
                {
                    AzureOid = _oid.ToString("D"),
                    FirstName = FirstName,
                    LastName = LastName
                }));
            _command = new CreatePersonCommand(_oid);

            _dut = new CreatePersonCommandHandler(
                _authenticatorMock.Object,
                _personApiServiceMock.Object,
                _personRepositoryMock.Object,
                UnitOfWorkMock.Object);
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

        [TestMethod]
        public async Task HandlingCreatePersonCommand_ShouldThrowException_WhenPersonNotFoundInProCoSys()
        {
            // Arrange
            _personApiServiceMock.Setup(p => p.TryGetPersonByOidAsync(_oid.ToString("D")))
                .Returns(Task.FromResult<PCSPerson>(null));

            // Act
            await Assert.ThrowsExceptionAsync<Exception>(() => _dut.Handle(_command, default));
        }
    }
}

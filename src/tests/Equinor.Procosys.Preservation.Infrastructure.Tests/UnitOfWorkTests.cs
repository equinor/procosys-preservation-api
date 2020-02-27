using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Infrastructure.Tests
{
    [TestClass]
    public class UnitOfWorkTests
    {
        private const string Plant = "PCS$TESTPLANT";
        private readonly DateTime _currentTime = new DateTime(2020, 2, 1, 0, 0, 0, DateTimeKind.Utc);
        private DbContextOptions<PreservationContext> _dbContextOptions;
        private Mock<IPlantProvider> _plantProviderMock;
        private Mock<IEventDispatcher> _eventDispatcherMock;
        private Mock<ITimeService> _timeServiceMock;
        private Mock<ICurrentUserProvider> _currentUserProviderMock;

        [TestInitialize]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<PreservationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock.Setup(x => x.Plant)
                .Returns(Plant);

            _eventDispatcherMock = new Mock<IEventDispatcher>();

            _timeServiceMock = new Mock<ITimeService>();
            _timeServiceMock
                .Setup(x => x.GetCurrentTimeUtc())
                .Returns(_currentTime);

            _currentUserProviderMock = new Mock<ICurrentUserProvider>();
        }

        [TestMethod]
        public async Task SaveChangesAsync_SetsCreatedProperties_WhenCreated()
        {
            using var context = new PreservationContext(_dbContextOptions, _plantProviderMock.Object);

            var user = new Person(Guid.NewGuid(), "Current", "User");
            context.Persons.Add(user);
            context.SaveChanges();

            _currentUserProviderMock
                .Setup(x => x.GetCurrentUser())
                .Returns(Task.FromResult(user));

            var newMode = new Mode(Plant, "TestMode");
            context.Modes.Add(newMode);

            var dut = new UnitOfWork(context, _eventDispatcherMock.Object, _timeServiceMock.Object, _currentUserProviderMock.Object);
            await dut.SaveChangesAsync();

            Assert.AreEqual(_currentTime, newMode.CreatedAtUtc);
            Assert.AreEqual(user.Id, newMode.CreatedById);
            Assert.IsNull(newMode.ModifiedAtUtc);
            Assert.IsNull(newMode.ModifiedById);
        }

        [TestMethod]
        public async Task SaveChangesAsync_SetsModifiedProperties_WhenModified()
        {
            using var context = new PreservationContext(_dbContextOptions, _plantProviderMock.Object);

            var user = new Person(Guid.NewGuid(), "Current", "User");
            context.Persons.Add(user);
            context.SaveChanges();

            _currentUserProviderMock
                .Setup(x => x.GetCurrentUser())
                .Returns(Task.FromResult(user));

            var newMode = new Mode(Plant, "TestMode");
            context.Modes.Add(newMode);

            var dut = new UnitOfWork(context, _eventDispatcherMock.Object, _timeServiceMock.Object, _currentUserProviderMock.Object);
            await dut.SaveChangesAsync();

            newMode.Void();
            await dut.SaveChangesAsync();

            Assert.AreEqual(_currentTime, newMode.ModifiedAtUtc);
            Assert.AreEqual(user.Id, newMode.ModifiedById);
        }
    }
}

using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.MiscCommands.UpdateDateTimeSetting;
using Equinor.Procosys.Preservation.Domain.AggregateModels.SettingAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.MiscCommands.UpdateDateTimeSetting
{
    [TestClass]
    public class UpdateDateTimeSettingCommandTests : CommandHandlerTestsBase
    {
        private readonly int _settingId = 1;
        private readonly string _testSetting = "TestSetting";
        private readonly DateTime _dt = new DateTime(1989, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        private Mock<ISettingRepository> _settingRepositoryMock;
        private Setting _settingAdded;
        private Setting _settingToUpdate;
        private UpdateDateTimeSettingCommand _command;
        private UpdateDateTimeSettingCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            _settingToUpdate = new Setting(TestPlant, _testSetting);
            _settingToUpdate.SetProtectedIdForTesting(_settingId);
            _settingRepositoryMock = new Mock<ISettingRepository>();
            _settingRepositoryMock
                .Setup(x => x.GetByCodeAsync(_testSetting))
                .Returns(Task.FromResult(_settingToUpdate));
            _settingRepositoryMock
                .Setup(x => x.Add(It.IsAny<Setting>()))
                .Callback<Setting>(x =>
                {
                    _settingAdded = x;
                });

            _command = new UpdateDateTimeSettingCommand(_testSetting, _dt);

            _dut = new UpdateDateTimeSettingCommandHandler(
                _settingRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object
            );
        }
        
        [TestMethod]
        public async Task HandlingUpdateDateTimeSettingCommand_ShouldUpdateExistingSetting_WhenSettingAlreadyExists()
        {
            // Act
            var result = await _dut.Handle(_command, default);
            
            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_dt, _settingToUpdate.DateTimeUtc);
        }
        
        [TestMethod]
        public async Task HandlingUpdateDateTimeSettingCommand_ShouldNotAddSettingToRepository_WhenSettingAlreadyExists()
        {
            // Act
            var result = await _dut.Handle(_command, default);
            
            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsNull(_settingAdded);
            _settingRepositoryMock.Verify(r => r.Add(It.IsAny<Setting>()), Times.Never);
        }
        
        [TestMethod]
        public async Task HandlingUpdateDateTimeSettingCommand_ShouldAddSettingToRepository_WhenSettingNotExists()
        {
            // Arrange
            _settingRepositoryMock
                .Setup(x => x.GetByCodeAsync(_testSetting))
                .Returns(Task.FromResult<Setting>(null));

            // Act
            var result = await _dut.Handle(_command, default);
            
            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_testSetting, _settingAdded.Code);
            Assert.AreEqual(_dt, _settingAdded.DateTimeUtc);
        }

        [TestMethod]
        public async Task HandlingUpdateDateTimeSettingCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}

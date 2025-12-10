using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.SettingAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Tests.Repositories
{
    [TestClass]
    public class SettingRepositoryTests : RepositoryTestBase
    {
        private readonly string _code = "A";
        private readonly string _value = "B";
        private SettingRepository _dut;

        [TestInitialize]
        public void Setup()
        {
            var setting = new Setting
            {
                Code = _code,
                Value = _value
            };

            var settings = new List<Setting> { setting };

            var dbSetMock = settings.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.Settings)
                .Returns(dbSetMock.Object);

            _dut = new SettingRepository(ContextHelper.ContextMock.Object);
        }

        [TestMethod]
        public async Task GetByCode_ShouldReturnSetting_WhenSettingExists()
        {
            var result = await _dut.GetByCodeAsync(_code);

            // Assert
            Assert.AreEqual(_code, result.Code);
            Assert.AreEqual(_value, result.Value);
        }

        [TestMethod]
        public async Task GetByCode_ShouldReturnNull_WhenSettingNotExists()
        {
            var result = await _dut.GetByCodeAsync("XYZ");

            // Assert
            Assert.IsNull(result);
        }
    }
}

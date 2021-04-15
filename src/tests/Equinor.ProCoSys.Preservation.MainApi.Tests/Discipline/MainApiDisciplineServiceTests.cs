using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Client;
using Equinor.ProCoSys.Preservation.MainApi.Discipline;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.MainApi.Tests.Discipline
{
    [TestClass]
    public class MainApiDisciplineServiceTests
    {
        private const string _plant = "PCS$TESTPLANT";
        private Mock<IOptionsMonitor<MainApiOptions>> _mainApiOptions;
        private Mock<IBearerTokenApiClient> _mainApiClient;
        private PCSDiscipline _procosysDiscipline;
        private MainApiDisciplineService _dut;

        [TestInitialize]
        public void Setup()
        {
            _mainApiOptions = new Mock<IOptionsMonitor<MainApiOptions>>();
            _mainApiOptions
                .Setup(x => x.CurrentValue)
                .Returns(new MainApiOptions { ApiVersion = "4.0", BaseAddress = "http://example.com" });
            _mainApiClient = new Mock<IBearerTokenApiClient>();

            _procosysDiscipline = new PCSDiscipline
            {
                Id = 1,
                Code = "CodeA",
                Description = "Description1",
            };
           
            _dut = new MainApiDisciplineService(_mainApiClient.Object, _mainApiOptions.Object);
        }

        [TestMethod]
        public async Task TryGetDiscipline_ShouldReturnDiscipline()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.TryQueryAndDeserializeAsync<PCSDiscipline>(It.IsAny<string>()))
                .Returns(Task.FromResult(_procosysDiscipline));

            // Act
            var result = await _dut.TryGetDisciplineAsync(_plant, _procosysDiscipline.Code);

            // Assert
            Assert.AreEqual(_procosysDiscipline.Code, result.Code);
            Assert.AreEqual(_procosysDiscipline.Description, result.Description);
        }
    }
}

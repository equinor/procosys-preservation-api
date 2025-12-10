using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Client;
using Equinor.ProCoSys.Preservation.MainApi.Discipline;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.MainApi.Tests.Discipline
{
    [TestClass]
    public class MainApiDisciplineServiceTests
    {
        private const string Plant = "PCS$TESTPLANT";
        private Mock<IOptionsSnapshot<MainApiOptions>> _mainApiOptions;
        private Mock<IMainApiClientForUser> _mainApiClient;
        private PCSDiscipline _procosysDiscipline;
        private MainApiDisciplineService _dut;

        [TestInitialize]
        public void Setup()
        {
            _mainApiOptions = new Mock<IOptionsSnapshot<MainApiOptions>>();
            _mainApiOptions
                .Setup(x => x.Value)
                .Returns(new MainApiOptions { ApiVersion = "4.0", BaseAddress = "http://example.com" });
            _mainApiClient = new Mock<IMainApiClientForUser>();

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
                .SetupSequence(x => x.TryQueryAndDeserializeAsync<PCSDiscipline>(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>(),
                    null))
                .Returns(Task.FromResult(_procosysDiscipline));

            // Act
            var result = await _dut.TryGetDisciplineAsync(Plant, _procosysDiscipline.Code, CancellationToken.None);

            // Assert
            Assert.AreEqual(_procosysDiscipline.Code, result.Code);
            Assert.AreEqual(_procosysDiscipline.Description, result.Description);
        }
    }
}

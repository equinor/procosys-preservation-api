using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Responsible;
using Equinor.ProCoSys.Auth.Client;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.MainApi.Tests.Responsible
{
    [TestClass]
    public class MainApiResponsibleServiceTests
    {
        private const string _plant = "PCS$TESTPLANT";
        private Mock<IOptionsSnapshot<MainApiOptions>> _mainApiOptions;
        private Mock<IMainApiClient> _mainApiClient;
        private MainApiResponsibleService _dut;

        [TestInitialize]
        public void Setup()
        {
            _mainApiOptions = new Mock<IOptionsSnapshot<MainApiOptions>>();
            _mainApiOptions
                .Setup(x => x.Value)
                .Returns(new MainApiOptions { ApiVersion = "4.0", BaseAddress = "http://example.com" });
            _mainApiClient = new Mock<IMainApiClient>();

            _dut = new MainApiResponsibleService(_mainApiClient.Object, _mainApiOptions.Object);
        }

        [TestMethod]
        public async Task TryGetResponsibleCode_ShouldReturnResponsibleCode()
        {
            // Arrange
            var procosysResponsible = new PCSResponsible
            {
                Id = 1,
                Code = "CodeA",
                Description = "Description1",
            };
            _mainApiClient
                .SetupSequence(x => x.TryQueryAndDeserializeAsync<PCSResponsible>(It.IsAny<string>(), null))
                .Returns(Task.FromResult(procosysResponsible));
            // Act
            var result = await _dut.TryGetResponsibleAsync(_plant, procosysResponsible.Code);

            // Assert
            Assert.AreEqual(procosysResponsible.Code, result.Code);
            Assert.AreEqual(procosysResponsible.Description, result.Description);
        }
    }
}

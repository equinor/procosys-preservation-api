using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Project;
using Equinor.ProCoSys.Auth.Client;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.MainApi.Tests.Project
{
    [TestClass]
    public class MainApiProjectServiceTests
    {
        private const string _plant = "PCS$TESTPLANT";
        private Mock<IOptionsSnapshot<MainApiOptions>> _mainApiOptions;
        private Mock<IMainApiClient> _mainApiClient;
        private ProCoSysProject _result;
        private string _name = "NameA";
        private string _description = "Description1";
        private MainApiProjectService _dut;

        [TestInitialize]
        public void Setup()
        {
            _mainApiOptions = new Mock<IOptionsSnapshot<MainApiOptions>>();
            _mainApiOptions
                .Setup(x => x.Value)
                .Returns(new MainApiOptions { ApiVersion = "4.0", BaseAddress = "http://example.com" });
            _mainApiClient = new Mock<IMainApiClient>();

            _result = new ProCoSysProject {Id = 1, Name = _name, Description = _description};
            _dut = new MainApiProjectService(_mainApiClient.Object, _mainApiOptions.Object);
        }

        [TestMethod]
        public async Task TryGetProject_ShouldReturnProject()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.TryQueryAndDeserializeAsync<ProCoSysProject>(It.IsAny<string>(), null))
                .Returns(Task.FromResult(_result));

            // Act
            var result = await _dut.TryGetProjectAsync(_plant, _name);

            // Assert
            Assert.AreEqual(_name, result.Name);
            Assert.AreEqual(_description, result.Description);
        }
    }
}

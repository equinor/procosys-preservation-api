using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.TagFunction;
using Equinor.ProCoSys.Auth.Client;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.MainApi.Tests.TagFunction
{
    [TestClass]
    public class MainApiTagFunctionServiceTests
    {
        private const string _plant = "PCS$TESTPLANT";
        private Mock<IOptionsSnapshot<MainApiOptions>> _mainApiOptions;
        private Mock<IMainApiClientForApplication> _mainApiClient;
        private PCSTagFunction _result;
        private readonly string TagFunctionCode = "CodeTF";
        private readonly string RegisterCode = "CodeR";
        private readonly string Description = "Description1";
        private MainApiTagFunctionService _dut;

        [TestInitialize]
        public void Setup()
        {
            _mainApiOptions = new Mock<IOptionsSnapshot<MainApiOptions>>();
            _mainApiOptions
                .Setup(x => x.Value)
                .Returns(new MainApiOptions { ApiVersion = "4.0", BaseAddress = "http://example.com" });
            _mainApiClient = new Mock<IMainApiClientForApplication>();

            _result = new PCSTagFunction
            {
                Id = 1,
                Code = TagFunctionCode,
                Description = Description,
                RegisterCode = RegisterCode
            };

            _dut = new MainApiTagFunctionService(_mainApiClient.Object, _mainApiOptions.Object);
        }

        [TestMethod]
        public async Task TryGetTagFunction_ShouldReturnTagFunction()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.TryQueryAndDeserializeAsync<PCSTagFunction>(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>(),
                    null))
                .Returns(Task.FromResult(_result));

            // Act
            var result = await _dut.TryGetTagFunctionAsync(_plant, TagFunctionCode, RegisterCode, CancellationToken.None);

            // Assert
            Assert.AreEqual(TagFunctionCode, result.Code);
            Assert.AreEqual(Description, result.Description);
            Assert.AreEqual(RegisterCode, result.RegisterCode);
        }
    }
}

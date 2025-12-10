using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Client;
using Equinor.ProCoSys.Preservation.MainApi.TagFunction;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.MainApi.Tests.TagFunction
{
    [TestClass]
    public class MainApiTagFunctionServiceTests
    {
        private const string Plant = "PCS$TESTPLANT";
        private Mock<IOptionsSnapshot<MainApiOptions>> _mainApiOptions;
        private Mock<IMainApiClientForUser> _mainApiClient;
        private PCSTagFunction _result;
        private readonly string _tagFunctionCode = "CodeTF";
        private readonly string _registerCode = "CodeR";
        private readonly string _description = "Description1";
        private MainApiTagFunctionService _dut;

        [TestInitialize]
        public void Setup()
        {
            _mainApiOptions = new Mock<IOptionsSnapshot<MainApiOptions>>();
            _mainApiOptions
                .Setup(x => x.Value)
                .Returns(new MainApiOptions { ApiVersion = "4.0", BaseAddress = "http://example.com" });
            _mainApiClient = new Mock<IMainApiClientForUser>();

            _result = new PCSTagFunction
            {
                Id = 1,
                Code = _tagFunctionCode,
                Description = _description,
                RegisterCode = _registerCode
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
            var result = await _dut.TryGetTagFunctionAsync(Plant, _tagFunctionCode, _registerCode, CancellationToken.None);

            // Assert
            Assert.AreEqual(_tagFunctionCode, result.Code);
            Assert.AreEqual(_description, result.Description);
            Assert.AreEqual(_registerCode, result.RegisterCode);
        }
    }
}

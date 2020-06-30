using System;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.TagFunction;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.MainApi.Tests.TagFunction
{
    [TestClass]
    public class MainApiTagFunctionServiceTests
    {
        private const string _plant = "PCS$TESTPLANT";
        private Mock<IOptionsMonitor<MainApiOptions>> _mainApiOptions;
        private Mock<IBearerTokenApiClient> _mainApiClient;
        private Mock<IPlantCache> _plantCache;
        private ProcosysTagFunction _result;
        private readonly string TagFunctionCode = "CodeTF";
        private readonly string RegisterCode = "CodeR";
        private readonly string Description = "Description1";
        private MainApiTagFunctionService _dut;

        [TestInitialize]
        public void Setup()
        {
            _mainApiOptions = new Mock<IOptionsMonitor<MainApiOptions>>();
            _mainApiOptions
                .Setup(x => x.CurrentValue)
                .Returns(new MainApiOptions { ApiVersion = "4.0", BaseAddress = "http://example.com" });
            _mainApiClient = new Mock<IBearerTokenApiClient>();
            _plantCache = new Mock<IPlantCache>();
            _plantCache
                .Setup(x => x.IsValidPlantForCurrentUserAsync(_plant))
                .Returns(Task.FromResult(true));

            _result = new ProcosysTagFunction
            {
                Id = 1,
                Code = TagFunctionCode,
                Description = Description,
                RegisterCode = RegisterCode
            };

            _dut = new MainApiTagFunctionService(_mainApiClient.Object, _plantCache.Object, _mainApiOptions.Object);
        }

        [TestMethod]
        public async Task TryGetTagFunction_ReturnsTagFunction()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.TryQueryAndDeserializeAsync<ProcosysTagFunction>(It.IsAny<string>()))
                .Returns(Task.FromResult(_result));

            // Act
            var result = await _dut.TryGetTagFunctionAsync(_plant, TagFunctionCode, RegisterCode);

            // Assert
            Assert.AreEqual(TagFunctionCode, result.Code);
            Assert.AreEqual(Description, result.Description);
            Assert.AreEqual(RegisterCode, result.RegisterCode);
        }

        [TestMethod]
        public async Task TryGetTagFunction_ThrowsException_WhenPlantIsInvalid()
            => await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await _dut.TryGetTagFunctionAsync("INVALIDPLANT", "", ""));
    }
}

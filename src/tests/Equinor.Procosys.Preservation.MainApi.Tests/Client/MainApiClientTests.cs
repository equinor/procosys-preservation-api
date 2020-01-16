using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.MainApi.Tests.Client
{
    [TestClass]
    public class MainApiClientTests
    {
        private Mock<IBearerTokenProvider> _bearerTokenProvider;
        private Mock<ILogger<BearerTokenApiClient>> _logger;

        [TestInitialize]
        public void Setup()
        {
            _bearerTokenProvider = new Mock<IBearerTokenProvider>();
            _logger = new Mock<ILogger<BearerTokenApiClient>>();
        }

        [TestMethod]
        public async Task QueryAndDeserialize_ReturnsDeserialized_Object_TestAsync()
        {
            var httpClientFactory = HttpHelper.GetHttpClientFactory(HttpStatusCode.OK, "{\"Id\": 123}");
            var dut = new BearerTokenApiClient(httpClientFactory, _bearerTokenProvider.Object, _logger.Object);

            var response = await dut.QueryAndDeserialize<DummyClass>("");

            Assert.IsNotNull(response);
            Assert.AreEqual(123, response.Id);
        }

        [TestMethod]
        public async Task QueryAndDeserializeReturnsDefaultObject_WhenRequestIsNotSuccessful_TestAsync()
        {
            var httpClientFactory = HttpHelper.GetHttpClientFactory(HttpStatusCode.BadGateway, "");
            var dut = new BearerTokenApiClient(httpClientFactory, _bearerTokenProvider.Object, _logger.Object);

            var response = await dut.QueryAndDeserialize<DummyClass>("");

            Assert.AreEqual(default, response);
        }

        [TestMethod]
        public async Task QueryAndDeserialize_ThrowsException_WhenInvalidResponseIsReceived_TestAsync()
        {
            var httpClientFactory = HttpHelper.GetHttpClientFactory(HttpStatusCode.OK, "");
            var dut = new BearerTokenApiClient(httpClientFactory, _bearerTokenProvider.Object, _logger.Object);

            await Assert.ThrowsExceptionAsync<JsonException>(async () => await dut.QueryAndDeserialize<DummyClass>(""));
        }

        private class DummyClass
        {
            public int Id { get; set; }
        }
    }
}

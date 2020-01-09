using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.MainApi.Tests
{
    [TestClass]
    public class MainApiServiceTests
    {
        private class FakeHttpMessageHandler : DelegatingHandler
        {
            private HttpResponseMessage _fakeResponse;

            public FakeHttpMessageHandler(HttpResponseMessage responseMessage)
            {
                _fakeResponse = responseMessage;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return await Task.FromResult(_fakeResponse);
            }
        }

        private IHttpClientFactory GetHttpClientFactory(string jsonResponse)
        {
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            });
            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler)
            {
                BaseAddress = new Uri("http://example.com")
            };

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(fakeHttpClient);

            return httpClientFactoryMock.Object;
        }

        [TestMethod]
        public async Task Get_plants_returns_correct_number_or_plants_test_async()
        {
            // Arrange
            var jsonResponse = "[{\"Id\":\"PCS$AASTA_HANSTEEN\",\"Title\":\"AastaHansteen\"},{\"Id\":\"PCS$ASGARD\",\"Title\":\"Åsgard\"},{\"Id\":\"PCS$ASGARD_A\",\"Title\":\"ÅsgardA\"},{\"Id\":\"PCS$ASGARD_B\",\"Title\":\"ÅsgardB\"}]";
            var httpClientFactoryMock = GetHttpClientFactory(jsonResponse);
            var bearerTokenProvider = new Mock<IBearerTokenProvider>();
            var logger = new Mock<ILogger<MainApiService>>();
            var dut = new MainApiService(httpClientFactoryMock, bearerTokenProvider.Object, logger.Object);

            // Act
            var result = await dut.GetPlants();

            // Assert
            Assert.AreEqual(4, result.Count());
        }

        [TestMethod]
        public async Task Get_plants_sets_correct_properties_test_async()
        {
            // Arrange
            var jsonResponse = "[{\"Id\":\"PCS$AASTA_HANSTEEN\",\"Title\":\"AastaHansteen\"}]";
            var httpClientFactoryMock = GetHttpClientFactory(jsonResponse);
            var bearerTokenProvider = new Mock<IBearerTokenProvider>();
            var logger = new Mock<ILogger<MainApiService>>();
            var dut = new MainApiService(httpClientFactoryMock, bearerTokenProvider.Object, logger.Object);

            // Act
            var result = await dut.GetPlants();

            // Assert
            var plant = result.First();
            Assert.AreEqual("PCS$AASTA_HANSTEEN", plant.Id);
            Assert.AreEqual("AastaHansteen", plant.Title);
        }
    }
}

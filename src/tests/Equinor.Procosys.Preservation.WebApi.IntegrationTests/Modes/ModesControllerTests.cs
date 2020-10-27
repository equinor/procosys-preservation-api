using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Modes
{
    [TestClass]
    public class ModesControllerTests : TestBase
    {
        private const string ModesPath = "Modes";

        [TestMethod]
        public async Task Get_AllModes_AsAnonymous_ShouldReturnUnauthorized()
        {
            // Act
            var response = await anonymousClient.GetAsync($"{ModesPath}");

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task Get_AllModes_AsHacker_ShouldReturnForbidden()
        {
            // Act
            var response = await hackerClient.GetAsync($"{ModesPath}");

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [TestMethod]
        public async Task Get_AllModes_AsAdmin_ShouldReturnOk()
        {
            // Act
            var response = await adminClient.GetAsync($"{ModesPath}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content);
            var modes = JsonConvert.DeserializeObject<List<ModeDto>>(content);
            Assert.AreEqual(0, modes.Count);
        }
    }
}

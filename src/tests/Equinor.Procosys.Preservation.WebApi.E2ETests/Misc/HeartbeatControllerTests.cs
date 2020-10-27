using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Equinor.Procosys.Preservation.WebApi.E2ETests.Misc
{
    [TestClass]
    public class HeartbeatControllerTests : E2ETestBase
    {
        private const string HeartbeatPath = "Heartbeat";

        [TestMethod]
        public async Task Get_IsAlive_AsAnonymous_ShouldReturnOk()
        {
            // Act
            var response = await anonymousClient.GetAsync($"{HeartbeatPath}/IsAlive");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content);
            var isAlive = JsonConvert.DeserializeObject(content);
            // todo Assert on Json-object
            Assert.IsNotNull(isAlive);
        }
    }
}

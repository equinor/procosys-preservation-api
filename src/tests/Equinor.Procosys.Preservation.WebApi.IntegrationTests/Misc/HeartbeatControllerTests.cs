using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Misc
{
    [TestClass]
    public class HeartbeatControllerTests : TestBase
    {
        private const string HeartbeatPath = "Heartbeat";

        [TestMethod]
        public async Task Get_IsAlive_AsAnonymous_ShouldReturnOk()
        {
            // Act
            var response = await AnonymousClient.GetAsync($"{HeartbeatPath}/IsAlive");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content);
            var dto = JsonConvert.DeserializeObject<HeartbeatDto>(content);
            Assert.IsNotNull(dto);
            Assert.IsTrue(dto.IsAlive);
            Assert.IsNotNull(dto.TimeStamp);
        }
    }
}

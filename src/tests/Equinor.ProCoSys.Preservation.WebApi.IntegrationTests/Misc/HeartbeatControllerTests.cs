using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Misc
{
    [TestClass]
    public class HeartbeatControllerTests : TestBase
    {
        private const string _route = "Heartbeat";

        [TestMethod]
        public async Task Get_IsAlive_AsAnonymous_ShouldReturnOk() => await AssertIsAlive(UserType.Anonymous);

        [TestMethod]
        public async Task Get_IsAlive_AsHacker_ShouldReturnOk() => await AssertIsAlive(UserType.Hacker);

        private static async Task AssertIsAlive(UserType userType, HttpStatusCode expectedHttpStatusCode = HttpStatusCode.OK)
        {
            var response = await TestFactory.Instance.GetHttpClient(userType, null).GetAsync($"{_route}/IsAlive");

            // Assert
            Assert.AreEqual(expectedHttpStatusCode, response.StatusCode);
            if (expectedHttpStatusCode != HttpStatusCode.OK)
            {
                return;
            }
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content);
            var dto = JsonConvert.DeserializeObject<HeartbeatDto>(content);
            Assert.IsNotNull(dto);
            Assert.IsTrue(dto.IsAlive);
            Assert.IsNotNull(dto.TimeStamp);
        }
    }
}

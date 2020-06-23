using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Misc
{
    [TestClass]
    public class RequestBearerTokenProviderTests
    {
        [TestMethod]
        public void GetBearerTokenReturnsToken()
        {
            var dut = new RequestBearerTokenProvider();
            dut.SetBearerToken("Token");

            var token = dut.GetBearerToken();

            Assert.AreEqual("Token", token);
        }
    }
}

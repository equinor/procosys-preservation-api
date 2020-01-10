using Equinor.Procosys.Preservation.WebApi.Misc;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.WebApi.Tests.Misc
{
    [TestClass]
    public class RequestBearerTokenProviderTests
    {
        [TestMethod]
        public void GetBearerTokenReturnsToken()
        {
            IHeaderDictionary headers = new HeaderDictionary { { "Authorization", "Bearer Token" }  };

            var httpRequest = new Mock<HttpRequest>();
            httpRequest
                .Setup(x => x.Headers)
                .Returns(headers);

            var httpContext = new Mock<HttpContext>();
            httpContext
                .Setup(x => x.Request)
                .Returns(httpRequest.Object);

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor
                .Setup(x => x.HttpContext)
                .Returns(httpContext.Object);
            var dut = new RequestBearerTokenProvider(httpContextAccessor.Object);

            var token = dut.GetBearerToken();

            Assert.AreEqual("Token", token);
        }
    }
}

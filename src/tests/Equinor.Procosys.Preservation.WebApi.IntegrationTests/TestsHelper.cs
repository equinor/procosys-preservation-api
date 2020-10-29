using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests
{
    public static class TestsHelper
    {
        public static async Task AssertMessageOnBadRequestAsync(HttpResponseMessage result, string expectedMessageOnBadRequest)
        {
            if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                var jsonString = await result.Content.ReadAsStringAsync();
                Console.WriteLine(jsonString);
                Assert.AreEqual(expectedMessageOnBadRequest, jsonString);
            }
        }
    }
}

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests
{
    public static class TestsHelper
    {
        public static async Task AssertResponseAsync(
            HttpResponseMessage response, 
            HttpStatusCode expectedStatusCode,
            string expectedMessagePartOnBadRequest)
        {
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Bad request details: {jsonString}");
                
                if (!string.IsNullOrEmpty(expectedMessagePartOnBadRequest))
                {
                    var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(jsonString);
                    Assert.IsTrue(problemDetails.Errors.SelectMany(e => e.Value).Any(e => e.Contains(expectedMessagePartOnBadRequest)));
                }
            }

            Assert.AreEqual(expectedStatusCode, response.StatusCode);
        }
    }
}

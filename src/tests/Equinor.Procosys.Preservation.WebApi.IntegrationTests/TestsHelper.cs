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
        public static async Task AssertMessageOnBadRequestAsync(
            HttpResponseMessage result, 
            string expectedMessagePartOnBadRequest)
        {
            if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                var jsonString = await result.Content.ReadAsStringAsync();
                Console.WriteLine($"Bad request details: {jsonString}");
                var problemDetails = JsonConvert.DeserializeObject<ValidationProblemDetails>(jsonString);
                Assert.IsTrue(problemDetails.Errors.SelectMany(e => e.Value).Any(e => e.Contains(expectedMessagePartOnBadRequest)));
            }
        }
    }
}

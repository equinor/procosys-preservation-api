using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.E2ETests
{
    [TestClass]
    public abstract class E2ETestBase
    {
        protected static TestFactory testFactory;
        protected static HttpClient adminClient;
        protected static HttpClient anonymousClient;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            if (testFactory == null)
            {
                var projectDir = Directory.GetCurrentDirectory();
                var configPath = Path.Combine(projectDir, "appsettings.json");
                testFactory = new TestFactory();
                
                anonymousClient = testFactory.WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment(Startup.E2ETestEnvironment);
                    builder.ConfigureAppConfiguration((context, conf) => conf.AddJsonFile(configPath));
                }).CreateClient();
                
                adminClient = testFactory.WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment(Startup.E2ETestEnvironment);
                    builder.ConfigureAppConfiguration((context, conf) => conf.AddJsonFile(configPath));
                }).CreateClient();
            }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            if (testFactory != null)
            {
                testFactory.Dispose();
                testFactory = null;
            }
            if (anonymousClient != null)
            {
                anonymousClient.Dispose();
                anonymousClient = null;
            }
            if (adminClient != null)
            {
                adminClient.Dispose();
                adminClient = null;
            }
        }
    }
}

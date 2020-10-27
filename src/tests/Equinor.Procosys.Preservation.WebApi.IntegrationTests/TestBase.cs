using System;
using System.IO;
using System.Net.Http;
using Equinor.Procosys.Preservation.WebApi.IntegrationTests.Clients;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests

{
    [TestClass]
    public abstract class TestBase
    {
        protected static TestFactory testFactory;
        protected static HttpClient anonymousClient;
        protected static HttpClient adminClient;
        protected static HttpClient plannerClient;
        protected static HttpClient preserverClient;
        protected static HttpClient readerClient;
        protected static HttpClient hackerClient;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            if (testFactory == null)
            {
                testFactory = new TestFactory();

                anonymousClient = testFactory.CreateTestClient(null);
                adminClient = testFactory.CreateTestClient(AdminClient.Tokens);
                plannerClient = testFactory.CreateTestClient(PlannerClient.Tokens);
                preserverClient = testFactory.CreateTestClient(PreserverClient.Tokens);
                readerClient = testFactory.CreateTestClient(ReaderClient.Tokens);
                hackerClient = testFactory.CreateTestClient(HackerClient.Tokens);
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

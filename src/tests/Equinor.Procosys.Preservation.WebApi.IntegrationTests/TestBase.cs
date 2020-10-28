using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests

{
    [TestClass]
    public abstract class TestBase
    {
        protected static TestFactory TestFactory;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            if (TestFactory == null)
            {
                TestFactory = new TestFactory();
            }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            if (TestFactory != null)
            {
                TestFactory.Dispose();
                TestFactory = null;
            }
        }

        public HttpClient AnonymousClient => TestFactory.AnonymousClient;
        public HttpClient LibraryAdminClient => TestFactory.LibraryAdminClient;
        public HttpClient PlannerClient => TestFactory.PlannerClient;
        public HttpClient PreserverClient => TestFactory.PreserverClient;
        public HttpClient ReaderClient => TestFactory.ReaderClient;
        public HttpClient HackerClient => TestFactory.HackerClient;
    }
}

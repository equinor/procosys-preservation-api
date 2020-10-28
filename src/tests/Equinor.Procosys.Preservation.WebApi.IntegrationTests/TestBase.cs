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

        public HttpClient AnonymousClient => TestFactory.GetAnonymousClient();
        public HttpClient LibraryAdminClient => TestFactory.GetLibraryAdminClient();
        public HttpClient PlannerClient => TestFactory.GetPlannerClient();
        public HttpClient PreserverClient => TestFactory.GetPreserverClient();
        public HttpClient AuthenticatedHackerClient => TestFactory.GetAuthenticatedHackerClient();
    }
}

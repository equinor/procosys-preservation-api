using System.Net.Http;
using Equinor.Procosys.Preservation.Command.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests

{
    [TestClass]
    public abstract class TestBase
    {
        protected static TestFactory TestFactory;
        private readonly RowVersionValidator _rowVersionValidator = new RowVersionValidator();

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

        public HttpClient AnonymousClient(string plant) => TestFactory.GetClientForPlant(TestFactory.AnonymousUser, plant);
        public HttpClient LibraryAdminClient(string plant) => TestFactory.GetClientForPlant(TestFactory.LibraryAdminUser, plant);
        public HttpClient PlannerClient(string plant) => TestFactory.GetClientForPlant(TestFactory.PlannerUser, plant);
        public HttpClient PreserverClient(string plant) => TestFactory.GetClientForPlant(TestFactory.PreserverUser, plant);
        public HttpClient AuthenticatedHackerClient(string plant) => TestFactory.GetClientForPlant(TestFactory.HackerUser, plant);

        public void AssertRowVersionChange(string oldRowVersion, string newRowVersion)
        {
            Assert.IsTrue(_rowVersionValidator.IsValid(oldRowVersion));
            Assert.IsTrue(_rowVersionValidator.IsValid(newRowVersion));
            Assert.AreNotEqual(oldRowVersion, newRowVersion);
        }
    }
}

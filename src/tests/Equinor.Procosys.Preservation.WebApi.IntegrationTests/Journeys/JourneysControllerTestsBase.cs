using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Responsible;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.WebApi.IntegrationTests.Journeys
{
    [TestClass]
    public class JourneysControllerTestsBase : TestBase
    {
        protected int ModeIdUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            ModeIdUnderTest = TestFactory.KnownTestData.StandardTagIds.First();

            TestFactory
                .ResponsibleApiServiceMock
                .Setup(service => service.TryGetResponsibleAsync(TestFactory.PlantWithAccess, KnownTestData.ResponsibleCode))
                .Returns(Task.FromResult(new ProcosysResponsible
                {
                    Code = KnownTestData.ResponsibleCode, Description = KnownTestData.ResponsibleDescription
                }));
        }
    }
}

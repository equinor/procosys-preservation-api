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
        protected int JourneyIdUnderTest;
        protected int StepIdUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            ModeIdUnderTest = TestFactory.KnownTestData.ModeIds.First();
            JourneyIdUnderTest = TestFactory.KnownTestData.JourneyIds.First();
            StepIdUnderTest = TestFactory.KnownTestData.StepIds.First();

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

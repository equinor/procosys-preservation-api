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
        protected int JourneyAIdUnderTest;
        protected int StepInJourneyAIdUnderTest;
        protected int JourneyBIdUnderTest;
        protected int StepInJourneyBIdUnderTest;

        [TestInitialize]
        public async Task TestInitialize()
        {
            ModeIdUnderTest = TestFactory.KnownTestData.ModeIds.First();
            var journeys = await JourneysControllerTestsHelper.GetJourneysAsync(LibraryAdminClient(TestFactory.PlantWithAccess));
            var journeyA = journeys.Single(j => j.Title == KnownTestData.JourneyA);
            JourneyAIdUnderTest = journeyA.Id;
            StepInJourneyAIdUnderTest = journeyA.Steps.First().Id;
            var journeyB = journeys.Single(j => j.Title == KnownTestData.JourneyB);
            JourneyBIdUnderTest = journeyB.Id;
            StepInJourneyBIdUnderTest = journeyB.Steps.First().Id;

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

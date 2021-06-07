using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.MainApi.Responsible;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Journeys
{
    public class JourneysControllerTestsBase : TestBase
    {
        protected int ModeIdUnderTest;
        protected int JourneyWithTagsIdUnderTest;
        protected int StepInJourneyWithTagsIdUnderTest;
        protected int JourneyNotInUseIdUnderTest;
        protected int StepInJourneyNotInUseIdUnderTest;

        [TestInitialize]
        public async Task TestInitialize()
        {
            ModeIdUnderTest = TestFactory.Instance.SeededData[KnownPlantData.PlantA].ModeId;

            var journeys = await JourneysControllerTestsHelper.GetJourneysAsync(UserType.LibraryAdmin, TestFactory.PlantWithAccess);
            var journeyWithTags = journeys.Single(j => j.Title == KnownTestData.JourneyWithTags);
            JourneyWithTagsIdUnderTest = journeyWithTags.Id;
            StepInJourneyWithTagsIdUnderTest = journeyWithTags.Steps.First().Id;
            var journeyNotInUse = journeys.Single(j => j.Title == KnownTestData.JourneyNotInUse);
            JourneyNotInUseIdUnderTest = journeyNotInUse.Id;
            StepInJourneyNotInUseIdUnderTest = journeyNotInUse.Steps.First().Id;

            TestFactory.Instance
                .ResponsibleApiServiceMock
                .Setup(service => service.TryGetResponsibleAsync(TestFactory.PlantWithAccess, KnownTestData.ResponsibleCode))
                .Returns(Task.FromResult(new PCSResponsible
                {
                    Code = KnownTestData.ResponsibleCode, Description = KnownTestData.ResponsibleDescription
                }));
        }
    }
}

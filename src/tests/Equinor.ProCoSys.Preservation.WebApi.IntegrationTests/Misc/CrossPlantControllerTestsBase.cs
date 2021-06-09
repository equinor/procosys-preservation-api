using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Misc
{
    public class CrossPlantControllerTestsBase : TestBase
    {
        protected int ActionIdUnderTest_WithAttachments_Closed_InPlantA;
        protected int ActionIdUnderTest_WithAttachments_Closed_InPlantB;
        protected int TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started_InPlantA;
        protected int TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started_InPlantB;

        [TestInitialize]
        public void TestInitialize()
        {
            TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started_InPlantA
                = TestFactory.Instance.SeededData[KnownPlantData.PlantA].TagId_ForStandardTagWithAttachmentsAndActionAttachments_Started;
            TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started_InPlantB
                = TestFactory.Instance.SeededData[KnownPlantData.PlantB].TagId_ForStandardTagWithAttachmentsAndActionAttachments_Started;
            ActionIdUnderTest_WithAttachments_Closed_InPlantA
                = TestFactory.Instance.SeededData[KnownPlantData.PlantA].ActionId_ForActionWithAttachments_Closed;
            ActionIdUnderTest_WithAttachments_Closed_InPlantB
                = TestFactory.Instance.SeededData[KnownPlantData.PlantB].ActionId_ForActionWithAttachments_Closed;
        }
    }
}

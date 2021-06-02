using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Misc
{
    [TestClass]
    public class CrossPlantControllerTestsBase : TestBase
    {
        protected int ActionIdUnderTest_WithAttachments_Closed_InPlantA;
        protected int ActionIdUnderTest_WithAttachments_Closed_InPlantB;

        [TestInitialize]
        public void TestInitialize()
        {
            ActionIdUnderTest_WithAttachments_Closed_InPlantA
                = TestFactory.Instance.SeededData[KnownPlantData.PlantA].ActionId_ForActionWithAttachments_Closed;
            ActionIdUnderTest_WithAttachments_Closed_InPlantB
                = TestFactory.Instance.SeededData[KnownPlantData.PlantB].ActionId_ForActionWithAttachments_Closed;
        }
    }
}

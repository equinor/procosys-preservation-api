using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Misc
{
    [TestClass]
    public class CrossPlantControllerTests : CrossPlantControllerTestsBase
    {
        [TestMethod]
        public async Task GetAllActions_AsCrossPlantUser_ShouldGetActions()
        {
            // Act
            var actionDtos = await CrossPlantControllerTestsHelper.GetActionsAsync(UserType.CrossPlantUser);

            // Assert
            Assert.IsNotNull(actionDtos);
            Assert.IsTrue(actionDtos.Count > 0);
            AssertClosedActionWithAttachments(
                actionDtos.SingleOrDefault(a => a.Id == ActionIdUnderTest_WithAttachments_Closed_InPlantA),
                KnownPlantData.PlantA,
                KnownPlantData.PlantATitle);
            AssertClosedActionWithAttachments(
                actionDtos.SingleOrDefault(a => a.Id == ActionIdUnderTest_WithAttachments_Closed_InPlantB),
                KnownPlantData.PlantB,
                KnownPlantData.PlantBTitle);
        }

        private void AssertClosedActionWithAttachments(ActionDto actionDto, string plantId, string plantTitle)
        {
            Assert.IsNotNull(actionDto);
            Assert.AreEqual(actionDto.PlantId, plantId);
            Assert.AreEqual(actionDto.PlantTitle, plantTitle);
            Assert.AreEqual(actionDto.ProjectName, KnownTestData.ProjectName);
            Assert.AreEqual(actionDto.ProjectDescription, KnownTestData.ProjectDescription);
            Assert.IsTrue(actionDto.Title.StartsWith(KnownTestData.Action));
            Assert.IsTrue(actionDto.IsClosed);
            Assert.IsNotNull(actionDto.TagNo);
            Assert.IsTrue(actionDto.Id > 0);
            Assert.IsTrue(actionDto.TagId > 0);
            Assert.IsTrue(actionDto.AttachmentCount > 0);
        }
    }
}

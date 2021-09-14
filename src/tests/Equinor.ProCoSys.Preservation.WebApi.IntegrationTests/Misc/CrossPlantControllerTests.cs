using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Misc
{
    [TestClass]
    public class CrossPlantControllerTests : CrossPlantControllerTestsBase
    {
        [TestMethod]
        public async Task GetAllTags_AsCrossPlantUser_ShouldGetTags()
        {
            // Act
            var tagDtos = await CrossPlantControllerTestsHelper.GetTagsAsync(UserType.CrossPlantApp);

            // Assert
            Assert.IsNotNull(tagDtos);
            Assert.IsTrue(tagDtos.Count > 0);
            AssertTag(
                tagDtos.SingleOrDefault(a => a.Id == TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started_InPlantA),
                KnownPlantData.PlantA,
                KnownPlantData.PlantATitle);
            AssertTag(
                tagDtos.SingleOrDefault(a => a.Id == TagIdUnderTest_ForStandardTagWithAttachmentsAndActionAttachments_Started_InPlantB),
                KnownPlantData.PlantB,
                KnownPlantData.PlantBTitle);
        }

        [TestMethod]
        public async Task GetAllActions_AsCrossPlantUser_ShouldGetActions()
        {
            // Act
            var actionDtos = await CrossPlantControllerTestsHelper.GetActionsAsync(UserType.CrossPlantApp);

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
            Assert.AreEqual(plantId, actionDto.PlantId);
            Assert.AreEqual(plantTitle, actionDto.PlantTitle);
            Assert.AreEqual(KnownTestData.ProjectName, actionDto.ProjectName);
            Assert.AreEqual(KnownTestData.ProjectDescription, actionDto.ProjectDescription);
            Assert.IsTrue(actionDto.Title.StartsWith(KnownTestData.Action));
            Assert.IsTrue(actionDto.IsClosed);
            Assert.IsNotNull(actionDto.TagNo);
            Assert.IsTrue(actionDto.Id > 0);
            Assert.IsTrue(actionDto.TagId > 0);
            Assert.IsTrue(actionDto.AttachmentCount > 0);
        }

        private void AssertTag(TagDto tagDto, string plantId, string plantTitle)
        {
            Assert.IsNotNull(tagDto);
            Assert.AreEqual(plantId, tagDto.PlantId);
            Assert.AreEqual(plantTitle, tagDto.PlantTitle);
            Assert.AreEqual(KnownTestData.ProjectName, tagDto.ProjectName);
            Assert.AreEqual(KnownTestData.ProjectDescription, tagDto.ProjectDescription);
            Assert.IsTrue(tagDto.TagNo.StartsWith(KnownTestData.StandardTagNo));
            Assert.IsTrue(tagDto.Id > 0);
            Assert.AreEqual(TagType.Standard, tagDto.TagType);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.Validators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.ProCoSys.Preservation.MainApi.Tag;
using Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.RequirementTypes;
using Equinor.ProCoSys.Preservation.WebApi.IntegrationTests.Tags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.WebApi.IntegrationTests
{
    [TestClass]
    public abstract class TestBase
    {
        private readonly RowVersionValidator _rowVersionValidator = new RowVersionValidator();

        [AssemblyCleanup]
        public static void AssemblyCleanup() => TestFactory.Instance.Dispose();

        public void AssertRowVersionChange(string oldRowVersion, string newRowVersion)
        {
            Assert.IsTrue(_rowVersionValidator.IsValid(oldRowVersion));
            Assert.IsTrue(_rowVersionValidator.IsValid(newRowVersion));
            Assert.AreNotEqual(oldRowVersion, newRowVersion);
        }

        protected async Task<int> CreateRequirementDefinitionAsync(string plant)
        {
            var reqTypes = await RequirementTypesControllerTestsHelper.GetRequirementTypesAsync(UserType.LibraryAdmin, plant);
            var newReqDefId = await RequirementTypesControllerTestsHelper.CreateRequirementDefinitionAsync(
                UserType.LibraryAdmin, plant, reqTypes.First().Id, Guid.NewGuid().ToString());
            return newReqDefId;
        }

        protected async Task<int> CreateStandardTagAsync(
            int stepId,
            bool startPreservation)
        {
            var newReqDefId = await CreateRequirementDefinitionAsync(TestFactory.PlantWithAccess);

            var tagNo = Guid.NewGuid().ToString();
            MockMainApiDataForTag(tagNo);

            var newTagIds = await TagsControllerTestsHelper.CreateStandardTagAsync(
                UserType.Planner,
                TestFactory.PlantWithAccess,
                TestFactory.ProjectWithAccess,
                new[] { tagNo },
                new List<TagRequirementDto>
                {
                    new TagRequirementDto
                    {
                        IntervalWeeks = 4,
                        RequirementDefinitionId = newReqDefId
                    }
                },
                stepId,
                null,
                null);

            if (startPreservation)
            {
                await TagsControllerTestsHelper.StartPreservationAsync(UserType.Planner, TestFactory.PlantWithAccess, newTagIds);
                await AssertInHistoryAsLatestEventAsync(newTagIds.Single(), UserType.Planner, EventType.PreservationStarted);
            }
            return newTagIds.Single();
        }

        protected async Task AssertInHistoryAsExistingEventAsync(int tagIdUnderTest, UserType userType, EventType eventType)
        {
            var historyDtos = await TagsControllerTestsHelper.GetHistoryAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagIdUnderTest);

            // history records are sorted with newest first in list
            var historyDto = historyDtos.First(h => h.Description.StartsWith(eventType.GetDescription()));
            AssertCreatedBy(userType, historyDto);
        }

        protected async Task AssertInHistoryAsLatestEventAsync(int tagId, UserType userType, EventType eventType)
        {
            var historyDtos = await TagsControllerTestsHelper.GetHistoryAsync(
                UserType.Preserver, TestFactory.PlantWithAccess,
                tagId);

            // history records are sorted with newest first in list
            var historyDto = historyDtos.First();
            Assert.IsTrue(historyDto.Description.StartsWith(eventType.GetDescription()));
            AssertCreatedBy(userType, historyDto);
        }

        protected void AssertCreatedBy(UserType userType, HistoryDto historyDto)
        {
            var profile = TestFactory.Instance.GetTestProfile(userType);
            AssertUser(profile, historyDto.CreatedBy);
        }

        protected void AssertUser(TestProfile profile, PersonDto personDto)
        {
            Assert.IsNotNull(personDto);
            Assert.AreEqual(profile.FirstName, personDto.FirstName);
            Assert.AreEqual(profile.LastName, personDto.LastName);
        }

        private void MockMainApiDataForTag(string tagNo)
        {
            var mainTagDetails = new PCSTagDetails
            {
                AreaCode = "ACode",
                AreaDescription = "ADesc",
                CallOffNo = "CalloffNo",
                CommPkgNo = "CommPkgNo",
                CommPkgProCoSysGuid = Guid.NewGuid(),
                Description = $"{tagNo}Description",
                DisciplineCode = "DCode",
                DisciplineDescription = "DDesc",
                McPkgNo = "McPkgNo",
                McPkgProCoSysGuid = Guid.NewGuid(),
                PurchaseOrderNo = "PurchaseOrderNo",
                TagFunctionCode = "TFCode",
                ProCoSysGuid = Guid.NewGuid(),
                TagNo = tagNo
            };

            IList<PCSTagDetails> mainTagDetailList = new List<PCSTagDetails> { mainTagDetails };
            TestFactory.Instance
                .TagApiServiceMock
                .Setup(service => service.GetTagDetailsAsync(
                    TestFactory.PlantWithAccess,
                    TestFactory.ProjectWithAccess,
                    new List<string> { tagNo },
                    CancellationToken.None,
                    false))
                .Returns(Task.FromResult(mainTagDetailList));
        }
    }
}

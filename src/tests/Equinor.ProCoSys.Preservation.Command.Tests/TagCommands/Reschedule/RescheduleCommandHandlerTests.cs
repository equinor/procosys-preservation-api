using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagCommands.Reschedule;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagCommands.Reschedule
{
    [TestClass]
    public class RescheduleCommandHandlerTests : CommandHandlerTestsBase
    {
        private RescheduleCommand _rescheduleOneWeekLaterCommand;
        private RescheduleCommand _rescheduleFourWeeksEarlierCommand;
        private const string _rowVersion1 = "AAAAAAAAABA=";
        private const string _rowVersion2 = "AAAAAAAABBA=";
        private Tag _tag1;
        private Tag _tag2;
        private TagRequirement _req1OnTag1;
        private TagRequirement _req2OnTag1;
        private TagRequirement _req1OnTag2;
        private TagRequirement _req2OnTag2;
        private Mock<RequirementDefinition> _rd1Mock;
        private Mock<RequirementDefinition> _rd2Mock;

        private int _rdId1 = 17;
        private int _rdId2 = 18;
        private int _tagId1 = 7;
        private int _tagId2 = 8;

        private RescheduleCommandHandler _dut;

        [TestInitialize]
        public void Setup()
        {
            var stepMock = new Mock<Step>();
            stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
            _rd1Mock = new Mock<RequirementDefinition>();
            _rd1Mock.SetupGet(rd => rd.Id).Returns(_rdId1);
            _rd1Mock.SetupGet(rd => rd.Plant).Returns(TestPlant);
            _rd2Mock = new Mock<RequirementDefinition>();
            _rd2Mock.SetupGet(rd => rd.Id).Returns(_rdId2);
            _rd2Mock.SetupGet(rd => rd.Plant).Returns(TestPlant);

            _req1OnTag1 = new TagRequirement(TestPlant, 1, _rd1Mock.Object);
            _req2OnTag1 = new TagRequirement(TestPlant, 2, _rd2Mock.Object);
            _req1OnTag2 = new TagRequirement(TestPlant, 3, _rd1Mock.Object);
            _req2OnTag2 = new TagRequirement(TestPlant, 4, _rd2Mock.Object);
            
            _tag1 = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "", "", stepMock.Object,
                new List<TagRequirement> {_req1OnTag1, _req2OnTag1});
            _tag1.StartPreservation();
            _tag1.SetProtectedIdForTesting(_tagId1);

            _tag2 = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "", "", stepMock.Object,
                new List<TagRequirement> {_req1OnTag2, _req2OnTag2});
            _tag2.StartPreservation();
            _tag2.SetProtectedIdForTesting(_tagId2);

            var tags = new List<Tag> {_tag1, _tag2};

            var tagIds = new List<int> {_tagId1, _tagId2};
            var tagIdsWithRowVersion = new List<IdAndRowVersion>
            {
                new IdAndRowVersion(_tagId1, _rowVersion1), new IdAndRowVersion(_tagId2, _rowVersion2)
            };
            
            var projectRepoMock = new Mock<IProjectRepository>();
            projectRepoMock.Setup(r => r.GetTagsWithPreservationHistoryByTagIdsAsync(tagIds))
                .Returns(Task.FromResult(tags));
            _rescheduleOneWeekLaterCommand = new RescheduleCommand(tagIdsWithRowVersion, 1, RescheduledDirection.Later, "Comment");
            _rescheduleFourWeeksEarlierCommand = new RescheduleCommand(tagIdsWithRowVersion, 4, RescheduledDirection.Earlier, "Comment");

            _dut = new RescheduleCommandHandler(projectRepoMock.Object, UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingRescheduleCommand_ShouldRescheduleLaterOnAllRequirementsOnAllTags()
        {
            var expectedNextDueTimeUtcReq1OnTag1 = _req1OnTag1.ActivePeriod.DueTimeUtc.AddWeeks(1);
            var expectedNextDueTimeUtcReq2OnTag1 = _req2OnTag1.ActivePeriod.DueTimeUtc.AddWeeks(1);
            var expectedNextDueTimeUtcReq1OnTag2 = _req1OnTag2.ActivePeriod.DueTimeUtc.AddWeeks(1);
            var expectedNextDueTimeUtcReq2OnTag2 = _req2OnTag2.ActivePeriod.DueTimeUtc.AddWeeks(1);
            
            await _dut.Handle(_rescheduleOneWeekLaterCommand, default);

            Assert.AreEqual(expectedNextDueTimeUtcReq1OnTag1, _req1OnTag1.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtcReq2OnTag1, _req2OnTag1.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtcReq1OnTag2, _req1OnTag2.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtcReq2OnTag2, _req2OnTag2.NextDueTimeUtc);

            Assert.AreEqual(expectedNextDueTimeUtcReq1OnTag1, _tag1.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtcReq1OnTag2, _tag2.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingRescheduleCommand_ShouldRescheduleEarlierOnAllRequirementsOnAllTags()
        {
            var expectedNextDueTimeUtcReq1OnTag1 = _req1OnTag1.ActivePeriod.DueTimeUtc.AddWeeks(-4);
            var expectedNextDueTimeUtcReq2OnTag1 = _req2OnTag1.ActivePeriod.DueTimeUtc.AddWeeks(-4);
            var expectedNextDueTimeUtcReq1OnTag2 = _req1OnTag2.ActivePeriod.DueTimeUtc.AddWeeks(-4);
            var expectedNextDueTimeUtcReq2OnTag2 = _req2OnTag2.ActivePeriod.DueTimeUtc.AddWeeks(-4);
            
            await _dut.Handle(_rescheduleFourWeeksEarlierCommand, default);

            Assert.AreEqual(expectedNextDueTimeUtcReq1OnTag1, _req1OnTag1.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtcReq2OnTag1, _req2OnTag1.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtcReq1OnTag2, _req1OnTag2.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtcReq2OnTag2, _req2OnTag2.NextDueTimeUtc);

            Assert.AreEqual(expectedNextDueTimeUtcReq1OnTag1, _tag1.NextDueTimeUtc);
            Assert.AreEqual(expectedNextDueTimeUtcReq1OnTag2, _tag2.NextDueTimeUtc);
        }

        [TestMethod]
        public async Task HandlingRescheduleCommand_ShouldSave()
        {
            await _dut.Handle(_rescheduleOneWeekLaterCommand, default);

            UnitOfWorkMock.Verify(r => r.SaveChangesAsync(default), Times.Once);
        }
    
        [TestMethod]
        public async Task HandlingRescheduleCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_rescheduleOneWeekLaterCommand, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            // In real life EF Core will create a new RowVersion when save.
            // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
            Assert.AreEqual(_rowVersion1, result.Data.First().RowVersion);
            Assert.AreEqual(_rowVersion1, _tag1.RowVersion.ConvertToString());
        }
    }
}

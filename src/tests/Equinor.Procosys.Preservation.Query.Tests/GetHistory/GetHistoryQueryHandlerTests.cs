using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetHistory;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetHistory
{
    [TestClass]
    public class GetHistoryQueryHandlerTests : ReadOnlyTestsBase
    {
        private Tag _tagWithHistory;
        private Tag _tagWithNoHistory;
        private History _historyVoidTag;
        private History _historyCreateTag;
        private GetHistoryQuery _query;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var project = AddProject(context, "P", "Project description");
                var journey = AddJourneyWithStep(context, "J", "S1", AddMode(context, "M1", false), AddResponsible(context, "R1"));
                var rd = AddRequirementTypeWith1DefWithoutField(context, "Rot", "D", RequirementTypeIcon.Other).RequirementDefinitions.First();

                _tagWithNoHistory = new Tag(TestPlant, TagType.Standard, "TagNo", "Tag description", journey.Steps.First(),
                    new List<TagRequirement> { new TagRequirement(TestPlant, 2, rd) });
                project.AddTag(_tagWithNoHistory);

                _tagWithHistory = new Tag(TestPlant, TagType.Standard, "TagNo1", "Tag description1", journey.Steps.First(),
                    new List<TagRequirement> { new TagRequirement(TestPlant, 2, rd) });
                project.AddTag(_tagWithHistory);

                _historyVoidTag = new History(TestPlant, "D", _tagWithHistory.ObjectGuid, ObjectType.Tag, EventType.TagVoided);
                _historyCreateTag = new History(TestPlant, "D1", _tagWithHistory.ObjectGuid, ObjectType.Tag, EventType.TagCreated);

                context.History.Add(_historyVoidTag);
                context.History.Add(_historyCreateTag);

                context.SaveChangesAsync().Wait();

                _query = new GetHistoryQuery(_tagWithHistory.Id);
            }
        }

        [TestMethod]
        public async Task HandleGetHistoryQuery_ShouldReturnOkResult()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            { 
                var dut = new GetHistoryQueryHandler(context);
                var result = await dut.Handle(_query, default);

                Assert.AreEqual(ResultType.Ok, result.ResultType);
            }
        }

        [TestMethod]
        public async Task HandleGetHistoryQuery_ShouldReturnCorrectHistory_WhenTagHasHistory()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var dut = new GetHistoryQueryHandler(context);
                var result = await dut.Handle(_query, default);

                Assert.AreEqual(2, result.Data.Count);
                AssertHistory(_historyVoidTag, result.Data.Single(t => t.EventType == EventType.TagVoided));
                AssertHistory(_historyCreateTag, result.Data.Single(t => t.EventType == EventType.TagCreated));
            }
        }

        [TestMethod]
        public async Task HandleGetHistoryQuery_ShouldReturnEmptyListOfHistory_WhenTagHasNoHistory()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var dut = new GetHistoryQueryHandler(context);
                var result = await dut.Handle(new GetHistoryQuery(_tagWithNoHistory.Id), default);

                Assert.AreEqual(0, result.Data.Count);
            }
        }

        private void AssertHistory(History expected, HistoryDto actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.PreservationRecordId, actual.PreservationRecordId);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.EventType, actual.EventType);
            Assert.AreEqual(expected.CreatedById, actual.CreatedBy.Id);
            Assert.AreEqual(expected.CreatedAtUtc, actual.CreatedAtUtc);
        }
    }
}

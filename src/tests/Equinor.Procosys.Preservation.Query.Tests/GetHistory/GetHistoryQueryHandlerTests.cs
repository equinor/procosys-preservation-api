using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetHistory;
using Equinor.Procosys.Preservation.Test.Common;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;


namespace Equinor.Procosys.Preservation.Query.Tests.GetHistory
{
    [TestClass]
    public class GetHistoryQueryHandlerTests : ReadOnlyTestsBase
    {
        private const int _tagIdWithNoHistory = 2;
        
        private Tag _tagWithHistory;
        private Tag _tagWithNoHistory;
        private History _historyVoidTag;
        private History _historyCreateTag;
        private GetHistoryQuery _query;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var step = new Step(TestPlant, "T", new Mode(TestPlant, "TI", false), new Responsible(TestPlant, "C", "TITL"));
                var requirementDefinition = new RequirementDefinition(TestPlant, "T", 3, RequirementUsage.ForAll, 1);
                var requirement = new TagRequirement(TestPlant, 2, requirementDefinition);

                _tagWithNoHistory = new Tag(TestPlant, TagType.Standard, "T", "D", step, new List<TagRequirement> { requirement });
                _tagWithNoHistory.SetProtectedIdForTesting(_tagIdWithNoHistory);
                context.Tags.Add(_tagWithNoHistory);

                _tagWithHistory = new Tag(TestPlant, TagType.Standard, "T1", "D1", step, new List<TagRequirement> { requirement });
                context.Tags.Add(_tagWithHistory);

                _historyVoidTag = new History(TestPlant, "D", _tagWithHistory.ObjectGuid, ObjectType.Tag, EventType.VoidTag);
                _historyCreateTag = new History(TestPlant, "D1", _tagWithHistory.ObjectGuid, ObjectType.Tag, EventType.CreateTag);

                _query = new GetHistoryQuery(_tagWithHistory.Id);

                context.History.Add(_historyVoidTag);
                context.History.Add(_historyCreateTag);

                context.SaveChangesAsync().Wait();
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
                AssertHistory(_historyVoidTag, result.Data.Single(t => t.EventType == EventType.VoidTag));
                AssertHistory(_historyCreateTag, result.Data.Single(t => t.EventType == EventType.CreateTag));
            }
        }

        [TestMethod]
        public async Task HandleGetHistoryQuery_ShouldReturnEmptyListOfHistory_WhenTagHasNoHistory()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var dut = new GetHistoryQueryHandler(context);
                var result = await dut.Handle(new GetHistoryQuery(_tagIdWithNoHistory), default);

                Assert.AreEqual(0, result.Data.Count);
            }
        }

        private void AssertHistory(History expected, HistoryDto actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.PreservationRecordId, actual.PreservationRecordId);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.EventType, actual.EventType);
            Assert.AreEqual(expected.CreatedById, actual.CreatedById);
            Assert.AreEqual(expected.CreatedAtUtc, actual.CreatedAtUtc);
        }
    }
}

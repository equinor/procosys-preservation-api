using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.HistoryAggregate;
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
        private const int _tagIdWithNoHistory = 1;
        private const int _tagIdWithHistory = 2;

        private History _historyVoidTag;
        private History _historyCreateTag;
        private GetHistoryQuery _query;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _query = new GetHistoryQuery(_tagIdWithHistory);
                _historyVoidTag = new History(TestPlant, "D", _tagIdWithHistory, ObjectType.Tag, EventType.VoidTag);
                _historyCreateTag = new History(TestPlant, "D1", _tagIdWithHistory, ObjectType.Tag, EventType.CreateTag);

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

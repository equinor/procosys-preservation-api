using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetTagActionDetails;
using Equinor.Procosys.Preservation.Test.Common;
using Equinor.Procosys.Preservation.Test.Common.ExtentionMethods;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Query.Tests.GetActionDetails
{
    [TestClass]
    public class GetActionDetailsQueryHandlerTests : ReadOnlyTestsBase
    {
        private int _tagId;
        private int _openActionId;
        private int _closedActionId;
        private Action _openAction;
        private Action _closedAction;
        private static DateTime _utcNow = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static DateTime _dueUtc = _utcNow.AddDays(30);
        private TestDataSet _testDataSet;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _testDataSet = AddTestDataSet(context);

                var tag = _testDataSet.Project1.Tags.First();

                _openAction = new Action(TestPlant, "Open", "Desc1", _dueUtc);
                _openAction.SetProtectedRowVersionForTesting(123);
                tag.AddAction(_openAction);
                _closedAction = new Action(TestPlant, "Closed", "Desc2", _dueUtc);
                _closedAction.SetProtectedRowVersionForTesting(456);
                _closedAction.Close(_utcNow, _testDataSet.CurrentUser);
                tag.AddAction(_closedAction);
                context.SaveChangesAsync().Wait();

                _tagId = tag.Id;
                _openActionId = _openAction.Id;
                _closedActionId = _closedAction.Id;
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsClosedAction()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetActionDetailsQuery(_tagId, _closedActionId);
                var dut = new GetActionDetailsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                
                var actionDetailDto = result.Data;

                AssertClosedAction(actionDetailDto, _closedAction, _testDataSet.CurrentUser);
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsOpenAction()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetActionDetailsQuery(_tagId, _openActionId);
                var dut = new GetActionDetailsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                
                AssertAction(result.Data, _openAction, false);
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsNotFound_IfTagIsNotFound()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetActionDetailsQuery(0, _closedActionId);
                var dut = new GetActionDetailsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
                Assert.IsNull(result.Data);
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsNotFound_IfActionIsNotFound()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetActionDetailsQuery(_tagId, 0);
                var dut = new GetActionDetailsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
                Assert.IsNull(result.Data);
            }
        }

        private void AssertClosedAction(ActionDetailsDto actionDetailsDto, Action action, Person closer)
        {
            AssertAction(actionDetailsDto, action, true);
            Assert.AreEqual(closer.Id, actionDetailsDto.ClosedBy.Id);
            Assert.AreEqual(closer.FirstName, actionDetailsDto.ClosedBy.FirstName);
            Assert.AreEqual(closer.LastName, actionDetailsDto.ClosedBy.LastName);
        }

        private static void AssertAction(ActionDetailsDto actionDetailsDto, Action action, bool isClosed)
        {
            Assert.AreEqual(action.Id, actionDetailsDto.Id);
            Assert.AreEqual(action.Title, actionDetailsDto.Title);
            Assert.AreEqual(action.Description, actionDetailsDto.Description);
            Assert.AreEqual(action.IsClosed, actionDetailsDto.IsClosed);
            Assert.AreEqual(action.DueTimeUtc, actionDetailsDto.DueTimeUtc);
            Assert.AreEqual(action.ClosedAtUtc, actionDetailsDto.ClosedAtUtc);
            Assert.AreEqual(isClosed, actionDetailsDto.IsClosed);
        }
    }
}

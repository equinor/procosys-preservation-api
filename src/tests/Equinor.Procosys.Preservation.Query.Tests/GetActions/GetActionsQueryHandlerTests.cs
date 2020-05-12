using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetActions;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Query.Tests.GetActions
{
    [TestClass]
    public class GetActionsQueryHandlerTests : ReadOnlyTestsBase
    {
        private int _tagId;
        private int _openActionId;
        private int _closedActionId;
        private Action _openAction;
        private Action _closedAction;
        private readonly DateTime _utcNow = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private TestDataSet _testDataSet;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _testDataSet = AddTestDataSet(context);

                var tag = _testDataSet.Project1.Tags.First();

                _openAction = new Action(TestPlant, "Open", "Desc1", _utcNow);
                tag.AddAction(_openAction);
                _closedAction = new Action(TestPlant, "Closed", "Desc2", _utcNow);
                _closedAction.Close(_utcNow, _testDataSet.CurrentUser);
                tag.AddAction(_closedAction);

                context.SaveChangesAsync().Wait();

                _tagId = tag.Id;
                _openActionId = _openAction.Id;
                _closedActionId = _closedAction.Id;
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsActions()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetActionsQuery(_tagId);
                var dut = new GetActionsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                
                var actionDtos = result.Data;
                Assert.AreEqual(2, actionDtos.Count);

                AssertAction(actionDtos.Single(a => a.Id == _openActionId), _openAction);
                AssertAction(actionDtos.Single(a => a.Id == _closedActionId), _closedAction);
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsNotFound_IfTagIsNotFound()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetActionsQuery(0);
                var dut = new GetActionsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
                Assert.IsNull(result.Data);
            }
        }

        private void AssertAction(ActionDto actionDto, Action action)
        {
            Assert.AreEqual(action.Id, actionDto.Id);
            Assert.AreEqual(action.Title, actionDto.Title);
            Assert.AreEqual(action.IsClosed, actionDto.IsClosed);
            Assert.AreEqual(action.DueTimeUtc, actionDto.DueTimeUtc);
        }
    }
}

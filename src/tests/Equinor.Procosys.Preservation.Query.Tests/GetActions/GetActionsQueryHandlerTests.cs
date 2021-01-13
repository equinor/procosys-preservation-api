using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetActions;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Query.Tests.GetActions
{
    [TestClass]
    public class GetActionsQueryHandlerTests : ReadOnlyTestsBase
    {
        private int _tagId;
        private int _tag2Id;
        private int _openActionId;
        private int _closedActionId;
        private Action _openAction;
        private Action _closedAction;
        private Action _openActionWithDueTime;
        private Action _openActionWithEarliestDueTime;
        private Action _openActionWithoutDueTime;
        private Action _closedActionWithDueTime;
        private Action _closedActionWithEarliestDueTime;
        private Action _closedActionWithoutDueTime;
        private readonly DateTime _utcNow = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly DateTime _dueTimeUtc = DateTime.UtcNow;
        private TestDataSet _testDataSet;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _testDataSet = AddTestDataSet(context);

                var tag = _testDataSet.Project1.Tags.First();
                var attachment = new ActionAttachment(TestPlant, Guid.NewGuid(), "FileA");

                _openAction = new Action(TestPlant, "Open", "Desc1", _utcNow);
                tag.AddAction(_openAction);
                _closedAction = new Action(TestPlant, "Closed", "Desc2", _utcNow);
                _closedAction.Close(_utcNow, _testDataSet.CurrentUser);
                tag.AddAction(_closedAction);

                var tag2 = _testDataSet.Project1.Tags.Last();

                var personMock = new Mock<Person>();
                personMock.SetupGet(p => p.Id).Returns(7);

                _openActionWithEarliestDueTime = new Action(TestPlant, "OpenWithEarliestDueTime", "D2", _dueTimeUtc);
                _openActionWithDueTime = new Action(TestPlant, "OpenWithDueTime", "D1", _dueTimeUtc);
                _openActionWithoutDueTime = new Action(TestPlant, "OpenWithoutDueTime", "D3", _dueTimeUtc);
                _openActionWithoutDueTime.SetDueTime(null);
                _openAction.AddAttachment(attachment);
                _closedActionWithEarliestDueTime = new Action(TestPlant, "ClosedWithEarliestDueTime", "D5", _dueTimeUtc);
                _closedActionWithEarliestDueTime.Close(_utcNow, personMock.Object);
                _closedActionWithDueTime = new Action(TestPlant, "ClosedWithDueTime", "D4", _dueTimeUtc);
                _closedActionWithDueTime.Close(_utcNow, personMock.Object);
                _closedActionWithoutDueTime = new Action(TestPlant, "ClosedWithoutDueTime", "D6", _dueTimeUtc);
                _closedActionWithoutDueTime.SetDueTime(null);
                _closedActionWithoutDueTime.Close(_utcNow, personMock.Object);

                tag2.AddAction(_openActionWithoutDueTime);
                tag2.AddAction(_closedActionWithoutDueTime);
                tag2.AddAction(_openActionWithDueTime);
                tag2.AddAction(_closedActionWithDueTime);
                tag2.AddAction(_openActionWithEarliestDueTime);
                tag2.AddAction(_closedActionWithEarliestDueTime);

                context.SaveChangesAsync().Wait();

                _tagId = tag.Id;
                _tag2Id = tag2.Id;
                _openActionId = _openAction.Id;
                _closedActionId = _closedAction.Id;
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnActions()
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
        public async Task Handler_ShouldReturnNotFound_IfTagIsNotFound()
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

        [TestMethod]
        public async Task Handler_ShouldReturnActionsInCorrectOrder()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var query = new GetActionsQuery(_tag2Id);
                var dut = new GetActionsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.AreEqual(_openActionWithEarliestDueTime.Id, result.Data[0].Id);
                Assert.AreEqual(_openActionWithDueTime.Id, result.Data[1].Id);
                Assert.AreEqual(_openActionWithoutDueTime.Id, result.Data[2].Id);
                Assert.AreEqual(_closedActionWithEarliestDueTime.Id, result.Data[3].Id);
                Assert.AreEqual(_closedActionWithDueTime.Id, result.Data[4].Id);
                Assert.AreEqual(_closedActionWithoutDueTime.Id, result.Data[5].Id);
            }
        }

        private void AssertAction(ActionDto actionDto, Action action)
        {
            Assert.AreEqual(action.Id, actionDto.Id);
            Assert.AreEqual(action.IsOverDue(), actionDto.IsOverDue);
            Assert.AreEqual(action.Title, actionDto.Title);
            Assert.AreEqual(action.IsClosed, actionDto.IsClosed);
            Assert.AreEqual(action.DueTimeUtc, actionDto.DueTimeUtc);
            Assert.AreEqual(action.Attachments.Count, actionDto.AttachmentCount);
        }
    }
}

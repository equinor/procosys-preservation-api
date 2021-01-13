using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetActionDetails;
using Equinor.Procosys.Preservation.Test.Common;
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
                var attachment = new ActionAttachment(TestPlant, Guid.NewGuid(), "FileA");

                _openAction = new Action(TestPlant, "Open", "Desc1", _dueUtc);
                _openAction.AddAttachment(attachment);
                tag.AddAction(_openAction);

                _closedAction = new Action(TestPlant, "Closed", "Desc2", _dueUtc);
                _closedAction.Close(_utcNow, _testDataSet.CurrentUser);
                tag.AddAction(_closedAction);
                context.SaveChangesAsync().Wait();

                _tagId = tag.Id;
                _openActionId = _openAction.Id;
                _closedActionId = _closedAction.Id;
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnClosedAction()
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
                AssertNotModifiedAction(actionDetailDto);
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnModifiedAction()
        {
            DateTime? modifiedTime;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var openAction = context.Actions.Single(a => a.Id == _openActionId);
                openAction.Title = "Changed title";
                _timeProvider.Elapse(new TimeSpan(1, 1, 1, 1));
                context.SaveChangesAsync().Wait();
                modifiedTime = openAction.ModifiedAtUtc;
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetActionDetailsQuery(_tagId, _openActionId);
                var dut = new GetActionDetailsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                
                var actionDetailDto = result.Data;

                AssertModifiedAction(actionDetailDto, _testDataSet.CurrentUser, modifiedTime);
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnOpenAction()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetActionDetailsQuery(_tagId, _openActionId);
                var dut = new GetActionDetailsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);
                
                var actionDetailDto = result.Data;
                AssertAction(actionDetailDto, _openAction, false);
                AssertNotModifiedAction(actionDetailDto);
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnNotFound_WhenTagNotFound()
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
        public async Task Handler_ShouldReturnNotFound_IfActionIsNotFound()
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
            Assert.AreEqual(action.Attachments.Count, actionDetailsDto.AttachmentCount);
        }

        private void AssertModifiedAction(ActionDetailsDto actionDetailsDto, Person modifier, DateTime? modifiedAt)
        {
            Assert.IsNotNull(actionDetailsDto.ModifiedBy);
            Assert.AreEqual(modifier.Id, actionDetailsDto.ModifiedBy.Id);
            Assert.AreEqual(modifiedAt, actionDetailsDto.ModifiedAtUtc);
        }

        private void AssertNotModifiedAction(ActionDetailsDto actionDetailsDto)
        {
            if (actionDetailsDto == null)
            {
                throw new ArgumentNullException(nameof(actionDetailsDto));
            }

            Assert.IsNull(actionDetailsDto.ModifiedBy);
            Assert.IsFalse(actionDetailsDto.ModifiedAtUtc.HasValue);
        }
    }
}

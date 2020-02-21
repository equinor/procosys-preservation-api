using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetTagActions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceResult;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagActions
{
    [TestClass]
    public class GetTagActionsQueryHandlerTests : ReadOnlyTestsBase
    {
        private int _tagId;
        private int _openActionId;
        private int _closedActionId;
        private Action _openAction;
        private Action _closedAction;
        private DateTime _utcNow = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private Person _creator;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
            {
                var journey = AddJourneyWithStep(context, "J1", AddMode(context, "M1"), AddResponsible(context, "R1"));

                var reqType = AddRequirementTypeWith1DefWithoutField(context, "T1", "D1");

                _creator = AddPerson(context, "Ole", "Lukkøye");

                var tag = new Tag(_schema, TagType.Standard, "", "", "", "", "", "", "", "", "", "",
                    journey.Steps.ElementAt(0),
                    new List<Requirement>
                    {
                        new Requirement(_schema, 2, reqType.RequirementDefinitions.ElementAt(0))
                    });

                context.Tags.Add(tag);

                _openAction = new Action(_schema, "Open", "Desc1", _utcNow, _creator, _utcNow);
                tag.AddAction(_openAction);
                _closedAction = new Action(_schema, "Closed", "Desc2", _utcNow, _creator, _utcNow);
                _closedAction.Close(_utcNow, _creator);
                tag.AddAction(_closedAction);

                context.SaveChanges();

                _tagId = tag.Id;
                _openActionId = _openAction.Id;
                _closedActionId = _closedAction.Id;
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsActions()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
            {
                var query = new GetTagActionsQuery(_tagId);
                var dut = new GetTagActionsQueryHandler(context);

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
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
            {
                var query = new GetTagActionsQuery(0);
                var dut = new GetTagActionsQueryHandler(context);

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

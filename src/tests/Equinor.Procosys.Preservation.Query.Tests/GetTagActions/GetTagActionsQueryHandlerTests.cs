using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetTagActions;
using Equinor.Procosys.Preservation.Query.GetTagDetails;
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
        private Action _action1;
        private Action _action2;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
            {
                var journey = AddJourneyWithStep(context, "J1", AddMode(context, "M1"), AddResponsible(context, "R1"));

                var reqType = AddRequirementTypeWith1DefWithoutField(context, "T1", "D1");

                var tag = new Tag(_schema, TagType.Standard, "", "", "", "", "", "", "", "", "", "",
                    journey.Steps.ElementAt(0),
                    new List<Requirement>
                    {
                        new Requirement(_schema, 2, reqType.RequirementDefinitions.ElementAt(0))
                    });

                context.Tags.Add(tag);

                _action1 = new Action(_schema, "Desc1", DateTime.UtcNow);
                tag.AddAction(_action1);
                _action2 = new Action(_schema, "Desc2", DateTime.UtcNow);
                tag.AddAction(_action2);
                context.SaveChanges();

                _tagId = tag.Id;
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
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsNotFound_IfTagIsNotFound()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
            {
                var query = new GetTagDetailsQuery(0);
                var dut = new GetTagDetailsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
                Assert.IsNull(result.Data);
            }
        }
    }
}

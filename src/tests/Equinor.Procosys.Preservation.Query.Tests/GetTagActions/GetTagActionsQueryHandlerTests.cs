using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Domain.Events;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Query.GetTagActions;
using Equinor.Procosys.Preservation.Query.GetTagDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;
using Action = Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagActions
{
    [TestClass]
    public class GetTagActionsQueryHandlerTests
    {
        readonly string _schema = "PCS$TEST";
        Mock<IEventDispatcher> _eventDispatcherMock;
        Mock<IPlantProvider> _plantProviderMock;
        private int _tagId;
        private Action _action1;
        private Action _action2;

        [TestInitialize]
        public void Setup()
        {
            _eventDispatcherMock = new Mock<IEventDispatcher>();
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock.SetupGet(x => x.Plant).Returns(_schema);
        }

        [TestMethod]
        public async Task Handler_ReturnsActions()
        {
            var dbContextOptions = new DbContextOptionsBuilder<PreservationContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                 .Options;

            using (var context = new PreservationContext(dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
            {
                var responsible = new Responsible(_schema, "Responsible");
                context.Responsibles.Add(responsible);
                context.SaveChanges();

                var mode = new Mode(_schema, "Mode");
                context.Modes.Add(mode);
                context.SaveChanges();

                var step = new Step(_schema, mode, context.Responsibles.First());
                var journey = new Journey(_schema, "Journey");
                journey.AddStep(step);
                context.Journeys.Add(journey);
                context.SaveChanges();

                var requirementType = new RequirementType(_schema, "Code", "Title", 0);
                context.RequirementTypes.Add(requirementType);
                context.SaveChanges();

                var requirementDefinitionWithoutField = new RequirementDefinition(_schema, "Title", 2, 1);
                requirementType.AddRequirementDefinition(requirementDefinitionWithoutField);
                context.SaveChanges();

                var requirementWithoutField = new Requirement(_schema, 2, requirementDefinitionWithoutField);

                var tag = new Tag(_schema, TagType.Standard, "", "", "", "",
                    "", "", "", "", "", "",
                    step,
                    new List<Requirement>
                    {
                        requirementWithoutField
                    });
                context.Tags.Add(tag);

                _action1 = new Action(_schema, "Desc1", DateTime.UtcNow);
                tag.AddAction(_action1);
                _action2 = new Action(_schema, "Desc2", DateTime.UtcNow);
                tag.AddAction(_action2);
                context.SaveChanges();

                _tagId = tag.Id;
            }

            using (var context = new PreservationContext(dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
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
            var dbContextOptions = new DbContextOptionsBuilder<PreservationContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                 .Options;

            using var context = new PreservationContext(dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object);
            var query = new GetTagDetailsQuery(1);
            var dut = new GetTagDetailsQueryHandler(context);

            var result = await dut.Handle(query, default);

            Assert.IsNotNull(result);
            Assert.AreEqual(ResultType.NotFound, result.ResultType);
            Assert.IsNull(result.Data);
        }
    }
}

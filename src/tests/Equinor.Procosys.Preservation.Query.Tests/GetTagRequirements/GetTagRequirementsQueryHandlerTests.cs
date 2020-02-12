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
using Equinor.Procosys.Preservation.Query.GetTagRequirements;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagRequirements
{
    [TestClass]
    public class GetTagRequirementsQueryHandlerTests
    {
        const string _schema = "PCS$TEST";
        private DbContextOptions<PreservationContext> _dbContextOptions;
        private Mock<IEventDispatcher> _eventDispatcherMock;
        private Mock<IPlantProvider> _plantProviderMock;
        private DateTime _startedAtUtc = new DateTime(2020, 2, 1, 0, 0, 0, DateTimeKind.Utc);

        [TestInitialize]
        public void Setup()
        {
            _eventDispatcherMock = new Mock<IEventDispatcher>();
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock.SetupGet(x => x.Plant).Returns(_schema);
            
            _dbContextOptions = SetupNewDatabase();
        }

        private DbContextOptions<PreservationContext> SetupNewDatabase()
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

                var requirementDefinitionWithoutField = new RequirementDefinition(_schema, "WithoutField", 2, 1);
                context.RequirementDefinitions.Add(requirementDefinitionWithoutField);

                var requirementDefinitionWithInfo = new RequirementDefinition(_schema, "WithInfo", 2, 1);
                var infoField = new Field(_schema, "Label for Info", FieldType.Info, 0);
                requirementDefinitionWithInfo.AddField(infoField);
                context.RequirementDefinitions.Add(requirementDefinitionWithInfo);

                var requirementDefinitionWithCheckBox = new RequirementDefinition(_schema, "WithCheckBox", 2, 1);
                var cbField = new Field(_schema, "Label for CheckBox", FieldType.CheckBox, 10);
                requirementDefinitionWithCheckBox.AddField(cbField);
                context.RequirementDefinitions.Add(requirementDefinitionWithCheckBox);

                var requirementDefinitionWithThreeNumberShowPrev =
                    new RequirementDefinition(_schema, "WithNumber previous", 2, 1);
                var numberFieldPrev1 = new Field(_schema, "Label for number - third", FieldType.Number, 15, "unit", true);
                var numberFieldPrev2 = new Field(_schema, "Label for number - first", FieldType.Number, 2, "unit", true);
                var numberFieldPrev3 = new Field(_schema, "Label for number - second", FieldType.Number, 10, "unit", true);
                requirementDefinitionWithThreeNumberShowPrev.AddField(numberFieldPrev1);
                requirementDefinitionWithThreeNumberShowPrev.AddField(numberFieldPrev2);
                requirementDefinitionWithThreeNumberShowPrev.AddField(numberFieldPrev3);
                context.RequirementDefinitions.Add(requirementDefinitionWithThreeNumberShowPrev);

                var requirementDefinitionWithNumberNoPrev = new RequirementDefinition(_schema, "WithNumber no previous", 2, 1);
                var numberFieldNoPrev = new Field(_schema, "Label for number", FieldType.Number, 10, "unit", false);
                requirementDefinitionWithNumberNoPrev.AddField(numberFieldNoPrev);
                context.RequirementDefinitions.Add(requirementDefinitionWithNumberNoPrev);

                context.SaveChanges();

                var tag = new Tag(_schema,
                    "TagNo",
                    "Description",
                    "AreaCode",
                    "Calloff",
                    "DisciplineCode",
                    "McPkgNo",
                    "CommPkgNo",
                    "PurchaseOrderNo",
                    "Remark",
                    "TagFunctionCode",
                    step,
                    new List<Requirement>
                    {
                        new Requirement(_schema, 2, requirementDefinitionWithoutField),
                        new Requirement(_schema, 2, requirementDefinitionWithInfo),
                        new Requirement(_schema, 1, requirementDefinitionWithCheckBox),
                        new Requirement(_schema, 12, requirementDefinitionWithNumberNoPrev),
                        new Requirement(_schema, 4, requirementDefinitionWithThreeNumberShowPrev)
                    });
                context.Tags.Add(tag);
                context.SaveChanges();
            }

            return dbContextOptions;
        }

        [TestMethod]
        public async Task Handler_ShouldReturnsTagRequirements_NoDueDates_BeforePreservationStarted()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
            {
                var query = new GetTagRequirementsQuery(1);
                var dut = new GetTagRequirementsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);

                foreach (var requirement in result.Data)
                {
                    Assert.IsFalse(requirement.NextDueTimeUtc.HasValue);
                    Assert.IsNull(requirement.NextDueAsYearAndWeek);
                    Assert.IsFalse(requirement.ReadyToBePreserved);
                }
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnsTagRequirements_AfterPreservationStarted()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcherMock.Object,
                _plantProviderMock.Object))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single();
                tag.StartPreservation(_startedAtUtc);
                context.Update(tag);
                context.SaveChanges();

            }
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
            {
                var query = new GetTagRequirementsQuery(1);
                var dut = new GetTagRequirementsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);

                foreach (var requirement in result.Data)
                {
                    Assert.IsTrue(requirement.NextDueTimeUtc.HasValue);
                    Assert.IsNotNull(requirement.NextDueTimeUtc.Value);
                    Assert.IsNotNull(requirement.NextDueAsYearAndWeek);
                    //Assert.IsFalse(requirement.ReadyToBePreserved);
                }
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsNotFound_IfTagIsNotFound()
        {
            var dbContextOptions = new DbContextOptionsBuilder<PreservationContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                 .Options;

            using var context = new PreservationContext(dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object);
            var query = new GetTagRequirementsQuery(1);
            var dut = new GetTagRequirementsQueryHandler(context);

            var result = await dut.Handle(query, default);

            Assert.IsNotNull(result);
            Assert.AreEqual(ResultType.NotFound, result.ResultType);
            Assert.IsNull(result.Data);
        }
    }
}

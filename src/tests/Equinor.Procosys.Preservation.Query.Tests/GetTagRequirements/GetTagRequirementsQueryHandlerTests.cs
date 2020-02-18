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
        const string _unit = "unit";
        const string _requirementType1Code = "Code1";
        const string _requirementType1Title = "Title1";
        const string _requirementType2Code = "Code2";
        const string _requirementType2Title = "Title2";
        const string _requirementDefinitionWithoutFieldTitle = "Without fields";
        const string _requirementDefinitionWithOneInfoTitle = "With 1 info";
        const string _requirementDefinitionWithTwoCheckBoxesTitle = "With 2 checkboxes";
        const string _requirementDefinitionWithThreeNumberShowPrevTitle = "With 3 number with previous";
        const string _requirementDefinitionWithOneNumberNoPrevTitle = "With 1 number no previous";
        private DbContextOptions<PreservationContext> _dbContextOptions;
        private Mock<IEventDispatcher> _eventDispatcherMock;
        private Mock<IPlantProvider> _plantProviderMock;
        private Mock<ITimeService> _timeServiceMock;
        private DateTime _startedAtUtc;
        private DateTime _currentUtc;

        private int _requirementWithoutFieldId;
        private int _requirementWithOneInfoId;
        private int _requirementWithTwoCheckBoxesId;
        private int _requirementWithThreeNumberShowPrevId;
        private int _requirementWithOneNumberNoPrevId;

        private int _tagId;
        private int _requestTimeAfterPreservationStartedInWeeks = 1;
        private int _interval = 8;
        private int _firstCbFieldId;
        private int _secondCbFieldId;
        private int _firstNumberFieldId;
        private int _secondNumberFieldId;
        private int _thirdNumberFieldId;
        private int _requirementDefinitionWithTwoCheckBoxesId;
        private int _requirementDefinitionWithThreeNumberShowPrevId;

        [TestInitialize]
        public void Setup()
        {
            _startedAtUtc = new DateTime(2020, 2, 1, 0, 0, 0, DateTimeKind.Utc);
            _currentUtc = _startedAtUtc.AddWeeks(_requestTimeAfterPreservationStartedInWeeks);

            _eventDispatcherMock = new Mock<IEventDispatcher>();
            _plantProviderMock = new Mock<IPlantProvider>();
            _timeServiceMock = new Mock<ITimeService>();
            _timeServiceMock.Setup(t => t.GetCurrentTimeUtc()).Returns(_currentUtc);
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

                var requirementType1 = new RequirementType(_schema, _requirementType1Code, _requirementType1Title, 0);
                context.RequirementTypes.Add(requirementType1);
                var requirementType2 = new RequirementType(_schema, _requirementType2Code, _requirementType2Title, 0);
                context.RequirementTypes.Add(requirementType2);
                context.SaveChanges();

                var requirementDefinitionWithoutField = new RequirementDefinition(_schema, _requirementDefinitionWithoutFieldTitle, 2, 1);
                requirementType1.AddRequirementDefinition(requirementDefinitionWithoutField);

                var requirementDefinitionWithOneInfo = new RequirementDefinition(_schema, _requirementDefinitionWithOneInfoTitle, 2, 1);
                var infoField = new Field(_schema, "Label for Info", FieldType.Info, 0);
                requirementDefinitionWithOneInfo.AddField(infoField);
                requirementType1.AddRequirementDefinition(requirementDefinitionWithOneInfo);

                var requirementDefinitionWithTwoCheckBoxes = new RequirementDefinition(_schema, _requirementDefinitionWithTwoCheckBoxesTitle, 2, 1);
                var cbField1 = new Field(_schema, "Label for checkBox - second", FieldType.CheckBox, 10);
                var cbField2 = new Field(_schema, "Label for checkBox - first", FieldType.CheckBox, 2);
                requirementDefinitionWithTwoCheckBoxes.AddField(cbField1);
                requirementDefinitionWithTwoCheckBoxes.AddField(cbField2);
                requirementType2.AddRequirementDefinition(requirementDefinitionWithTwoCheckBoxes);

                var requirementDefinitionWithThreeNumberShowPrev = new RequirementDefinition(_schema, _requirementDefinitionWithThreeNumberShowPrevTitle, 2, 1);
                var numberFieldPrev1 = new Field(_schema, "Label for number - third", FieldType.Number, 15, _unit, true);
                var numberFieldPrev2 = new Field(_schema, "Label for number - first", FieldType.Number, 2, _unit, true);
                var numberFieldPrev3 = new Field(_schema, "Label for number - second", FieldType.Number, 10, _unit, true);
                requirementDefinitionWithThreeNumberShowPrev.AddField(numberFieldPrev1);
                requirementDefinitionWithThreeNumberShowPrev.AddField(numberFieldPrev2);
                requirementDefinitionWithThreeNumberShowPrev.AddField(numberFieldPrev3);
                requirementType2.AddRequirementDefinition(requirementDefinitionWithThreeNumberShowPrev);

                var requirementDefinitionWithOneNumberNoPrev = new RequirementDefinition(_schema, _requirementDefinitionWithOneNumberNoPrevTitle, 2, 1);
                var numberFieldNoPrev = new Field(_schema, "Label for number", FieldType.Number, 10, _unit, false);
                requirementDefinitionWithOneNumberNoPrev.AddField(numberFieldNoPrev);
                requirementType2.AddRequirementDefinition(requirementDefinitionWithOneNumberNoPrev);

                context.SaveChanges();

                var requirementWithoutField = new Requirement(_schema, _interval, requirementDefinitionWithoutField);
                var requirementWithOneInfo = new Requirement(_schema, _interval, requirementDefinitionWithOneInfo);
                var requirementWithTwoCheckBoxes = new Requirement(_schema, _interval, requirementDefinitionWithTwoCheckBoxes);
                var requirementWithOneNumberNoPrev = new Requirement(_schema, _interval, requirementDefinitionWithOneNumberNoPrev);
                var requirementWithThreeNumberShowPrev = new Requirement(_schema, _interval, requirementDefinitionWithThreeNumberShowPrev);
                
                var tag = new Tag(_schema,
                    TagType.Standard, 
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
                        requirementWithoutField,
                        requirementWithOneInfo,
                        requirementWithTwoCheckBoxes,
                        requirementWithOneNumberNoPrev,
                        requirementWithThreeNumberShowPrev
                    });
                context.Tags.Add(tag);
                context.SaveChanges();

                _tagId = tag.Id;

                _requirementDefinitionWithTwoCheckBoxesId = requirementDefinitionWithTwoCheckBoxes.Id;
                _requirementDefinitionWithThreeNumberShowPrevId = requirementDefinitionWithThreeNumberShowPrev.Id;

                _requirementWithoutFieldId = requirementWithoutField.Id;
                _requirementWithOneInfoId = requirementWithOneInfo.Id;
                _requirementWithTwoCheckBoxesId = requirementWithTwoCheckBoxes.Id;
                _requirementWithThreeNumberShowPrevId = requirementWithThreeNumberShowPrev.Id;
                _requirementWithOneNumberNoPrevId = requirementWithOneNumberNoPrev.Id;

                _firstCbFieldId = cbField2.Id;
                _secondCbFieldId = cbField1.Id;

                _firstNumberFieldId = numberFieldPrev2.Id;
                _secondNumberFieldId = numberFieldPrev3.Id;
                _thirdNumberFieldId = numberFieldPrev1.Id;
            }

            return dbContextOptions;
        }

        [TestMethod]
        public async Task Handler_ShouldReturnsTagRequirements_NoDueDates_BeforePreservationStarted()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
            {
                var query = new GetTagRequirementsQuery(_tagId);
                var dut = new GetTagRequirementsQueryHandler(context, _timeServiceMock.Object);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);

                foreach (var requirement in result.Data)
                {
                    Assert.IsFalse(requirement.NextDueTimeUtc.HasValue);
                    Assert.IsNull(requirement.NextDueAsYearAndWeek);
                    Assert.IsFalse(requirement.ReadyToBePreserved);
                    Assert.AreEqual(_interval, requirement.IntervalWeeks);
                    Assert.IsNull(requirement.NextDueWeeks);
                }

                AssertRequirements(result.Data);
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
                context.SaveChanges();
            }

            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
            {
                var query = new GetTagRequirementsQuery(_tagId);
                var dut = new GetTagRequirementsQueryHandler(context, _timeServiceMock.Object);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);

                foreach (var requirement in result.Data)
                {
                    Assert.IsTrue(requirement.NextDueTimeUtc.HasValue);
                    Assert.IsNotNull(requirement.NextDueTimeUtc.Value);
                    Assert.IsNotNull(requirement.NextDueAsYearAndWeek);
                    Assert.AreEqual(_interval, requirement.IntervalWeeks);
                    Assert.AreEqual(_interval-_requestTimeAfterPreservationStartedInWeeks, requirement.NextDueWeeks);
                }
            
                AssertRequirements(result.Data);
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnsTagRequirementsWithCheckBoxField_AfterRecordingCheckBoxField()
        {
            var fieldId = _firstCbFieldId;

            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcherMock.Object,
                _plantProviderMock.Object))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single();
                tag.StartPreservation(_startedAtUtc);
                context.SaveChanges();

                var requirementDefinition = context.RequirementDefinitions.Include(rd => rd.Fields)
                    .Single(rd => rd.Id == _requirementDefinitionWithTwoCheckBoxesId);
                var requirement = context.Requirements.Single(r => r.Id == _requirementWithTwoCheckBoxesId);
                requirement.RecordValues(
                    new Dictionary<int, string> {{fieldId, "true"}},
                    "",
                    requirementDefinition);
                context.SaveChanges();
            }

            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
            {
                var query = new GetTagRequirementsQuery(_tagId);
                var dut = new GetTagRequirementsQueryHandler(context, _timeServiceMock.Object);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);

                var requirementWithTwoCheckBoxes = result.Data.Single(r => r.Id == _requirementWithTwoCheckBoxesId);
                Assert.AreEqual(2, requirementWithTwoCheckBoxes.Fields.Count);

                var field = requirementWithTwoCheckBoxes.Fields.Single(f => f.Id == fieldId);
                Assert.IsNotNull(field);
                Assert.IsNotNull(field.CurrentValue);
                Assert.IsInstanceOfType(field.CurrentValue, typeof(CheckBoxDto));
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnsTagRequirementsWithNumbers_AfterRecordingNumberFields()
        {
            var fieldWithNaId = _thirdNumberFieldId;
            var fieldWithDoubleId = _secondNumberFieldId;
            var number = 1282.91;
            var numberAsString = number.ToString("F2");

            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcherMock.Object,
                _plantProviderMock.Object))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single();
                tag.StartPreservation(_startedAtUtc);
                context.SaveChanges();

                var requirementDefinition = context.RequirementDefinitions.Include(rd => rd.Fields)
                    .Single(rd => rd.Id == _requirementDefinitionWithThreeNumberShowPrevId);
                var requirement = context.Requirements.Single(r => r.Id == _requirementWithThreeNumberShowPrevId);
                
                requirement.RecordValues(
                    new Dictionary<int, string>
                    {
                        {fieldWithNaId, "NA"},
                        {fieldWithDoubleId, numberAsString}
                    },
                    "",
                    requirementDefinition);
                context.SaveChanges();
            }

            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
            {
                var query = new GetTagRequirementsQuery(_tagId);
                var dut = new GetTagRequirementsQueryHandler(context, _timeServiceMock.Object);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);

                var requirementWithThreeNumberShowPrev = result.Data.Single(r => r.Id == _requirementWithThreeNumberShowPrevId);
                Assert.AreEqual(3, requirementWithThreeNumberShowPrev.Fields.Count);

                var fieldWithNa = requirementWithThreeNumberShowPrev.Fields.Single(f => f.Id == fieldWithNaId);
                AssertNaNumber(fieldWithNa);

                var fieldWithDouble = requirementWithThreeNumberShowPrev.Fields.Single(f => f.Id == fieldWithDoubleId);
                AssertNumber(fieldWithDouble, number);
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnsTagRequirementsWithOrderedFields()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object))
            {
                var query = new GetTagRequirementsQuery(_tagId);
                var dut = new GetTagRequirementsQueryHandler(context, _timeServiceMock.Object);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);

                var requirementWithTwoCheckBoxes = result.Data.Single(r => r.Id == _requirementWithTwoCheckBoxesId);
                Assert.AreEqual(2, requirementWithTwoCheckBoxes.Fields.Count);

                Assert.AreEqual(_firstCbFieldId, requirementWithTwoCheckBoxes.Fields.ElementAt(0).Id);
                Assert.AreEqual(_secondCbFieldId, requirementWithTwoCheckBoxes.Fields.ElementAt(1).Id);

                var requirementWithThreeNumbers = result.Data.Single(r => r.Id == _requirementWithThreeNumberShowPrevId);
                Assert.AreEqual(3, requirementWithThreeNumbers.Fields.Count);

                Assert.AreEqual(_firstNumberFieldId, requirementWithThreeNumbers.Fields.ElementAt(0).Id);
                Assert.AreEqual(_secondNumberFieldId, requirementWithThreeNumbers.Fields.ElementAt(1).Id);
                Assert.AreEqual(_thirdNumberFieldId, requirementWithThreeNumbers.Fields.ElementAt(2).Id);
            }
        }

        [TestMethod]
        public async Task Handler_ReturnsNotFound_IfTagIsNotFound()
        {
            var dbContextOptions = new DbContextOptionsBuilder<PreservationContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                 .Options;

            using var context = new PreservationContext(dbContextOptions, _eventDispatcherMock.Object, _plantProviderMock.Object);
            var query = new GetTagRequirementsQuery(_tagId);
            var dut = new GetTagRequirementsQueryHandler(context, _timeServiceMock.Object);

            var result = await dut.Handle(query, default);

            Assert.IsNotNull(result);
            Assert.AreEqual(ResultType.NotFound, result.ResultType);
            Assert.IsNull(result.Data);
        }

        private void AssertRequirements(List<RequirementDto> requirements)
        {
            var requirementWithoutField = requirements.Single(r => r.Id == _requirementWithoutFieldId);
            var requirementWithOneInfo = requirements.Single(r => r.Id == _requirementWithOneInfoId);
            var requirementWithTwoCheckBoxes = requirements.Single(r => r.Id == _requirementWithTwoCheckBoxesId);
            var requirementWithThreeNumberShowPrev = requirements.Single(r => r.Id == _requirementWithThreeNumberShowPrevId);
            var requirementWithOneNumberNoPrev = requirements.Single(r => r.Id == _requirementWithOneNumberNoPrevId);

            Assert.AreEqual(0, requirementWithoutField.Fields.Count);
            Assert.AreEqual(_requirementDefinitionWithoutFieldTitle, requirementWithoutField.RequirementDefinitionTitle);
            Assert.AreEqual(_requirementType1Code, requirementWithoutField.RequirementTypeCode);
            Assert.AreEqual(_requirementType1Title, requirementWithoutField.RequirementTypeTitle);
            
            Assert.AreEqual(1, requirementWithOneInfo.Fields.Count);
            AssertInfoField(requirementWithOneInfo.Fields.ElementAt(0));
            Assert.AreEqual(_requirementDefinitionWithOneInfoTitle, requirementWithOneInfo.RequirementDefinitionTitle);
            Assert.AreEqual(_requirementType1Code, requirementWithOneInfo.RequirementTypeCode);
            Assert.AreEqual(_requirementType1Title, requirementWithOneInfo.RequirementTypeTitle);

            Assert.AreEqual(2, requirementWithTwoCheckBoxes.Fields.Count);
            AssertCheckBoxField(requirementWithTwoCheckBoxes.Fields.ElementAt(0));
            AssertCheckBoxField(requirementWithTwoCheckBoxes.Fields.ElementAt(1));
            Assert.AreEqual(_requirementDefinitionWithTwoCheckBoxesTitle, requirementWithTwoCheckBoxes.RequirementDefinitionTitle);
            Assert.AreEqual(_requirementType2Code, requirementWithTwoCheckBoxes.RequirementTypeCode);
            Assert.AreEqual(_requirementType2Title, requirementWithTwoCheckBoxes.RequirementTypeTitle);

            Assert.AreEqual(3, requirementWithThreeNumberShowPrev.Fields.Count);
            AssertNumberWithPreviewField(requirementWithThreeNumberShowPrev.Fields.ElementAt(0));
            AssertNumberWithPreviewField(requirementWithThreeNumberShowPrev.Fields.ElementAt(1));
            AssertNumberWithPreviewField(requirementWithThreeNumberShowPrev.Fields.ElementAt(2));
            Assert.AreEqual(_requirementDefinitionWithThreeNumberShowPrevTitle, requirementWithThreeNumberShowPrev.RequirementDefinitionTitle);
            Assert.AreEqual(_requirementType2Code, requirementWithThreeNumberShowPrev.RequirementTypeCode);
            Assert.AreEqual(_requirementType2Title, requirementWithThreeNumberShowPrev.RequirementTypeTitle);

            Assert.AreEqual(1, requirementWithOneNumberNoPrev.Fields.Count);
            AssertNumberWithNoPreviewField(requirementWithOneNumberNoPrev.Fields.ElementAt(0));
            Assert.AreEqual(_requirementDefinitionWithOneNumberNoPrevTitle, requirementWithOneNumberNoPrev.RequirementDefinitionTitle);
            Assert.AreEqual(_requirementType2Code, requirementWithOneNumberNoPrev.RequirementTypeCode);
            Assert.AreEqual(_requirementType2Title, requirementWithOneNumberNoPrev.RequirementTypeTitle);
        }

        private void AssertInfoField(FieldDto f)
        {
            Assert.AreEqual(FieldType.Info, f.FieldType);
            Assert.IsFalse(f.ShowPrevious);
            Assert.IsNull(f.Unit);
        }

        private void AssertCheckBoxField(FieldDto f)
        {
            Assert.AreEqual(FieldType.CheckBox, f.FieldType);
            Assert.IsFalse(f.ShowPrevious);
            Assert.IsNull(f.Unit);
        }

        private void AssertNumberWithPreviewField(FieldDto f)
        {
            Assert.AreEqual(FieldType.Number, f.FieldType);
            Assert.IsTrue(f.ShowPrevious);
            Assert.AreEqual(_unit, f.Unit);
        }

        private void AssertNumberWithNoPreviewField(FieldDto f)
        {
            Assert.AreEqual(FieldType.Number, f.FieldType);
            Assert.IsFalse(f.ShowPrevious);
            Assert.AreEqual(_unit, f.Unit);
        }

        private static void AssertNaNumber(FieldDto f)
        {
            var numberValue = AssertNumber(f);
            Assert.IsTrue(numberValue.IsNA);
            Assert.IsFalse(numberValue.Value.HasValue);
        }

        private static void AssertNumber(FieldDto f, double value)
        {
            var numberValue = AssertNumber(f);
            Assert.IsFalse(numberValue.IsNA);
            Assert.IsTrue(numberValue.Value.HasValue);
            Assert.AreEqual(value, numberValue.Value.Value);
        }

        private static NumberDto AssertNumber(FieldDto f)
        {
            var numberValue = f.CurrentValue as NumberDto;
            Assert.IsNotNull(f);
            Assert.IsNotNull(f.CurrentValue);
            Assert.IsInstanceOfType(f.CurrentValue, typeof(NumberDto));

            Assert.IsNotNull(numberValue);
            return numberValue;
        }
    }
}

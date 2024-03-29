﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Query.GetTagRequirements;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceResult;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetTagRequirements
{
    [TestClass]
    public class GetTagRequirementsQueryHandlerTests : ReadOnlyTestsBase
    {
        protected const string _unit = "unit";
        const string _requirementType1Code = "Code1";
        RequirementTypeIcon _requirementType1Icon = RequirementTypeIcon.Other;
        const string _requirementType1Title = "Title1";
        const string _requirementType2Code = "Code2";
        RequirementTypeIcon _requirementType2Icon = RequirementTypeIcon.Battery;
        const string _requirementType2Title = "Title2";
        const string _requirementDefinitionWithoutFieldTitle = "Without fields";
        const string _requirementDefinitionWithOneInfoTitle = "With 1 info";
        const string _requirementDefinitionWithOneAttachmentTitle = "With 1 attachment";
        const string _requirementDefinitionWithTwoCheckBoxesTitle = "With 2 checkboxes";
        const string _requirementDefinitionWithThreeNumberShowPrevTitle = "With 3 number with previous";
        const string _requirementDefinitionWithOneNumberNoPrevTitle = "With 1 number no previous";
        const string _requirementDefinitionForVoidedTest = "With 1 field, for voided test";
        protected DateTime _currentUtc;
        private DateTime _startedAtUtc;

        private int _requirementWithoutFieldId;
        private int _requirementWithOneInfoId;
        private int _requirementWithOneAttachmentId;
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
        private int _attachmentFieldId;
        private int _requirementDefinitionWithOneAttachmentId;
        private int _requirementDefinitionWithTwoCheckBoxesId;
        private int _requirementDefinitionWithThreeNumberShowPrevId;

        [TestInitialize]
        public void Setup()
        {
            _startedAtUtc = new DateTime(2020, 2, 1, 0, 0, 0, DateTimeKind.Utc);
            _currentUtc = _startedAtUtc.AddWeeks(_requestTimeAfterPreservationStartedInWeeks);
        }

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var journey = AddJourneyWithStep(context, "J1", "S", AddMode(context, "M1", false), AddResponsible(context, "R1"));

                var requirementType1 = new RequirementType(TestPlant, _requirementType1Code, _requirementType1Title, _requirementType1Icon,0);
                context.RequirementTypes.Add(requirementType1);
                var requirementType2 = new RequirementType(TestPlant, _requirementType2Code, _requirementType2Title, _requirementType2Icon,0);
                context.RequirementTypes.Add(requirementType2);
                context.SaveChangesAsync().Wait();

                var requirementDefinitionWithoutField = new RequirementDefinition(TestPlant, _requirementDefinitionWithoutFieldTitle, 2, RequirementUsage.ForAll, 1);
                requirementType1.AddRequirementDefinition(requirementDefinitionWithoutField);

                var requirementDefinitionWithOneInfo = new RequirementDefinition(TestPlant, _requirementDefinitionWithOneInfoTitle, 2, RequirementUsage.ForAll, 1);
                var infoField = new Field(TestPlant, "Label for Info", FieldType.Info, 0);
                requirementDefinitionWithOneInfo.AddField(infoField);
                requirementType1.AddRequirementDefinition(requirementDefinitionWithOneInfo);

                var requirementDefinitionWithOneAttachment = new RequirementDefinition(TestPlant, _requirementDefinitionWithOneAttachmentTitle, 2, RequirementUsage.ForAll, 1);
                var attachmentField = new Field(TestPlant, "Label for Attachment", FieldType.Attachment, 0);
                requirementDefinitionWithOneAttachment.AddField(attachmentField);
                requirementType1.AddRequirementDefinition(requirementDefinitionWithOneAttachment);

                var requirementDefinitionWithTwoCheckBoxes = new RequirementDefinition(TestPlant, _requirementDefinitionWithTwoCheckBoxesTitle, 2, RequirementUsage.ForAll, 1);
                var cbField1 = new Field(TestPlant, "Label for checkBox - second", FieldType.CheckBox, 10);
                var cbField2 = new Field(TestPlant, "Label for checkBox - first", FieldType.CheckBox, 2);
                requirementDefinitionWithTwoCheckBoxes.AddField(cbField1);
                requirementDefinitionWithTwoCheckBoxes.AddField(cbField2);
                requirementType2.AddRequirementDefinition(requirementDefinitionWithTwoCheckBoxes);

                var requirementDefinitionWithThreeNumberShowPrev = new RequirementDefinition(TestPlant, _requirementDefinitionWithThreeNumberShowPrevTitle, 2, RequirementUsage.ForAll, 1);
                var numberFieldPrev1 = new Field(TestPlant, "Label for number - third", FieldType.Number, 15, _unit, true);
                var numberFieldPrev2 = new Field(TestPlant, "Label for number - first", FieldType.Number, 2, _unit, true);
                var numberFieldPrev3 = new Field(TestPlant, "Label for number - second", FieldType.Number, 10, _unit, true);
                requirementDefinitionWithThreeNumberShowPrev.AddField(numberFieldPrev1);
                requirementDefinitionWithThreeNumberShowPrev.AddField(numberFieldPrev2);
                requirementDefinitionWithThreeNumberShowPrev.AddField(numberFieldPrev3);
                requirementType2.AddRequirementDefinition(requirementDefinitionWithThreeNumberShowPrev);

                var requirementDefinitionWithOneNumberNoPrev = new RequirementDefinition(TestPlant, _requirementDefinitionWithOneNumberNoPrevTitle, 2, RequirementUsage.ForAll, 1);
                var numberFieldNoPrev = new Field(TestPlant, "Label for number", FieldType.Number, 10, _unit, false);
                requirementDefinitionWithOneNumberNoPrev.AddField(numberFieldNoPrev);
                requirementType2.AddRequirementDefinition(requirementDefinitionWithOneNumberNoPrev);

                var requirementDefinitionForVoidedTest = new RequirementDefinition(TestPlant, _requirementDefinitionForVoidedTest, 2, RequirementUsage.ForAll, 1);
                var vField1 = new Field(TestPlant, "Label for field", FieldType.CheckBox, 10);
                requirementDefinitionForVoidedTest.AddField(vField1);
                requirementType2.AddRequirementDefinition(requirementDefinitionForVoidedTest);

                context.SaveChangesAsync().Wait();

                var requirementWithoutField = new TagRequirement(TestPlant, _interval, requirementDefinitionWithoutField);
                var requirementWithOneInfo = new TagRequirement(TestPlant, _interval, requirementDefinitionWithOneInfo);
                var requirementWithOneAttachment = new TagRequirement(TestPlant, _interval, requirementDefinitionWithOneAttachment);
                var requirementWithTwoCheckBoxes = new TagRequirement(TestPlant, _interval, requirementDefinitionWithTwoCheckBoxes);
                var requirementWithOneNumberNoPrev = new TagRequirement(TestPlant, _interval, requirementDefinitionWithOneNumberNoPrev);
                var requirementWithThreeNumberShowPrev = new TagRequirement(TestPlant, _interval, requirementDefinitionWithThreeNumberShowPrev);
                var requirementThatIsVoided = new TagRequirement(TestPlant, _interval, requirementDefinitionForVoidedTest);
                requirementThatIsVoided.IsVoided = true;

                var tag = new Tag(TestPlant,
                    TagType.Standard,
                    Guid.NewGuid(),
                    "TagNo",
                    "Description",
                    journey.Steps.ElementAt(0),
                    new List<TagRequirement>
                    {
                        requirementWithoutField,
                        requirementWithOneInfo,
                        requirementWithOneAttachment,
                        requirementWithTwoCheckBoxes,
                        requirementWithOneNumberNoPrev,
                        requirementWithThreeNumberShowPrev,
                        requirementThatIsVoided
                    });
                context.Tags.Add(tag);
                context.SaveChangesAsync().Wait();

                _tagId = tag.Id;

                _requirementDefinitionWithOneAttachmentId = requirementDefinitionWithOneAttachment.Id;
                _requirementDefinitionWithTwoCheckBoxesId = requirementDefinitionWithTwoCheckBoxes.Id;
                _requirementDefinitionWithThreeNumberShowPrevId = requirementDefinitionWithThreeNumberShowPrev.Id;

                _requirementWithoutFieldId = requirementWithoutField.Id;
                _requirementWithOneInfoId = requirementWithOneInfo.Id;
                _requirementWithOneAttachmentId = requirementWithOneAttachment.Id;
                _requirementWithTwoCheckBoxesId = requirementWithTwoCheckBoxes.Id;
                _requirementWithThreeNumberShowPrevId = requirementWithThreeNumberShowPrev.Id;
                _requirementWithOneNumberNoPrevId = requirementWithOneNumberNoPrev.Id;

                _attachmentFieldId = attachmentField.Id;

                _firstCbFieldId = cbField2.Id;
                _secondCbFieldId = cbField1.Id;

                _firstNumberFieldId = numberFieldPrev2.Id;
                _secondNumberFieldId = numberFieldPrev3.Id;
                _thirdNumberFieldId = numberFieldPrev1.Id;
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnTagRequirements_NoDueDates_BeforePreservationStarted()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagRequirementsQuery(_tagId, false, false);
                var dut = new GetTagRequirementsQueryHandler(context);

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
        public async Task Handler_ShouldReturnTagRequirements_NotInUse_BeforePreservationStarted()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagRequirementsQuery(_tagId, false, false);
                var dut = new GetTagRequirementsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);

                foreach (var requirement in result.Data)
                {
                    Assert.IsFalse(requirement.IsInUse);
                }
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnTagRequirements_Voided()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagRequirementsQuery(_tagId, false, false);
                var dut = new GetTagRequirementsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);

                foreach (var requirement in result.Data)
                {
                    Assert.IsFalse(requirement.IsVoided);
                }

                AssertRequirements(result.Data);
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnTagRequirements_Voided_and_Unvoided()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagRequirementsQuery(_tagId, true, false);
                var dut = new GetTagRequirementsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);

                Assert.IsTrue(result.Data.FindIndex(req => req.IsVoided) > -1);

                AssertRequirements(result.Data);
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnTagRequirements_AfterPreservationStarted()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single();
                tag.StartPreservation();
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _timeProvider.ElapseWeeks(_requestTimeAfterPreservationStartedInWeeks);

                var query = new GetTagRequirementsQuery(_tagId, false, false);
                var dut = new GetTagRequirementsQueryHandler(context);

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
        public async Task Handler_ShouldReturnTagRequirements_IsInUse_AfterPreservationStarted()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single();
                tag.StartPreservation();
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagRequirementsQuery(_tagId, false, false);
                var dut = new GetTagRequirementsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);

                foreach (var requirement in result.Data)
                {
                    Assert.IsTrue(requirement.IsInUse);
                }
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnTagRequirementsWithCheckBoxFieldAndComment_AfterRecordingCheckBoxFieldAndComment()
        {
            var cbFieldId = _firstCbFieldId;

            using (var context = new PreservationContext(_dbContextOptions, _plantProviderMock.Object, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single();
                tag.StartPreservation();
                context.SaveChangesAsync().Wait();

                var requirementDefinition = context.RequirementDefinitions.Include(rd => rd.Fields)
                    .Single(rd => rd.Id == _requirementDefinitionWithTwoCheckBoxesId);
                var requirement = context.TagRequirements.Single(r => r.Id == _requirementWithTwoCheckBoxesId);
                requirement.RecordCheckBoxValues(
                    new Dictionary<int, bool> {{cbFieldId, true}},
                    requirementDefinition);
                requirement.SetComment("CommentABC");
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProviderMock.Object, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagRequirementsQuery(_tagId, false, false);
                var dut = new GetTagRequirementsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);

                var requirementWithTwoCheckBoxes = result.Data.Single(r => r.Id == _requirementWithTwoCheckBoxesId);
                Assert.AreEqual("CommentABC", requirementWithTwoCheckBoxes.Comment);
                Assert.AreEqual(2, requirementWithTwoCheckBoxes.Fields.Count);

                var field = requirementWithTwoCheckBoxes.Fields.Single(f => f.Id == cbFieldId);
                Assert.IsNotNull(field);
                Assert.IsNotNull(field.CurrentValue);
                Assert.IsInstanceOfType(field.CurrentValue, typeof(CheckBoxDetailsDto));
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnTagRequirementsWithNumbers_AfterRecordingNumberFields()
        {
            var numberFieldWithNaId = _thirdNumberFieldId;
            var numberFieldWithDoubleId = _secondNumberFieldId;
            var number = 1282.91;

            using (var context = new PreservationContext(_dbContextOptions, _plantProviderMock.Object, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single();
                tag.StartPreservation();
                context.SaveChangesAsync().Wait();

                var requirementDefinition = context.RequirementDefinitions.Include(rd => rd.Fields)
                    .Single(rd => rd.Id == _requirementDefinitionWithThreeNumberShowPrevId);
                var requirement = context.TagRequirements.Single(r => r.Id == _requirementWithThreeNumberShowPrevId);
                
                requirement.RecordNumberIsNaValues(
                    new List<int>
                    {
                        numberFieldWithNaId
                    },
                    requirementDefinition);
                requirement.RecordNumberValues(
                    new Dictionary<int, double?>
                    {
                        {numberFieldWithDoubleId, number}
                    },
                    requirementDefinition);
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProviderMock.Object, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagRequirementsQuery(_tagId, false, false);
                var dut = new GetTagRequirementsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);

                var requirementWithThreeNumberShowPrev = result.Data.Single(r => r.Id == _requirementWithThreeNumberShowPrevId);
                Assert.AreEqual(3, requirementWithThreeNumberShowPrev.Fields.Count);

                var fieldWithNa = requirementWithThreeNumberShowPrev.Fields.Single(f => f.Id == numberFieldWithNaId);
                AssertNaNumberInCurrentValue(fieldWithNa);

                var fieldWithDouble = requirementWithThreeNumberShowPrev.Fields.Single(f => f.Id == numberFieldWithDoubleId);
                AssertNumberInCurrentValue(fieldWithDouble, number);
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnTagRequirementsWithAttachment_AfterRecordingAttachment()
        {
            var attachmentField = _attachmentFieldId;
            var fieldValueAttachment = new FieldValueAttachment(TestPlant, Guid.Empty, "FilA.txt");

            using (var context = new PreservationContext(_dbContextOptions, _plantProviderMock.Object, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single();
                tag.StartPreservation();
                context.SaveChangesAsync().Wait();

                var requirementDefinition = context.RequirementDefinitions.Include(rd => rd.Fields)
                    .Single(rd => rd.Id == _requirementDefinitionWithOneAttachmentId);
                var requirement = context.TagRequirements.Single(r => r.Id == _requirementWithOneAttachmentId);

                requirement.RecordAttachment(
                    fieldValueAttachment, 
                    attachmentField,
                    requirementDefinition);
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProviderMock.Object, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagRequirementsQuery(_tagId, false, false);
                var dut = new GetTagRequirementsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);

                var requirementWithAttachment = result.Data.Single(r => r.Id == _requirementWithOneAttachmentId);
                Assert.AreEqual(1, requirementWithAttachment.Fields.Count);

                var fieldWithAttachment = requirementWithAttachment.Fields.Single(f => f.Id == attachmentField);
                AssertAttachmentField(fieldWithAttachment, fieldValueAttachment);
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnTagRequirementsWithOrderedFields()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProviderMock.Object, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagRequirementsQuery(_tagId, false, false);
                var dut = new GetTagRequirementsQueryHandler(context);

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
        public async Task Handler_ShouldReturnPreviousValues_AfterRecordingNumberFieldsAndPreserving()
        {
            var number = 1.91;

            using (var context = new PreservationContext(_dbContextOptions, _plantProviderMock.Object, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single();
                tag.StartPreservation();
                context.SaveChangesAsync().Wait();

                var requirementDefinition = context.RequirementDefinitions.Include(rd => rd.Fields)
                    .Single(rd => rd.Id == _requirementDefinitionWithThreeNumberShowPrevId);
                var requirement = context.TagRequirements.Single(r => r.Id == _requirementWithThreeNumberShowPrevId);
                
                requirement.RecordNumberValues(
                    new Dictionary<int, double?>
                    {
                        {_firstNumberFieldId, number},
                        {_secondNumberFieldId, number},
                        {_thirdNumberFieldId, number}
                    },
                    requirementDefinition);
                context.SaveChangesAsync().Wait();

                _timeProvider.ElapseWeeks(_interval);
                tag.Preserve(new Mock<Person>().Object);
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagRequirementsQuery(_tagId, false, false);
                var dut = new GetTagRequirementsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.Ok, result.ResultType);

                var requirementWithThreeNumberShowPrev = result.Data.Single(r => r.Id == _requirementWithThreeNumberShowPrevId);
                Assert.AreEqual(3, requirementWithThreeNumberShowPrev.Fields.Count);

                foreach (var fieldDto in requirementWithThreeNumberShowPrev.Fields)
                {
                    AssertNumberInPreviousValue(fieldDto, number);
                }
            }
        }

        [TestMethod]
        public async Task Handler_ShouldReturnNotFound_WhenTagNotFound()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProviderMock.Object, _eventDispatcher, _currentUserProvider))
            {
                var query = new GetTagRequirementsQuery(0, false, false);
                var dut = new GetTagRequirementsQueryHandler(context);

                var result = await dut.Handle(query, default);

                Assert.IsNotNull(result);
                Assert.AreEqual(ResultType.NotFound, result.ResultType);
                Assert.IsNull(result.Data);
            }
        }

        private void AssertRequirements(List<RequirementDetailsDto> requirements)
        {
            var requirementWithoutField = requirements.Single(r => r.Id == _requirementWithoutFieldId);
            var requirementWithOneInfo = requirements.Single(r => r.Id == _requirementWithOneInfoId);
            var requirementWithTwoCheckBoxes = requirements.Single(r => r.Id == _requirementWithTwoCheckBoxesId);
            var requirementWithThreeNumberShowPrev = requirements.Single(r => r.Id == _requirementWithThreeNumberShowPrevId);
            var requirementWithOneNumberNoPrev = requirements.Single(r => r.Id == _requirementWithOneNumberNoPrevId);

            Assert.AreEqual(0, requirementWithoutField.Fields.Count);
            Assert.AreEqual(_requirementDefinitionWithoutFieldTitle, requirementWithoutField.RequirementDefinition.Title);
            Assert.AreEqual(_requirementType1Code, requirementWithoutField.RequirementType.Code);
            Assert.AreEqual(_requirementType1Icon, requirementWithoutField.RequirementType.Icon);
            Assert.AreEqual(_requirementType1Title, requirementWithoutField.RequirementType.Title);
            
            Assert.AreEqual(1, requirementWithOneInfo.Fields.Count);
            AssertInfoField(requirementWithOneInfo.Fields.ElementAt(0));
            Assert.AreEqual(_requirementDefinitionWithOneInfoTitle, requirementWithOneInfo.RequirementDefinition.Title);
            Assert.AreEqual(_requirementType1Code, requirementWithOneInfo.RequirementType.Code);
            Assert.AreEqual(_requirementType1Icon, requirementWithOneInfo.RequirementType.Icon);
            Assert.AreEqual(_requirementType1Title, requirementWithOneInfo.RequirementType.Title);

            Assert.AreEqual(2, requirementWithTwoCheckBoxes.Fields.Count);
            AssertCheckBoxField(requirementWithTwoCheckBoxes.Fields.ElementAt(0));
            AssertCheckBoxField(requirementWithTwoCheckBoxes.Fields.ElementAt(1));
            Assert.AreEqual(_requirementDefinitionWithTwoCheckBoxesTitle, requirementWithTwoCheckBoxes.RequirementDefinition.Title);
            Assert.AreEqual(_requirementType2Code, requirementWithTwoCheckBoxes.RequirementType.Code);
            Assert.AreEqual(_requirementType2Icon, requirementWithTwoCheckBoxes.RequirementType.Icon);
            Assert.AreEqual(_requirementType2Title, requirementWithTwoCheckBoxes.RequirementType.Title);

            Assert.AreEqual(3, requirementWithThreeNumberShowPrev.Fields.Count);
            AssertNumberWithPreviewField(requirementWithThreeNumberShowPrev.Fields.ElementAt(0));
            AssertNumberWithPreviewField(requirementWithThreeNumberShowPrev.Fields.ElementAt(1));
            AssertNumberWithPreviewField(requirementWithThreeNumberShowPrev.Fields.ElementAt(2));
            Assert.AreEqual(_requirementDefinitionWithThreeNumberShowPrevTitle, requirementWithThreeNumberShowPrev.RequirementDefinition.Title);
            Assert.AreEqual(_requirementType2Code, requirementWithThreeNumberShowPrev.RequirementType.Code);
            Assert.AreEqual(_requirementType2Icon, requirementWithThreeNumberShowPrev.RequirementType.Icon);
            Assert.AreEqual(_requirementType2Title, requirementWithThreeNumberShowPrev.RequirementType.Title);

            Assert.AreEqual(1, requirementWithOneNumberNoPrev.Fields.Count);
            AssertNumberWithNoPreviewField(requirementWithOneNumberNoPrev.Fields.ElementAt(0));
            Assert.AreEqual(_requirementDefinitionWithOneNumberNoPrevTitle, requirementWithOneNumberNoPrev.RequirementDefinition.Title);
            Assert.AreEqual(_requirementType2Code, requirementWithOneNumberNoPrev.RequirementType.Code);
            Assert.AreEqual(_requirementType2Icon, requirementWithOneNumberNoPrev.RequirementType.Icon);
            Assert.AreEqual(_requirementType2Title, requirementWithOneNumberNoPrev.RequirementType.Title);
        }

        private void AssertInfoField(FieldDetailsDto f)
        {
            Assert.AreEqual(FieldType.Info, f.FieldType);
            Assert.IsFalse(f.ShowPrevious);
            Assert.IsNull(f.Unit);
        }

        private void AssertCheckBoxField(FieldDetailsDto f)
        {
            Assert.AreEqual(FieldType.CheckBox, f.FieldType);
            Assert.IsFalse(f.ShowPrevious);
            Assert.IsNull(f.Unit);
        }

        private void AssertNumberWithPreviewField(FieldDetailsDto f)
        {
            Assert.AreEqual(FieldType.Number, f.FieldType);
            Assert.IsTrue(f.ShowPrevious);
            Assert.AreEqual(_unit, f.Unit);
        }

        private void AssertNumberWithNoPreviewField(FieldDetailsDto f)
        {
            Assert.AreEqual(FieldType.Number, f.FieldType);
            Assert.IsFalse(f.ShowPrevious);
            Assert.AreEqual(_unit, f.Unit);
        }

        private static void AssertNaNumberInCurrentValue(FieldDetailsDto f)
        {
            var numberValue = AssertIsNumberDto(f.CurrentValue);
            Assert.IsTrue(numberValue.IsNA);
            Assert.IsFalse(numberValue.Value.HasValue);
        }

        private static void AssertNumberInCurrentValue(FieldDetailsDto f, double expectedValue)
        {
            var numberValue = AssertIsNumberDto(f.CurrentValue);
            AssertNumberDto(numberValue, expectedValue);
        }

        private static void AssertNumberInPreviousValue(FieldDetailsDto f, double expectedValue)
        {
            var numberValue = AssertIsNumberDto(f.PreviousValue);
            AssertNumberDto(numberValue, expectedValue);
        }

        private static NumberDetailsDto AssertIsNumberDto(object numberDto)
        {
            Assert.IsInstanceOfType(numberDto, typeof(NumberDetailsDto));
            var numberValue = numberDto as NumberDetailsDto;
            Assert.IsNotNull(numberDto);

            Assert.IsNotNull(numberValue);
            return numberValue;
        }

        private static void AssertNumberDto(NumberDetailsDto numberValue, double expectedValue)
        {
            Assert.IsFalse(numberValue.IsNA);
            Assert.IsTrue(numberValue.Value.HasValue);
            Assert.AreEqual(expectedValue, numberValue.Value.Value);
        }

        private void AssertAttachmentField(FieldDetailsDto f, FieldValueAttachment expectedValue)
        {
            Assert.AreEqual(FieldType.Attachment, f.FieldType);
            Assert.IsFalse(f.ShowPrevious);
            Assert.IsNull(f.Unit);

            Assert.IsInstanceOfType(f.CurrentValue, typeof(AttachmentDetailsDto));
            var attachmentDto = f.CurrentValue as AttachmentDetailsDto;
            Assert.IsNotNull(attachmentDto);
            Assert.AreEqual(expectedValue.Id, attachmentDto.Id);
            Assert.AreEqual(expectedValue.FileName, attachmentDto.FileName);
        }
    }
}

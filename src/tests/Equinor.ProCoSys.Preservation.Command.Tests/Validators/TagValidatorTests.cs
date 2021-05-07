using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.ProCoSys.Preservation.Command.Validators.TagValidators;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Action = Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate.Action;

namespace Equinor.ProCoSys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class TagValidatorTests : ReadOnlyTestsBase
    {
        private const string ProjectName = "P";
        private const string TagNo1 = "PA-13";
        private const string TagNo2 = "PA-14";
        private const string TagNo3 = "PA-15";
        private const string _tagDescription = "Tag description";
        private int _tagWithOneReqsId;
        private int _tagWithTwoReqsId;
        private int _tagWithAllReqsId;
        private int _standardTagCompletedId;
        private int _standardTagNotStartedInFirstStepId;
        private int _standardTagStartedAndInLastStepId;
        private int _preAreaTagNotStartedInFirstStepId;
        private int _preAreaTagStartedInFirstStepId;
        private int _siteAreaTagNotStartedId;
        private int _poAreaTagNotStartedId;
        private int _siteAreaTagStartedId;
        private int _poAreaTagStartedId;
        private int _poAreaTagWithTwoReqsId;
        private int _reqDefForAll2Id;
        private int _tagReqForAll1Id;
        private int _tagReqForSupplierId;
        private int _tagReqForOther1Id;
        private int _reqDefForOther2Id;
        private int _firstStepId;
        private int _poAreaTagActionId;
        private int _poAreaTagAttachmentId;
        private int _poAreaTagActionAttachmentId;
        private TagRequirement _tagRequirementForPoAreaTag;
        private const int IntervalWeeks = 4;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var project = AddProject(context, ProjectName, "Project description");
                var journey = AddJourneyWithStep(context, "J", "S1", AddMode(context, "M1", false), AddResponsible(context, "R1"));
                journey.AddStep(new Step(TestPlant, "S2", AddMode(context, "M2", false), AddResponsible(context, "R2")));

                var requirementType = AddRequirementTypeWith1DefWithoutField(context, "R1", "D1", RequirementTypeIcon.Other);
                var reqDefForAll1 = requirementType.RequirementDefinitions.First();
                var reqDefForAll2 = AddRequirementTypeWith1DefWithoutField(context, "R2", "D2", RequirementTypeIcon.Other).RequirementDefinitions.First();
                var reqDefForSupplier = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForSuppliersOnly, 1);
                requirementType.AddRequirementDefinition(reqDefForSupplier);
                var reqDefForOther1 = new RequirementDefinition(TestPlant, "D3", 2, RequirementUsage.ForOtherThanSuppliers, 1);
                var reqDefForOther2 = new RequirementDefinition(TestPlant, "D4", 2, RequirementUsage.ForOtherThanSuppliers, 1);
                requirementType.AddRequirementDefinition(reqDefForOther1);
                requirementType.AddRequirementDefinition(reqDefForOther2);
                context.SaveChangesAsync().Wait();

                var firstStep = journey.Steps.First();
                _firstStepId = firstStep.Id;
                var standardTagNotStartedInFirstStep = AddTag(context, project, TagType.Standard, TagNo1,
                    _tagDescription, firstStep, new List<TagRequirement>
                    {
                        new TagRequirement(TestPlant, IntervalWeeks, reqDefForAll1), 
                        new TagRequirement(TestPlant, IntervalWeeks, reqDefForSupplier), 
                        new TagRequirement(TestPlant, IntervalWeeks, reqDefForOther1)
                    });

                var standardTagCompleted = AddTag(context, project, TagType.Standard, TagNo2, "",
                    journey.Steps.Last(), new List<TagRequirement> { new TagRequirement(TestPlant, IntervalWeeks, reqDefForAll1) });
                standardTagCompleted.StartPreservation();
                standardTagCompleted.CompletePreservation(journey);
                var standardTagStartedInLastStep = AddTag(context, project, TagType.Standard, TagNo2, "",
                    journey.Steps.Last(), new List<TagRequirement> {new TagRequirement(TestPlant, IntervalWeeks, reqDefForAll1)});
                standardTagStartedInLastStep.StartPreservation();
                var standardTagWithTwoReqsInLastStep = AddTag(context, project, TagType.Standard, TagNo3, "",
                    journey.Steps.Last(), new List<TagRequirement>
                    {
                        new TagRequirement(TestPlant, IntervalWeeks, reqDefForOther1),
                        new TagRequirement(TestPlant, IntervalWeeks, reqDefForAll1)
                    });

                var preAreaTagNotStartedInFirstStep = AddTag(context, project, TagType.PreArea, "#PRE-E-A1", _tagDescription,
                    firstStep, new List<TagRequirement> {new TagRequirement(TestPlant, IntervalWeeks, reqDefForAll1)});
                var preAreaTagStartedInFirstStep = AddTag(context, project, TagType.PreArea, "#PRE-E-A2", _tagDescription,
                    firstStep, new List<TagRequirement> {new TagRequirement(TestPlant, IntervalWeeks, reqDefForAll1)});
                preAreaTagStartedInFirstStep.StartPreservation();
                var siteAreaTagNotStarted = AddTag(context, project, TagType.SiteArea, "#SITE-E-A1", _tagDescription, 
                    firstStep, new List<TagRequirement> {new TagRequirement(TestPlant, IntervalWeeks, reqDefForAll1)});
                var siteAreaTagStarted = AddTag(context, project, TagType.SiteArea, "#SITE-E-A2", _tagDescription, 
                    firstStep, new List<TagRequirement> {new TagRequirement(TestPlant, IntervalWeeks, reqDefForAll1)});
                siteAreaTagStarted.StartPreservation();
                var poAreaTagNotStarted = AddTag(context, project, TagType.PoArea, "#PO-E-A1", _tagDescription,
                    firstStep, new List<TagRequirement> {new TagRequirement(TestPlant, IntervalWeeks, reqDefForAll1)});
                _tagRequirementForPoAreaTag = new TagRequirement(TestPlant, IntervalWeeks, reqDefForAll1);
                var poAreaTagStarted = AddTag(context, project, TagType.PoArea, "#PO-E-A2", _tagDescription,
                    firstStep, new List<TagRequirement> {_tagRequirementForPoAreaTag});
                poAreaTagStarted.StartPreservation();
                var poAreaTagWithTwoReqs = AddTag(context, project, TagType.PoArea, "#PO-E-A3", _tagDescription,
                    firstStep, new List<TagRequirement>
                    {
                        new TagRequirement(TestPlant, IntervalWeeks, reqDefForSupplier),
                        new TagRequirement(TestPlant, IntervalWeeks, reqDefForAll1)
                    });

                var tagAttachment = new TagAttachment(TestPlant, Guid.Empty, "fil1.txt");
                poAreaTagStarted.AddAttachment(tagAttachment);

                var action = new Action(TestPlant, "A", "D", null);
                poAreaTagStarted.AddAction(action);
                var actionAttachment = new ActionAttachment(TestPlant, Guid.Empty, "fil2.txt");
                action.AddAttachment(actionAttachment);
                
                context.SaveChangesAsync().Wait();

                _reqDefForAll2Id = reqDefForAll2.Id;
                _tagReqForAll1Id = standardTagNotStartedInFirstStep.Requirements.Single(r => r.RequirementDefinitionId == reqDefForAll1.Id).Id;
                _tagReqForSupplierId = standardTagNotStartedInFirstStep.Requirements.Single(r => r.RequirementDefinitionId == reqDefForSupplier.Id).Id;
                _reqDefForOther2Id = reqDefForOther2.Id;
                _tagReqForOther1Id = standardTagNotStartedInFirstStep.Requirements.Single(r => r.RequirementDefinitionId == reqDefForOther1.Id).Id;
                _standardTagNotStartedInFirstStepId = _tagWithAllReqsId = standardTagNotStartedInFirstStep.Id;
                _standardTagStartedAndInLastStepId = _tagWithOneReqsId = standardTagStartedInLastStep.Id;
                _tagWithTwoReqsId = standardTagWithTwoReqsInLastStep.Id;
                _preAreaTagNotStartedInFirstStepId = preAreaTagNotStartedInFirstStep.Id;
                _preAreaTagStartedInFirstStepId = preAreaTagStartedInFirstStep.Id;
                _siteAreaTagNotStartedId = siteAreaTagNotStarted.Id;
                _poAreaTagNotStartedId = poAreaTagNotStarted.Id;
                _poAreaTagWithTwoReqsId = poAreaTagWithTwoReqs.Id;
                _siteAreaTagStartedId = siteAreaTagStarted.Id;
                _poAreaTagStartedId = poAreaTagStarted.Id;
                _standardTagCompletedId = standardTagCompleted.Id;
                _poAreaTagActionId = action.Id;
                _poAreaTagAttachmentId = tagAttachment.Id;
                _poAreaTagActionAttachmentId = actionAttachment.Id;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownTagNoInKnownProject_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsAsync(TagNo1, ProjectName, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownTagNoInUnknownProject_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsAsync(TagNo1, "XYZ", default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task ExistsRequirementAsync_KnownIds_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsRequirementAsync(
                    _poAreaTagStartedId,
                    _tagRequirementForPoAreaTag.Id,
                    default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsRequirementAsync_UnknownTagId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsRequirementAsync(
                    9999,
                    _tagRequirementForPoAreaTag.Id,
                    default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsRequirementAsync_UnknownRequirementId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsRequirementAsync(
                    _poAreaTagStartedId,
                    9999,
                    default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task ExistsFieldForRequirementAsync_KnownIds_ShouldReturnTrue()
        {
            var fieldId = 3654;
            var requirementDefinitionValidatorMock = new Mock<IRequirementDefinitionValidator>();
            requirementDefinitionValidatorMock.Setup(
                    r => r.ExistsFieldAsync(_tagRequirementForPoAreaTag.RequirementDefinitionId, fieldId, default))
                .Returns(Task.FromResult(true));
            
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, requirementDefinitionValidatorMock.Object);
                var result = await dut.ExistsFieldForRequirementAsync(
                    _poAreaTagStartedId,
                    _tagRequirementForPoAreaTag.Id,
                    fieldId,
                    default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsFieldForRequirementAsync_UnknownTagId_ShouldReturnFalse()
        {
            var fieldId = 3654;
            var requirementDefinitionValidatorMock = new Mock<IRequirementDefinitionValidator>();
            requirementDefinitionValidatorMock.Setup(
                    r => r.ExistsFieldAsync(_tagRequirementForPoAreaTag.RequirementDefinitionId, fieldId, default))
                .Returns(Task.FromResult(true));
            
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, requirementDefinitionValidatorMock.Object);
                var result = await dut.ExistsFieldForRequirementAsync(
                    9999,
                    _tagRequirementForPoAreaTag.Id,
                    fieldId,
                    default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsFieldForRequirementAsync_UnknownRequirementId_ShouldReturnFalse()
        {
            var fieldId = 3654;
            var requirementDefinitionValidatorMock = new Mock<IRequirementDefinitionValidator>();
            requirementDefinitionValidatorMock.Setup(
                    r => r.ExistsFieldAsync(_tagRequirementForPoAreaTag.RequirementDefinitionId, fieldId, default))
                .Returns(Task.FromResult(true));
            
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, requirementDefinitionValidatorMock.Object);
                var result = await dut.ExistsFieldForRequirementAsync(
                    _poAreaTagStartedId,
                    9999,
                    fieldId,
                    default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsFieldForRequirementAsync_UnknownFieldId_ShouldReturnFalse()
        {
            var fieldId = 3654;
            var requirementDefinitionValidatorMock = new Mock<IRequirementDefinitionValidator>();
            requirementDefinitionValidatorMock.Setup(
                    r => r.ExistsFieldAsync(_tagRequirementForPoAreaTag.RequirementDefinitionId, fieldId, default))
                .Returns(Task.FromResult(false));
            
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, requirementDefinitionValidatorMock.Object);
                var result = await dut.ExistsFieldForRequirementAsync(
                    _poAreaTagStartedId,
                    _tagRequirementForPoAreaTag.Id,
                    fieldId,
                    default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task ExistsActionAsync_KnownIds_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsActionAsync(
                    _poAreaTagStartedId,
                    _poAreaTagActionId,
                    default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsActionAsync_UnknownTagId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsActionAsync(
                    9999,
                    _poAreaTagActionId,
                    default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsActionAsync_UnknownActionId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsActionAsync(
                    _poAreaTagStartedId,
                    9999,
                    default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task ExistsTagAttachmentAsync_KnownIds_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsTagAttachmentAsync(
                    _poAreaTagStartedId,
                    _poAreaTagAttachmentId,
                    default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsTagAttachmentAsync_UnknownTagId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsTagAttachmentAsync(
                    9999,
                    _poAreaTagAttachmentId,
                    default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsTagAttachmentAsync_UnknownAttachmentId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsTagAttachmentAsync(
                    _poAreaTagStartedId,
                    9999,
                    default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsActionAttachmentAsync_KnownIds_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsActionAttachmentAsync(
                    _poAreaTagStartedId,
                    _poAreaTagActionId,
                    _poAreaTagActionAttachmentId,
                    default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsActionAttachmentAsync_UnknownTagId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsActionAttachmentAsync(
                    9999,
                    _poAreaTagActionId,
                    _poAreaTagActionAttachmentId,
                    default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsActionAttachmentAsync_UnknownActionId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsActionAttachmentAsync(
                    _poAreaTagStartedId,
                    9999,
                    _poAreaTagActionAttachmentId,
                    default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsActionAttachmentAsync_UnknownAttachmentId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsActionAttachmentAsync(
                    _poAreaTagStartedId,
                    _poAreaTagActionId,
                    9999,
                    default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownTagNoInKnownProject_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsAsync("X", ProjectName, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownTagNoInProjectForTag_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsAsync(TagNo1, _siteAreaTagStartedId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownTagNoInProjectForTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.ExistsAsync("X", _siteAreaTagStartedId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_NotVoidedTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsVoidedAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_VoidedTag_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Single(t => t.Id == _standardTagNotStartedInFirstStepId);
                tag.IsVoided = true;
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsVoidedAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_UnknownTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsVoidedAsync(0, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasANonVoidedRequirementAsync_KnownTag_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.HasANonVoidedRequirementAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HasANonVoidedRequirementAsync_KnownTag_ShouldReturnTrue_AfterVoidingOne()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _standardTagNotStartedInFirstStepId);
                var req = tag.Requirements.First();
                tag.UpdateRequirement(req.Id, true, req.IntervalWeeks, req.RowVersion.ConvertToString());
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.HasANonVoidedRequirementAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task VerifyTagTypeAsync_StandardTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.VerifyTagTypeAsync(_standardTagNotStartedInFirstStepId, TagType.PoArea, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task VerifyTagTypeAsync_UnknownTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.VerifyTagTypeAsync(1283, TagType.PoArea, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task VerifyTagTypeAsync_KnownPoAreaTag_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.VerifyTagTypeAsync(_poAreaTagNotStartedId, TagType.PoArea, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task VerifyAnyOfTagTypeAsync_StandardTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.VerifyTagIsAreaTagAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task VerifyAnyOfTagTypeAsync_UnknownTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.VerifyTagIsAreaTagAsync(1283, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task VerifyAnyOfTagTypeAsync_KnownPoAreaTag_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.VerifyTagIsAreaTagAsync(_poAreaTagNotStartedId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task VerifyPreservationStatusAsync_KnownTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.VerifyPreservationStatusAsync(_standardTagNotStartedInFirstStepId, PreservationStatus.Completed, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task VerifyPreservationStatusAsync_KnownTag_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.VerifyPreservationStatusAsync(_standardTagNotStartedInFirstStepId, PreservationStatus.NotStarted, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ReadyToBePreservedAsync_KnownTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                _timeProvider.ElapseWeeks(IntervalWeeks);
                var result = await dut.IsReadyToBePreservedAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ReadyToBePreservedAsync_KnownTag_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                _timeProvider.ElapseWeeks(IntervalWeeks);
                var result = await dut.IsReadyToBePreservedAsync(_standardTagStartedAndInLastStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task RequirementIsReadyToBePreservedAsync_KnownTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _standardTagNotStartedInFirstStepId);
                var req = tag.Requirements.First();
                var dut = new TagValidator(context, null);
                var result = await dut.RequirementIsReadyToBePreservedAsync(_standardTagNotStartedInFirstStepId, req.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task RequirementIsReadyToBePreservedAsync_KnownTag_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _standardTagStartedAndInLastStepId);
                var req = tag.Requirements.Single();
                var dut = new TagValidator(context, null);
                var result = await dut.RequirementIsReadyToBePreservedAsync(_standardTagStartedAndInLastStepId, req.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task RequirementIsReadyToBePreservedAsync_KnownTag_ShouldReturnTrue_WhenStartedInSeparateContext()
        {
            int reqId;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods).Single(t => t.Id == _standardTagNotStartedInFirstStepId);
                reqId = tag.Requirements.First().Id;
                tag.StartPreservation();
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.RequirementIsReadyToBePreservedAsync(_standardTagNotStartedInFirstStepId, reqId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HasRequirementWithActivePeriodAsync_KnownTag_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _standardTagStartedAndInLastStepId);
                var req = tag.Requirements.Single();
                var dut = new TagValidator(context, null);
                var result = await dut.HasRequirementWithActivePeriodAsync(_standardTagStartedAndInLastStepId, req.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HasRequirementAsync_KnownTag_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _standardTagStartedAndInLastStepId);
                var req = tag.Requirements.Single();
                var dut = new TagValidator(context, null);
                var result = await dut.HasRequirementAsync(_standardTagStartedAndInLastStepId, req.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HasRequirementAsync_UnknownTag_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _standardTagStartedAndInLastStepId);
                var req = tag.Requirements.Single();
                var dut = new TagValidator(context, null);
                var result = await dut.HasRequirementAsync(8181, req.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasRequirementAsync_UnknownReq_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.HasRequirementAsync(_standardTagStartedAndInLastStepId, 8181, default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task IsReadyToBeTransferredAsync_StandardTagNotStarted_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeTransferredAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeTransferredAsync_StandardTagInLastStep_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeTransferredAsync(_standardTagStartedAndInLastStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeTransferredAsync_PreAreaTagNotStarted_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeTransferredAsync(_preAreaTagNotStartedInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeTransferredAsync_PreAreaTagInFirstStep_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeTransferredAsync(_preAreaTagStartedInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeTransferredAsync_SiteAreaTag_AlwaysReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeTransferredAsync(_siteAreaTagStartedId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeTransferredAsync_PoAreaTag_AlwaysReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeTransferredAsync(_poAreaTagStartedId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeTransferredAsync_UnknownTag_AlwaysReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeTransferredAsync(0, default);
                Assert.IsFalse(result);
            }
        }
 
        [TestMethod]
        public async Task IsReadyToBeCompletedAsync_StandardTagNotStarted_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeCompletedAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeCompletedAsync_StandardTagInLastStep_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeCompletedAsync(_standardTagStartedAndInLastStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeCompletedAsync_PreAreaTagNotStarted_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeCompletedAsync(_preAreaTagNotStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeCompletedAsync_PreAreaTagInFirstStep_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeCompletedAsync(_preAreaTagStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeCompletedAsync_SiteAreaTagInAnyStep_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeCompletedAsync(_siteAreaTagStartedId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeCompletedAsync_PoAreaTagInAnyStep_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeCompletedAsync(_poAreaTagStartedId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeCompletedAsync_UnknownTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeCompletedAsync(0, default);
                Assert.IsFalse(result);
            }
        }
 
        [TestMethod]
        public async Task IsReadyToBeStartedAsync_StandardTagNotStarted_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeStartedAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStartedAsync_StandardTagAlreadyStarted_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeStartedAsync(_standardTagStartedAndInLastStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStartedAsync_PreAreaTagNotStarted_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeStartedAsync(_preAreaTagNotStartedInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStartedAsync_PreAreaTagAlreadyStarted_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeStartedAsync(_preAreaTagStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStartedAsync_SiteAreaTagNotStarted_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeStartedAsync(_siteAreaTagNotStartedId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStartedAsync_SiteAreaTagAlreadyStarted_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeStartedAsync(_siteAreaTagStartedId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStartedAsync_PoAreaTagNotStarted_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeStartedAsync(_poAreaTagNotStartedId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStartedAsync_PoAreaTagAlreadyStarted_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeStartedAsync(_poAreaTagStartedId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStartedAsync_UnknownTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeStartedAsync(0, default);
                Assert.IsFalse(result);
            }
        }
     
        [TestMethod]
        public async Task IsReadyToBeRescheduledAsync_TagNotStarted_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeRescheduledAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeRescheduledAsync_TagStarted_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeRescheduledAsync(_standardTagStartedAndInLastStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeRescheduledAsync_UnknownTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeRescheduledAsync(0, default);
                Assert.IsFalse(result);
            }
        }
                    
        [TestMethod]
        public async Task IsReadyToBeDuplicatedAsync_StandardTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeDuplicatedAsync(_standardTagStartedAndInLastStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeDuplicatedAsync_PreAreaTag_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeDuplicatedAsync(_preAreaTagNotStartedInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeDuplicatedAsync_SiteAreaTag_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeDuplicatedAsync(_siteAreaTagStartedId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeDuplicatedAsync_PoAreaTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeDuplicatedAsync(_poAreaTagStartedId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeDuplicatedAsync_UnknownTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsReadyToBeDuplicatedAsync(0, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AttachmentWithFilenameExistsAsync_UnknownTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.AttachmentWithFilenameExistsAsync(0, "A", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AttachmentWithFilenameExistsAsync_UnknownFilename_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.AttachmentWithFilenameExistsAsync(_preAreaTagNotStartedInFirstStepId, "A", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AttachmentWithFilenameExistsAsync_KnownFilename_ShouldReturnTrue()
        {
            var fileName = "A.txt";
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Single(t => t.Id == _standardTagNotStartedInFirstStepId);
                tag.AddAttachment(new TagAttachment(TestPlant, Guid.Empty, fileName));
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.AttachmentWithFilenameExistsAsync(_standardTagNotStartedInFirstStepId, fileName, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task RequirementsWillBeUniqueAfterAddingNewAsync_ShouldReturnFalse_WhenAddingSameRequirementDefinitionAgain()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithAllReqsId);
                var reqDefId = tag.Requirements.ElementAt(0).RequirementDefinitionId;
                var dut = new TagValidator(context, null);
                var result = await dut.AllRequirementsWillBeUniqueAsync(tag.Id, new List<int>{reqDefId}, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task RequirementsWillBeUniqueAfterAddingNewAsync_ShouldReturnTrue_WhenAddingNewRequirementDefinition()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithOneReqsId);
                var dut = new TagValidator(context, null);
                var result = await dut.AllRequirementsWillBeUniqueAsync(tag.Id, new List<int>{_reqDefForAll2Id}, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task RequirementUsageWillCoverBothForSupplierAndOtherAsync_ShouldReturnTrue_WhenRequirementsCoversAll()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithAllReqsId);
                var dut = new TagValidator(context, new RequirementDefinitionValidator(context));
                var result = await dut.RequirementUsageWillCoverBothForSupplierAndOtherAsync(
                    tag.Id,
                    new List<int>(),
                    new List<int>(),
                    new List<int>(),
                    default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task RequirementUsageWillCoverBothForSupplierAndOtherAsync_ShouldReturnFalse_WhenVoidingRequirementsForAllAndForSupplier()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithAllReqsId);
                var dut = new TagValidator(context, new RequirementDefinitionValidator(context));
                var result = await dut.RequirementUsageWillCoverBothForSupplierAndOtherAsync(
                    tag.Id, 
                    new List<int>(),
                    new List<int>{_tagReqForAll1Id, _tagReqForSupplierId},
                    new List<int>(),
                    default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task RequirementUsageWillCoverBothForSupplierAndOtherAsync_ShouldReturnFalse_WhenVoidingRequirementsForAllAndForOther()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithAllReqsId);
                var dut = new TagValidator(context, new RequirementDefinitionValidator(context));
                var result = await dut.RequirementUsageWillCoverBothForSupplierAndOtherAsync(
                    tag.Id,
                    new List<int>(),
                    new List<int>{_tagReqForAll1Id, _tagReqForOther1Id}, 
                    new List<int>(), 
                    default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task RequirementUsageWillCoverBothForSupplierAndOtherAsync_ShouldReturnTrue_WhenVoidingAllRequirementsAndAddingNew()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithAllReqsId);
                var dut = new TagValidator(context, new RequirementDefinitionValidator(context));
                var result = await dut.RequirementUsageWillCoverBothForSupplierAndOtherAsync(
                    tag.Id,
                    new List<int>(),
                    new List<int>{_tagReqForAll1Id, _tagReqForSupplierId}, 
                    new List<int>{_reqDefForAll2Id}, 
                    default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task RequirementUsageWillCoverBothForSupplierAndOtherAsync_ShouldReturnTrue_WhenVoidingAndUnvoidingRequirement()
        {
            int reqId1;
            int reqId2;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _poAreaTagWithTwoReqsId);
                var req = tag.Requirements.First();
                req.IsVoided = true;
                reqId1 = req.Id;
                context.SaveChangesAsync().Wait();
                reqId2 = tag.Requirements.Last().Id;
            }
            
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _poAreaTagWithTwoReqsId);
                var dut = new TagValidator(context, new RequirementDefinitionValidator(context));
                var result = await dut.RequirementUsageWillCoverBothForSupplierAndOtherAsync(
                    tag.Id,
                    new List<int>{reqId1},
                    new List<int>{reqId2}, 
                    new List<int>(), 
                    default);
                Assert.IsTrue(result);
            }
        }
        
        [TestMethod]
        public async Task RequirementUsageWillCoverForSuppliersAsync_ShouldReturnTrue_WhenRequirementsCoversAll()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithAllReqsId);
                var dut = new TagValidator(context, new RequirementDefinitionValidator(context));
                var result = await dut.RequirementUsageWillCoverForSuppliersAsync(
                    tag.Id,
                    new List<int>(),
                    new List<int>(),
                    new List<int>(),
                    default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task RequirementUsageWillCoverForSuppliersAsync_ShouldReturnFalse_WhenVoidingRequirementsForAllAndForSupplier()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithAllReqsId);
                var dut = new TagValidator(context, new RequirementDefinitionValidator(context));
                var result = await dut.RequirementUsageWillCoverForSuppliersAsync(
                    tag.Id, 
                    new List<int>(),
                    new List<int>{_tagReqForAll1Id, _tagReqForSupplierId},
                    new List<int>(),
                    default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task RequirementUsageWillCoverForSuppliersAsync_ShouldReturnTrue_WhenVoidingRequirementsForAllAndForOther()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithAllReqsId);
                var dut = new TagValidator(context, new RequirementDefinitionValidator(context));
                var result = await dut.RequirementUsageWillCoverForSuppliersAsync(
                    tag.Id,
                    new List<int>(),
                    new List<int>{_tagReqForAll1Id, _tagReqForOther1Id}, 
                    new List<int>(), 
                    default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task RequirementUsageWillCoverForSuppliersAsync_ShouldReturnTrue_WhenVoidingAllRequirementsAndAddingNew()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithAllReqsId);
                var dut = new TagValidator(context, new RequirementDefinitionValidator(context));
                var result = await dut.RequirementUsageWillCoverForSuppliersAsync(
                    tag.Id,
                    new List<int>(),
                    new List<int>{_tagReqForAll1Id, _tagReqForSupplierId}, 
                    new List<int>{_reqDefForAll2Id}, 
                    default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task RequirementUsageWillCoverForOtherThanSuppliersAsync_ShouldReturnTrue_WhenRequirementsCoversAll()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithAllReqsId);
                var dut = new TagValidator(context, new RequirementDefinitionValidator(context));
                var result = await dut.RequirementUsageWillCoverForOtherThanSuppliersAsync(
                    tag.Id,
                    new List<int>(),
                    new List<int>(),
                    new List<int>(),
                    default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task RequirementUsageWillCoverForOtherThanSuppliersAsync_ShouldReturnFalse_WhenVoidingRequirementsForAllAndForOther()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithAllReqsId);
                var dut = new TagValidator(context, new RequirementDefinitionValidator(context));
                var result = await dut.RequirementUsageWillCoverForOtherThanSuppliersAsync(
                    tag.Id, 
                    new List<int>(),
                    new List<int>{_tagReqForAll1Id, _tagReqForOther1Id}, 
                    new List<int>(), 
                    default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task RequirementUsageWillCoverForOtherThanSuppliersAsync_ShouldReturnTrue_WhenVoidingAllRequirementsAndAddingNew()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithAllReqsId);
                var dut = new TagValidator(context, new RequirementDefinitionValidator(context));
                var result = await dut.RequirementUsageWillCoverForOtherThanSuppliersAsync(
                    tag.Id, 
                    new List<int>(),
                    new List<int>{_tagReqForAll1Id, _tagReqForOther1Id}, 
                    new List<int>{_reqDefForAll2Id}, 
                    default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task RequirementUsageWillCoverForOtherThanSuppliersAsync_ShouldReturnTrue_WhenVoidingAndUnvoidingRequirement()
        {
            int reqId1;
            int reqId2;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithTwoReqsId);
                var req = tag.Requirements.First();
                req.IsVoided = true;
                reqId1 = req.Id;
                context.SaveChangesAsync().Wait();
                reqId2 = tag.Requirements.Last().Id;
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithTwoReqsId);
                var dut = new TagValidator(context, new RequirementDefinitionValidator(context));
                var result = await dut.RequirementUsageWillCoverForOtherThanSuppliersAsync(
                    tag.Id, 
                    new List<int>{reqId1},
                    new List<int>{reqId2}, 
                    new List<int>(), 
                    default);
                Assert.IsTrue(result);
            }
        }
        
        [TestMethod]
        public async Task RequirementHasAnyForOtherThanSuppliersUsageAsync_ShouldReturnTrue_WhenRequirementsCoversAll()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithAllReqsId);
                var dut = new TagValidator(context, new RequirementDefinitionValidator(context));
                var result = await dut.RequirementHasAnyForOtherThanSuppliersUsageAsync(
                    tag.Id,
                    new List<int>(),
                    new List<int>(),
                    new List<int>(),
                    default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task RequirementHasAnyForOtherThanSuppliersUsageAsync_ShouldReturnFalse_WhenVoidingRequirementsForOther()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithAllReqsId);
                var dut = new TagValidator(context, new RequirementDefinitionValidator(context));
                var result = await dut.RequirementHasAnyForOtherThanSuppliersUsageAsync(
                    tag.Id, 
                    new List<int>(),
                    new List<int>{_tagReqForOther1Id}, 
                    new List<int>(), 
                    default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task RequirementHasAnyForOtherThanSuppliersUsageAsync_ShouldReturnTrue_WhenVoidinOtherRequirementsAndAddingNew()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _tagWithAllReqsId);
                var dut = new TagValidator(context, new RequirementDefinitionValidator(context));
                var result = await dut.RequirementHasAnyForOtherThanSuppliersUsageAsync(
                    tag.Id, 
                    new List<int>(),
                    new List<int>{_tagReqForOther1Id}, 
                    new List<int>{_reqDefForOther2Id}, 
                    default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsInUseAsync_StatusActive_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                // Assert setup
                var tag = context.Tags
                    .Include(t => t.Actions)
                    .Include(t => t.Attachments)
                    .Single(t => t.Id == _standardTagStartedAndInLastStepId);

                Assert.AreEqual(0, tag.Attachments.Count);
                Assert.AreEqual(0, tag.Actions.Count);
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {

                var dut = new TagValidator(context, null);
                var result = await dut.IsInUseAsync(_standardTagStartedAndInLastStepId, default);

                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsInUseAsync_StatusCompleted_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                // Assert setup
                var tag = context.Tags
                    .Include(t => t.Actions)
                    .Include(t => t.Attachments)
                    .Single(t => t.Id == _standardTagCompletedId);

                Assert.AreEqual(0, tag.Attachments.Count);
                Assert.AreEqual(0, tag.Actions.Count);
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsInUseAsync(_standardTagCompletedId, default);

                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsInUseAsync_HasAction_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags
                    .Include(t => t.Actions)
                    .Include(t => t.Attachments)
                    .Single(t => t.Id == _standardTagNotStartedInFirstStepId);

                Assert.AreEqual(0, tag.Attachments.Count);
                Assert.AreEqual(PreservationStatus.NotStarted, tag.Status);

                tag.AddAction(new Action(TestPlant, "", "", null));
                context.SaveChangesAsync().Wait();
                Assert.AreEqual(1, tag.Actions.Count);
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsInUseAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsInUseAsync_HasAttachment_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags
                    .Include(t => t.Actions)
                    .Include(t => t.Attachments)
                    .Single(t => t.Id == _standardTagNotStartedInFirstStepId);

                Assert.AreEqual(0, tag.Actions.Count);
                Assert.AreEqual(PreservationStatus.NotStarted, tag.Status);

                tag.AddAttachment(new TagAttachment(TestPlant, Guid.Empty, "A.txt"));
                context.SaveChangesAsync().Wait();
                Assert.AreEqual(1, tag.Attachments.Count);
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsInUseAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsInUseAsync_NotInUse_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsInUseAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsInUseAsync_UnknownTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsInUseAsync(0, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasStepAsync_UnknownTagId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.HasStepAsync(126234, _firstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasStepAsync_UnknownStepId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.HasStepAsync(_standardTagNotStartedInFirstStepId, 123456, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasStepAsync_KnownIds_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.HasStepAsync(_standardTagNotStartedInFirstStepId, _firstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task VerifyTagDescriptionAsync_KnownTag_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.VerifyTagDescriptionAsync(_standardTagNotStartedInFirstStepId, $"Changed {_tagDescription}", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task VerifyTagDescriptionAsync_KnownTag_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.VerifyTagDescriptionAsync(_standardTagNotStartedInFirstStepId, _tagDescription, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsRequirementVoidedAsync_WhenRequirementIsVoided_ShouldReturnTrue()
        {
            int reqId;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _poAreaTagStartedId);
                var req = tag.Requirements.First();
                req.IsVoided = true;
                reqId = req.Id;
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsRequirementVoidedAsync(_poAreaTagStartedId, reqId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsRequirementVoidedAsync_UnknownTagId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsRequirementVoidedAsync(9999, _tagRequirementForPoAreaTag.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsRequirementVoidedAsync_UnknownRequirementId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context, null);
                var result = await dut.IsRequirementVoidedAsync(_poAreaTagStartedId, 9999, default);
                Assert.IsFalse(result);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class TagValidatorTests : ReadOnlyTestsBase
    {
        private const string ProjectName = "P";
        private const string TagNo1 = "PA-13";
        private const string TagNo2 = "PA-14";
        private int _standardTagNotStartedInFirstStepId;
        private int _standardTagStartedAndInLastStepId;
        private int _preAreaTagNotStartedInFirstStepId;
        private int _preAreaTagStartedInFirstStepId;
        private int _siteAreaTagNotStartedId;
        private int _poAreaTagNotStartedId;
        private int _siteAreaTagStartedId;
        private int _poAreaTagStartedId;
        private const int IntervalWeeks = 4;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var project = AddProject(context, ProjectName, "Project description");
                var journey = AddJourneyWithStep(context, "J", "S1", AddMode(context, "M1"), AddResponsible(context, "R1"));
                journey.AddStep(new Step(TestPlant, "S2", AddMode(context, "M2"), AddResponsible(context, "R2")));

                var rd = AddRequirementTypeWith1DefWithoutField(context, "Rot", "D").RequirementDefinitions.First();

                var standardTagNotStartedInFirstStep = AddTag(context, project, TagType.Standard, TagNo1,
                    "Tag description", journey.Steps.First(),
                    new List<TagRequirement> {new TagRequirement(TestPlant, IntervalWeeks, rd)});

                var standardTagStartedInLastStep = AddTag(context, project, TagType.Standard, TagNo2, "",
                    journey.Steps.Last(), new List<TagRequirement> {new TagRequirement(TestPlant, IntervalWeeks, rd)});
                standardTagStartedInLastStep.StartPreservation();

                var preAreaTagNotStartedInFirstStep = AddTag(context, project, TagType.PreArea, "#PRE-E-A1", "Tag description",
                    journey.Steps.First(), new List<TagRequirement> {new TagRequirement(TestPlant, IntervalWeeks, rd)});
                var preAreaTagStartedInFirstStep = AddTag(context, project, TagType.PreArea, "#PRE-E-A2", "Tag description",
                    journey.Steps.First(), new List<TagRequirement> {new TagRequirement(TestPlant, IntervalWeeks, rd)});
                preAreaTagStartedInFirstStep.StartPreservation();
                var siteAreaTagNotStarted = AddTag(context, project, TagType.SiteArea, "#SITE-E-A1", "Tag description", 
                    journey.Steps.First(), new List<TagRequirement> {new TagRequirement(TestPlant, IntervalWeeks, rd)});
                var siteAreaTagStarted = AddTag(context, project, TagType.SiteArea, "#SITE-E-A2", "Tag description", 
                    journey.Steps.First(), new List<TagRequirement> {new TagRequirement(TestPlant, IntervalWeeks, rd)});
                siteAreaTagStarted.StartPreservation();
                var poAreaTagNotStarted = AddTag(context, project, TagType.PoArea, "#PO-E-A1", "Tag description",
                    journey.Steps.First(), new List<TagRequirement> {new TagRequirement(TestPlant, IntervalWeeks, rd)});
                var poAreaTagStarted = AddTag(context, project, TagType.PoArea, "#PO-E-A2", "Tag description",
                    journey.Steps.First(), new List<TagRequirement> {new TagRequirement(TestPlant, IntervalWeeks, rd)});
                poAreaTagStarted.StartPreservation();

                _standardTagNotStartedInFirstStepId = standardTagNotStartedInFirstStep.Id;
                _standardTagStartedAndInLastStepId = standardTagStartedInLastStep.Id;
                _preAreaTagNotStartedInFirstStepId = preAreaTagNotStartedInFirstStep.Id;
                _preAreaTagStartedInFirstStepId = preAreaTagStartedInFirstStep.Id;
                _siteAreaTagNotStartedId = siteAreaTagNotStarted.Id;
                _poAreaTagNotStartedId = poAreaTagNotStarted.Id;
                _siteAreaTagStartedId = siteAreaTagStarted.Id;
                _poAreaTagStartedId = poAreaTagStarted.Id;

                context.SaveChangesAsync().Wait();
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.ExistsAsync(TagNo1, ProjectName, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.ExistsAsync("X", ProjectName, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_NotVoidedTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsVoidedAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_VoidedTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Single(t => t.Id == _standardTagNotStartedInFirstStepId);
                tag.Void();
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsVoidedAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_UnknownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsVoidedAsync(0, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasANonVoidedRequirementAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.HasANonVoidedRequirementAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HasANonVoidedRequirementAsync_KnownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _standardTagNotStartedInFirstStepId);
                var req = tag.Requirements.Single();
                req.Void();
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.HasANonVoidedRequirementAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task VerifyPreservationStatusAsync_KnownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.VerifyPreservationStatusAsync(_standardTagNotStartedInFirstStepId, PreservationStatus.Completed, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task VerifyPreservationStatusAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.VerifyPreservationStatusAsync(_standardTagNotStartedInFirstStepId, PreservationStatus.NotStarted, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ReadyToBePreservedAsync_KnownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                _timeProvider.ElapseWeeks(IntervalWeeks);
                var result = await dut.ReadyToBePreservedAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ReadyToBePreservedAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                _timeProvider.ElapseWeeks(IntervalWeeks);
                var result = await dut.ReadyToBePreservedAsync(_standardTagStartedAndInLastStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task RequirementIsReadyToBePreservedAsync_KnownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _standardTagNotStartedInFirstStepId);
                var req = tag.Requirements.Single();
                var dut = new TagValidator(context);
                var result = await dut.RequirementIsReadyToBePreservedAsync(_standardTagNotStartedInFirstStepId, req.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task RequirementIsReadyToBePreservedAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _standardTagStartedAndInLastStepId);
                var req = tag.Requirements.Single();
                var dut = new TagValidator(context);
                var result = await dut.RequirementIsReadyToBePreservedAsync(_standardTagStartedAndInLastStepId, req.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task RequirementIsReadyToBePreservedAsync_KnownTag_ReturnsTrue_WhenStartedInSeparateContext()
        {
            int reqId;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods).Single(t => t.Id == _standardTagNotStartedInFirstStepId);
                reqId = tag.Requirements.Single().Id;
                tag.StartPreservation();
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.RequirementIsReadyToBePreservedAsync(_standardTagNotStartedInFirstStepId, reqId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HasRequirementWithActivePeriodAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).Single(t => t.Id == _standardTagStartedAndInLastStepId);
                var req = tag.Requirements.Single();
                var dut = new TagValidator(context);
                var result = await dut.HasRequirementWithActivePeriodAsync(_standardTagStartedAndInLastStepId, req.Id, default);
                Assert.IsTrue(result);
            }
        }
        
        [TestMethod]
        public async Task IsReadyToBeTransferredAsync_StandardTagNotStarted_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeTransferredAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeTransferredAsync_StandardTagInLastStep_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeTransferredAsync(_standardTagStartedAndInLastStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeTransferredAsync_PreAreaTagNotStarted_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeTransferredAsync(_preAreaTagNotStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeTransferredAsync_PreAreaTagInFirstStep_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeTransferredAsync(_preAreaTagStartedInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeTransferredAsync_SiteAreaTag_AlwaysReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeTransferredAsync(_siteAreaTagStartedId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeTransferredAsync_PoAreaTag_AlwaysReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeTransferredAsync(_poAreaTagStartedId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeTransferredAsync_UnknownTag_AlwaysReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeTransferredAsync(0, default);
                Assert.IsFalse(result);
            }
        }
 
        [TestMethod]
        public async Task IsReadyToBeStoppedAsync_StandardTagNotStarted_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeStoppedAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStoppedAsync_StandardTagInLastStep_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeStoppedAsync(_standardTagStartedAndInLastStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStoppedAsync_PreAreaTagNotStarted_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeStoppedAsync(_preAreaTagNotStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStoppedAsync_PreAreaTagInFirstStep_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeStoppedAsync(_preAreaTagStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStoppedAsync_SiteAreaTagInAnyStep_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeStoppedAsync(_siteAreaTagStartedId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStoppedAsync_PoAreaTagInAnyStep_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeStoppedAsync(_poAreaTagStartedId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStoppedAsync_UnknownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeStoppedAsync(0, default);
                Assert.IsFalse(result);
            }
        }
 
        [TestMethod]
        public async Task IsReadyToBeStartedAsync_StandardTagNotStarted_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeStartedAsync(_standardTagNotStartedInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStartedAsync_StandardTagAlreadyStarted_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeStartedAsync(_standardTagStartedAndInLastStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStartedAsync_PreAreaTagNotStarted_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeStartedAsync(_preAreaTagNotStartedInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStartedAsync_PreAreaTagAlreadyStarted_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeStartedAsync(_preAreaTagStartedInFirstStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStartedAsync_SiteAreaTagNotStarted_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeStartedAsync(_siteAreaTagNotStartedId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStartedAsync_SiteAreaTagAlreadyStarted_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeStartedAsync(_siteAreaTagStartedId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStartedAsync_PoAreaTagNotStarted_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeStartedAsync(_poAreaTagNotStartedId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStartedAsync_PoAreaTagAlreadyStarted_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeStartedAsync(_poAreaTagStartedId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsReadyToBeStartedAsync_UnknownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsReadyToBeStartedAsync(0, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AttachmentWithFilenameExistsAsync_UnknownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.AttachmentWithFilenameExistsAsync(0, "A", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AttachmentWithFilenameExistsAsync_UnknownFilename_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.AttachmentWithFilenameExistsAsync(_preAreaTagNotStartedInFirstStepId, "A", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AttachmentWithFilenameExistsAsync_KnownFilename_ReturnsTrue()
        {
            var fileName = "A.txt";
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Single(t => t.Id == _standardTagNotStartedInFirstStepId);
                tag.AddAttachment(new TagAttachment(TestPlant, Guid.Empty, null, fileName));
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.AttachmentWithFilenameExistsAsync(_standardTagNotStartedInFirstStepId, fileName, default);
                Assert.IsTrue(result);
            }
        }
    }
}

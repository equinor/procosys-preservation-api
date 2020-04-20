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
        private int _tagNotStartedPreservationId;
        private int _tagStartedPreservationId;
        private int _preAreaTagId;
        private int _siteAreaTagId;
        private int _poAreaTagId;
        private int _tagInFirstStepId;
        private int _tagInLastStepId;
        private const int IntervalWeeks = 4;
        private int _reqStartedPreservationId;
        private int _reqNotStartedPreservationId;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var project = AddProject(context, ProjectName, "Project description");
                var journey = AddJourneyWithStep(context, "J", "S1", AddMode(context, "M1"), AddResponsible(context, "R1"));
                journey.AddStep(new Step(TestPlant, "S2", AddMode(context, "M2"), AddResponsible(context, "R2")));

                var rd = AddRequirementTypeWith1DefWithoutField(context, "Rot", "D").RequirementDefinitions.First();
                var reqStartedPreservation = new TagRequirement(TestPlant, IntervalWeeks, rd);
                var reqNotStartedPreservation = new TagRequirement(TestPlant, IntervalWeeks, rd);
                var reqSiteArea = new TagRequirement(TestPlant, IntervalWeeks, rd);

                var tagNotStartedPreservation = AddTag(context, project, TagType.Standard, TagNo1, "Tag description", journey.Steps.First(), new List<TagRequirement> {reqNotStartedPreservation});

                var tagStartedPreservation = AddTag(context, project, TagType.Standard, TagNo2, "", journey.Steps.Last(), new List<TagRequirement>{reqStartedPreservation});
                tagStartedPreservation.StartPreservation();

                var preAreaTag = AddTag(context, project, TagType.PreArea, "#PRE-E-A1", "Tag description",
                    journey.Steps.First(), new List<TagRequirement> {new TagRequirement(TestPlant, IntervalWeeks, rd)});
                var siteAreaTag = AddTag(context, project, TagType.SiteArea, "#SITE-E-A1", "Tag description", 
                    journey.Steps.First(), new List<TagRequirement> {reqSiteArea});
                var poAreaTag = AddTag(context, project, TagType.PoArea, "#PO-E-A1", "Tag description",
                    journey.Steps.First(), new List<TagRequirement> {new TagRequirement(TestPlant, IntervalWeeks, rd)});

                _tagNotStartedPreservationId = _tagInFirstStepId = tagNotStartedPreservation.Id;
                _tagStartedPreservationId = tagStartedPreservation.Id;
                _tagInLastStepId = tagStartedPreservation.Id;
                _preAreaTagId = preAreaTag.Id;
                _siteAreaTagId = siteAreaTag.Id;
                _poAreaTagId = poAreaTag.Id;

                _reqStartedPreservationId = reqStartedPreservation.Id;
                _reqNotStartedPreservationId = reqNotStartedPreservation.Id;
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
                var result = await dut.IsVoidedAsync(_tagNotStartedPreservationId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_VoidedTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Single(t => t.Id == _tagNotStartedPreservationId);
                tag.Void();
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsVoidedAsync(_tagNotStartedPreservationId, default);
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
        public async Task AllRequirementDefinitionsExistAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.AllRequirementDefinitionsExistAsync(_tagNotStartedPreservationId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HasANonVoidedRequirementAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.HasANonVoidedRequirementAsync(_tagNotStartedPreservationId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HasANonVoidedRequirementAsync_KnownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var req = context.TagRequirements.Single(r => r.Id == _reqNotStartedPreservationId);
                req.Void();
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.HasANonVoidedRequirementAsync(_tagNotStartedPreservationId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HaveNextStepAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.HaveNextStepAsync(_tagInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HaveNextStepAsync_KnownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.HaveNextStepAsync(_tagInLastStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task VerifyPreservationStatusAsync_KnownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.VerifyPreservationStatusAsync(_tagNotStartedPreservationId, PreservationStatus.Completed, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task VerifyPreservationStatusAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.VerifyPreservationStatusAsync(_tagNotStartedPreservationId, PreservationStatus.NotStarted, default);
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
                var result = await dut.ReadyToBePreservedAsync(_tagNotStartedPreservationId, default);
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
                var result = await dut.ReadyToBePreservedAsync(_tagStartedPreservationId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task RequirementIsReadyToBePreservedAsync_KnownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.RequirementIsReadyToBePreservedAsync(_tagNotStartedPreservationId, _reqNotStartedPreservationId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task RequirementIsReadyToBePreservedAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.RequirementIsReadyToBePreservedAsync(_tagStartedPreservationId, _reqStartedPreservationId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task RequirementIsReadyToBePreservedAsync_KnownTag_ReturnsTrue_WhenStartedInSeparateContext()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods).Single(t => t.Id == _tagNotStartedPreservationId);
                tag.StartPreservation();
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.RequirementIsReadyToBePreservedAsync(_tagNotStartedPreservationId, _reqNotStartedPreservationId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HasRequirementReadyForRecordingAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.HasRequirementWithActivePeriodAsync(_tagStartedPreservationId, _reqStartedPreservationId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TagFollowsAJourneyAsync_StandardTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.TagFollowsAJourneyAsync(_tagStartedPreservationId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TagFollowsAJourneyAsync_PreAreaTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.TagFollowsAJourneyAsync(_preAreaTagId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TagFollowsAJourneyAsync_SiteAreaTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.TagFollowsAJourneyAsync(_siteAreaTagId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task TagFollowsAJourneyAsync_PoAreaTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.TagFollowsAJourneyAsync(_poAreaTagId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task TagFollowsAJourneyAsync_UnknownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.TagFollowsAJourneyAsync(0, default);
                Assert.IsFalse(result);
            }
        }
    }
}

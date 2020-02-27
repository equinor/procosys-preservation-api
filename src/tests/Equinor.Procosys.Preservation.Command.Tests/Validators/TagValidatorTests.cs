using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.TagValidators;
using Equinor.Procosys.Preservation.Domain;
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
        private int _tagInFirstStepId;
        private int _tagInLastStepId;
        private const int IntervalWeeks = 4;
        private readonly DateTime _startedPreservationAtUtc = new DateTime(2020, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        private int _reqStartedPreservationId;
        private int _reqNotStartedPreservationId;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var project = AddProject(context, ProjectName, "Project description");
                var journey = AddJourneyWithStep(context, "J", AddMode(context, "M1"), AddResponsible(context, "R1"));
                journey.AddStep(new Step(TestPlant, AddMode(context, "M2"), AddResponsible(context, "R2")));

                var rd = AddRequirementTypeWith1DefWithoutField(context, "Rot", "D").RequirementDefinitions.First();
                var reqStartedPreservation = new Requirement(TestPlant, IntervalWeeks, rd);
                var reqNotStartedPreservation = new Requirement(TestPlant, IntervalWeeks, rd);

                var tagNotStartedPreservation = AddTag(context, project, TagNo1, "Tag description", journey.Steps.First(), new List<Requirement> {reqNotStartedPreservation});
                _tagNotStartedPreservationId = _tagInFirstStepId = tagNotStartedPreservation.Id;

                var tagStartedPreservation = AddTag(context, project, TagNo2, "", journey.Steps.Last(), new List<Requirement>{reqStartedPreservation});
                tagStartedPreservation.StartPreservation(_startedPreservationAtUtc);

                _tagStartedPreservationId = tagStartedPreservation.Id;
                _tagInLastStepId = tagStartedPreservation.Id;

                _reqStartedPreservationId = reqStartedPreservation.Id;
                _reqNotStartedPreservationId = reqNotStartedPreservation.Id;
                context.SaveChanges();
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.ExistsAsync(TagNo1, ProjectName, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.ExistsAsync("X", ProjectName, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_NotVoidedTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsVoidedAsync(_tagNotStartedPreservationId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_VoidedTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var tag = context.Tags.Single(t => t.Id == _tagNotStartedPreservationId);
                tag.Void();
                context.SaveChanges();
            }
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsVoidedAsync(_tagNotStartedPreservationId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_UnknownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.IsVoidedAsync(0, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AllRequirementDefinitionsExistAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.AllRequirementDefinitionsExistAsync(_tagNotStartedPreservationId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HasANonVoidedRequirementAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.HasANonVoidedRequirementAsync(_tagNotStartedPreservationId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HasANonVoidedRequirementAsync_KnownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var req = context.Requirements.Single(r => r.Id == _reqNotStartedPreservationId);
                req.Void();
                context.SaveChanges();
            }
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.HasANonVoidedRequirementAsync(_tagNotStartedPreservationId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HaveNextStepAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.HaveNextStepAsync(_tagInFirstStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HaveNextStepAsync_KnownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.HaveNextStepAsync(_tagInLastStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task VerifyPreservationStatusAsync_KnownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.VerifyPreservationStatusAsync(_tagNotStartedPreservationId, PreservationStatus.Completed, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task VerifyPreservationStatusAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.VerifyPreservationStatusAsync(_tagNotStartedPreservationId, PreservationStatus.NotStarted, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ReadyToBePreservedAsync_KnownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.ReadyToBePreservedAsync(_tagNotStartedPreservationId, _startedPreservationAtUtc.AddWeeks(IntervalWeeks), default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ReadyToBePreservedAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.ReadyToBePreservedAsync(_tagStartedPreservationId, _startedPreservationAtUtc.AddWeeks(IntervalWeeks), default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HasRequirementReadyToBePreservedAsync_KnownTag_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.HasRequirementReadyToBePreservedAsync(_tagNotStartedPreservationId, _reqNotStartedPreservationId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasRequirementReadyToBePreservedAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.HasRequirementReadyToBePreservedAsync(_tagStartedPreservationId, _reqStartedPreservationId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HasRequirementReadyToBePreservedAsync_KnownTag_ReturnsTrue_WhenStartedInSeparateContext()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var tag = context.Tags.Include(t => t.Requirements).ThenInclude(r => r.PreservationPeriods).Single(t => t.Id == _tagNotStartedPreservationId);
                tag.StartPreservation(_startedPreservationAtUtc);
                context.SaveChanges();
            }
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.HasRequirementReadyToBePreservedAsync(_tagNotStartedPreservationId, _reqNotStartedPreservationId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HaveRequirementReadyForRecordingAsync_KnownTag_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new TagValidator(context);
                var result = await dut.HaveRequirementWithActivePeriodAsync(_tagStartedPreservationId, _reqStartedPreservationId, default);
                Assert.IsTrue(result);
            }
        }
    }
}

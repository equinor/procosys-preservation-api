using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class JourneyValidatorTests : ReadOnlyTestsBase
    {
        private const string JourneyTitle1 = "Journey1";
        private const string JourneyTitle2 = "Journey2";

        private int _journeyWithStepId;
        private int _emptyJourneyId;
        private Step _step;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var journeyWithStep = AddJourneyWithStep(context, JourneyTitle1, "S", AddMode(context, "M", false), AddResponsible(context, "R"));
                _step = journeyWithStep.Steps.Single();
                _journeyWithStepId = journeyWithStep.Id;
                _emptyJourneyId = AddJourney(context, JourneyTitle2).Id;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsAsync(_journeyWithStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task StepExistsAsync_KnownIds_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.StepExistsAsync(_journeyWithStepId, _step.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task StepExistsAsync_UnknownJourneyId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.StepExistsAsync(126234, _step.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task StepExistsAsync_UnknownStepId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.StepExistsAsync(_journeyWithStepId, 126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleAsync_KnownTitle_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithSameTitleAsync(JourneyTitle1, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleAsync_UnknownTitle_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithSameTitleAsync("XXX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleInAnotherJourneyAsync_SameTitleAsAnotherJourney_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithSameTitleInAnotherJourneyAsync(_journeyWithStepId, JourneyTitle2, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleInAnotherJourneyAsync_NewTitle_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithSameTitleInAnotherJourneyAsync(_journeyWithStepId, "XXXXXX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleInAnotherJourneyAsync_SameTitle_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithSameTitleInAnotherJourneyAsync(_journeyWithStepId, JourneyTitle1, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownVoided_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var journey = context.Journeys.Single(j => j.Id == _journeyWithStepId);
                journey.Void();
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsVoidedAsync(_journeyWithStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownNotVoided_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsVoidedAsync(_journeyWithStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasAnyStepsAsync_JorneyWithNoSteps_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.HasAnyStepsAsync(_emptyJourneyId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasAnyStepsAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.HasAnyStepsAsync(1262, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasAnyStepsAsync_JorneyWithSteps_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.HasAnyStepsAsync(_journeyWithStepId, default);
                Assert.IsTrue(result);
            }
        }
        
        [TestMethod]
        public async Task IsInUseAsync_NoSteps_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsInUseAsync(_emptyJourneyId, default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task IsInUseAsync_NoTagsUsesAnySteps_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsInUseAsync(_journeyWithStepId, default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task IsInUseAsync_UnknownJourney_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsInUseAsync(1267, default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task IsInUseAsync_ReturnsTrue_AfterTagAddedToAStep()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var project = AddProject(context, "P", "Project description");
                var rd = AddRequirementTypeWith1DefWithoutField(context, "Rot", "D").RequirementDefinitions.First();
                AddTag(context, project, TagType.Standard, "TagNo", "Tag description", _step,
                    new List<TagRequirement> {new TagRequirement(TestPlant, 2, rd)});
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsInUseAsync(_journeyWithStepId, default);
                Assert.IsTrue(result);
            }
        }
    }
}

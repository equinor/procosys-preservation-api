using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.JourneyValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class JourneyValidatorTests : ReadOnlyTestsBase
    {
        private const string Journey1WithStepTitle = "Journey1WithStep";
        private const string Journey2WithStepTitle = "Journey2WithStep";
        private const string JourneyWithoutStepTitle = "JourneyWithoutStepTitle";
        private const string StepTitle1InJourney1 = "Step1";
        private const string StepTitle1InJourney2 = "Step2";
        private const string StepTitle2InJourney1 = "Step3";

        private int _journey1WithStepId;
        private int _journey2WithStepId;
        private int _journeyWithoutStepId;
        private int _journeyWithDuplicateId;
        private int _journeyWithoutDuplicateId;
        private Step _step1InJourney1;
        private Step _step2InJourney1;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var responsible = AddResponsible(context, "R1");
                var mode = AddMode(context, "M1", false);
                var journey1WithStep = AddJourneyWithStep(context, Journey1WithStepTitle, StepTitle1InJourney1, mode, responsible);
                _step1InJourney1 = journey1WithStep.Steps.Single();
                _step2InJourney1 = new Step(TestPlant, StepTitle2InJourney1, AddMode(context, "M2", false), AddResponsible(context, "R2"));
                journey1WithStep.AddStep(_step2InJourney1);
                context.SaveChangesAsync().Wait();

                _journey1WithStepId = _journeyWithoutDuplicateId = journey1WithStep.Id;
                _journeyWithoutStepId = _journeyWithDuplicateId = AddJourney(context, JourneyWithoutStepTitle).Id;
                
                _journey2WithStepId = AddJourneyWithStep(context, Journey2WithStepTitle, StepTitle1InJourney2, mode, responsible).Id;

                AddJourney(context, $"{JourneyWithoutStepTitle}{Journey.DuplicatePrefix}");
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsAsync(_journey1WithStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasStepAsync_KnownIds_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.HasStepAsync(_journey1WithStepId, _step1InJourney1.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HasStepAsync_UnknownJourneyId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.HasStepAsync(126234, _step1InJourney1.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasStepAsync_UnknownStepId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.HasStepAsync(_journey1WithStepId, 126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleAsync_KnownTitle_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithSameTitleAsync(Journey1WithStepTitle, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleAsync_UnknownTitle_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithSameTitleAsync("XXX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleInAnotherJourneyAsync_SameTitleAsAnotherJourney_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithSameTitleInAnotherJourneyAsync(_journey1WithStepId, JourneyWithoutStepTitle, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleInAnotherJourneyAsync_NewTitle_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithSameTitleInAnotherJourneyAsync(_journey1WithStepId, "XXXXXX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleInAnotherJourneyAsync_SameTitle_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithSameTitleInAnotherJourneyAsync(_journey1WithStepId, Journey1WithStepTitle, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownVoided_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var journey = context.Journeys.Single(j => j.Id == _journey1WithStepId);
                journey.IsVoided = true;
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsVoidedAsync(_journey1WithStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownNotVoided_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsVoidedAsync(_journey1WithStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_UnknownId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasAnyStepsAsync_JorneyWithNoSteps_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.HasAnyStepsAsync(_journeyWithoutStepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasAnyStepsAsync_UnknownId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.HasAnyStepsAsync(1262, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasAnyStepsAsync_JorneyWithSteps_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.HasAnyStepsAsync(_journey1WithStepId, default);
                Assert.IsTrue(result);
            }
        }
        
        [TestMethod]
        public async Task IsInUseAsync_NoSteps_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsInUseAsync(_journeyWithoutStepId, default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task IsInUseAsync_HaveSteps_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsInUseAsync(_journey1WithStepId, default);
                Assert.IsTrue(result);
            }
        }
        
        [TestMethod]
        public async Task IsInUseAsync_UnknownJourney_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsInUseAsync(1267, default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task IsAnyStepInJourneyInUseAsync_ShouldReturnFalse_WhenNoSteps()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsAnyStepInJourneyInUseAsync(_journeyWithoutStepId, default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task IsAnyStepInJourneyInUseAsync_ShouldReturnFalse_BeforeTagAddedToAStep()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsAnyStepInJourneyInUseAsync(_journey1WithStepId, default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task IsAnyStepInJourneyInUseAsync_ShouldReturnTrue_AfterTagAddedToAStep()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var project = AddProject(context, "P", "Project description");
                var rd = AddRequirementTypeWith1DefWithoutField(context, "Rot", "D", RequirementTypeIcon.Other).RequirementDefinitions.First();
                AddTag(context, project, TagType.Standard, "TagNo", "Tag description", _step1InJourney1,
                    new List<TagRequirement> {new TagRequirement(TestPlant, 2, rd)});
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.IsAnyStepInJourneyInUseAsync(_journey1WithStepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithDuplicateTitleAsync_KnownDuplicate_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithDuplicateTitleAsync(_journeyWithDuplicateId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithDuplicateTitleAsync_KnownNotDuplicate_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithDuplicateTitleAsync(_journeyWithoutDuplicateId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithDuplicateTitleAsync_UnknownKnownId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.ExistsWithDuplicateTitleAsync(92982, default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task HasOtherStepWithAutoTransferMethodAsync_NoSteps_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.HasOtherStepWithAutoTransferMethodAsync(_journeyWithoutStepId, 0, AutoTransferMethod.OnRfccSign, default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task HasOtherStepWithAutoTransferMethodAsync_NoStepsHasSameAutoTransferMethodSign_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.HasOtherStepWithAutoTransferMethodAsync(_journey1WithStepId, _step1InJourney1.Id, AutoTransferMethod.OnRfccSign, default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task HasOtherStepWithAutoTransferMethodAsync_UnknownJourney_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.HasOtherStepWithAutoTransferMethodAsync(1267, 0, AutoTransferMethod.OnRfccSign, default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task HasOtherStepWithAutoTransferMethodAsync_ShouldReturnTrue_WhenOtherStepHasSameAutoTransferMethod()
        {
            var autoTransferMethod = AutoTransferMethod.OnRfccSign;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var journey = context.Journeys.Single(j => j.Id == _journey1WithStepId);
                var step = new Step(TestPlant, "S2", AddMode(context, "M2", false), AddResponsible(context, "R2"))
                {
                    AutoTransferMethod = autoTransferMethod
                };
                journey.AddStep(step);
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.HasOtherStepWithAutoTransferMethodAsync(_journey1WithStepId, _step1InJourney1.Id, autoTransferMethod, default);
                Assert.IsTrue(result);
            }
        }
        
        [TestMethod]
        public async Task HasOtherStepWithAutoTransferMethodAsync_ShouldReturnFalse_WhenSameStepHasSameAutoTransferMethod()
        {
            int stepId;
            var autoTransferMethod = AutoTransferMethod.OnRfccSign;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var journey = context.Journeys.Single(j => j.Id == _journey1WithStepId);
                var step = new Step(TestPlant, "S2", AddMode(context, "M2", false), AddResponsible(context, "R2"))
                {
                    AutoTransferMethod = autoTransferMethod
                };
                journey.AddStep(step);
                context.SaveChangesAsync().Wait();

                stepId = step.Id;
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.HasOtherStepWithAutoTransferMethodAsync(_journey1WithStepId, stepId, autoTransferMethod, default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task AnyStepExistsWithSameTitleAsync_KnownTitleInJourney_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.AnyStepExistsWithSameTitleAsync(_journey1WithStepId, _step1InJourney1.Title, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task AnyStepExistsWithSameTitleAsync_NewTitle_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.AnyStepExistsWithSameTitleAsync(_journey1WithStepId, "AnotherStep", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AnyStepExistsWithSameTitleAsync_KnownTitleInAnotherJourney_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.AnyStepExistsWithSameTitleAsync(_journey2WithStepId, _step1InJourney1.Title, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task OtherStepExistsWithSameTitleAsync_SameTitleAsExisting_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.OtherStepExistsWithSameTitleAsync(_journey1WithStepId, _step1InJourney1.Id, StepTitle1InJourney1, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task OtherStepExistsWithSameTitleAsync_SameTitleAsAnotherStepInSameJourney_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.OtherStepExistsWithSameTitleAsync(_journey1WithStepId, _step1InJourney1.Id, StepTitle2InJourney1, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task OtherStepExistsWithSameTitleAsync_SameTitleAsStepInAnotherJourney_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new JourneyValidator(context);
                var result = await dut.OtherStepExistsWithSameTitleAsync(_journey1WithStepId, _step1InJourney1.Id, StepTitle1InJourney2, default);
                Assert.IsFalse(result);
            }
        }
    }
}

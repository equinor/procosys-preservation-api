using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class StepValidatorTests : ReadOnlyTestsBase
    {
        private Journey _journey1;
        private Journey _journey2;
        private Step _step1InJourney1;
        private const string StepTitle1InJourney1 = "Step1";
        private const string StepTitle1InJourney2 = "Step2";
        private const string StepTitle2InJourney1 = "Step3";

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var mode = AddMode(context, "M", false);
                var responsible = AddResponsible(context, "R");

                _journey1 = AddJourneyWithStep(context, "J1", StepTitle1InJourney1, mode, responsible);
                _step1InJourney1 = _journey1.Steps.Single();
                _journey1.AddStep(new Step(TestPlant, StepTitle2InJourney1, AddMode(context, "M2", false), AddResponsible(context, "R2")));

                _journey2 = AddJourneyWithStep(context, "J2", StepTitle1InJourney2, mode, responsible);

                context.SaveChangesAsync().Wait();
            }
        }

        [TestMethod]
        public async Task Exists_KnownId_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(_step1InJourney1.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task Exists_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task Exists_KnownTitleInJourney_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(_journey1.Id, _step1InJourney1.Title, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task Exists_NewTitle_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(_journey1.Id, "AnotherStep", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task Exists_KnownTitleInAnotherJourney_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(_journey2.Id, _step1InJourney1.Title, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoided_KnownVoided_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var step = context.Steps.Single(s => s.Id == _step1InJourney1.Id);
                step.Void();
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsVoidedAsync(_step1InJourney1.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoided_KnownNotVoided_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsVoidedAsync(_step1InJourney1.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoided_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsInExistingJourney_SameTitleAsExisting_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsInExistingJourneyAsync(_step1InJourney1.Id, StepTitle1InJourney1, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsInExistingJourney_SameTitleAsAnotherStepInSameJourney_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsInExistingJourneyAsync(_step1InJourney1.Id, StepTitle2InJourney1, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsInExistingJourney_SameTitleAsStepInAnotherJourney_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsInExistingJourneyAsync(_step1InJourney1.Id, StepTitle1InJourney2, default);
                Assert.IsFalse(result);
            }
        }
    }
}

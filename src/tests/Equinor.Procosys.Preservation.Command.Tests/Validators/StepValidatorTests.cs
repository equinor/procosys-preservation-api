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
        private Step _step1InJourney1ForSupplier;
        private Step _step2InJourney1;
        private Step _step3InJourney1;
        private const string StepTitle1InJourney1 = "Step1";
        private const string StepTitle1InJourney2 = "Step2";
        private const string StepTitle2InJourney1 = "Step3";
        private const string StepTitle3InJourney1 = "Step4";

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var supplierMode = AddMode(context, "M", true);
                var responsible = AddResponsible(context, "R");

                _journey1 = AddJourneyWithStep(context, "J1", StepTitle1InJourney1, supplierMode, responsible);
                _step1InJourney1ForSupplier = _journey1.Steps.Single();
                _step2InJourney1 = new Step(TestPlant, StepTitle2InJourney1, AddMode(context, "M2", false), AddResponsible(context, "R2"));
                _step3InJourney1 = new Step(TestPlant, StepTitle3InJourney1, AddMode(context, "M3", false), AddResponsible(context, "R3"));

                _journey1.AddStep(_step2InJourney1);
                _journey1.AddStep(_step3InJourney1);

                _journey2 = AddJourneyWithStep(context, "J2", StepTitle1InJourney2, supplierMode, responsible);

                context.SaveChangesAsync().Wait();
            }
        }

        [TestMethod]
        public async Task Exists_KnownId_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(_step1InJourney1ForSupplier.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task Exists_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task Exists_KnownTitleInJourney_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(_journey1.Id, _step1InJourney1ForSupplier.Title, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task Exists_NewTitle_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(_journey1.Id, "AnotherStep", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task Exists_KnownTitleInAnotherJourney_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(_journey2.Id, _step1InJourney1ForSupplier.Title, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoided_KnownVoided_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var step = context.Steps.Single(s => s.Id == _step1InJourney1ForSupplier.Id);
                step.Void();
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsVoidedAsync(_step1InJourney1ForSupplier.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoided_KnownNotVoided_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsVoidedAsync(_step1InJourney1ForSupplier.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoided_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsInExistingJourney_SameTitleAsExisting_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsInExistingJourneyAsync(_step1InJourney1ForSupplier.Id, StepTitle1InJourney1, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsInExistingJourney_SameTitleAsAnotherStepInSameJourney_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsInExistingJourneyAsync(_step1InJourney1ForSupplier.Id, StepTitle2InJourney1, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsInExistingJourney_SameTitleAsStepInAnotherJourney_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsInExistingJourneyAsync(_step1InJourney1ForSupplier.Id, StepTitle1InJourney2, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsAnyStepForSupplier_IncludesSupplierStep_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsAnyStepForSupplier(_step2InJourney1.Id, _step1InJourney1ForSupplier.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsAnyStepForSupplier_NotIncludesSupplierStep_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsAnyStepForSupplier(_step2InJourney1.Id, _step3InJourney1.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsFirstStepOrModeIsNotForSupplier_UpdatingFirstStepToSupplierStep_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsFirstStepOrModeIsNotForSupplier(_journey1.Id, _step1InJourney1ForSupplier.ModeId, _step1InJourney1ForSupplier.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsFirstStepOrModeIsNotForSupplier_UpdatingNotFirstStepToSupplierStep_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsFirstStepOrModeIsNotForSupplier(_journey1.Id, _step1InJourney1ForSupplier.ModeId, _step3InJourney1.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsForSupplier_KnownForSupplier_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsForSupplierAsync(_step1InJourney1ForSupplier.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsForSupplier_KnownNotForSupplier_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsForSupplierAsync(_step3InJourney1.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsForSupplier_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsForSupplierAsync(126234, default);
                Assert.IsFalse(result);
            }
        }
    }
}

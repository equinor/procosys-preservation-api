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
        private Step _step1InJourney1ForSupplier;
        private Step _step2InJourney1;
        private Step _step3InJourney1;
        private const string StepTitle1InJourney1 = "Step1";
        private const string StepTitle2InJourney1 = "Step3";
        private const string StepTitle3InJourney1 = "Step4";

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var supplierMode = AddMode(context, "M", true);

                _journey1 = AddJourneyWithStep(context, "J1", StepTitle1InJourney1, supplierMode, AddResponsible(context, "R1"));
                _step1InJourney1ForSupplier = _journey1.Steps.Single();
                _step2InJourney1 = new Step(TestPlant, StepTitle2InJourney1, AddMode(context, "M2", false), AddResponsible(context, "R2"));
                _step3InJourney1 = new Step(TestPlant, StepTitle3InJourney1, AddMode(context, "M3", false), AddResponsible(context, "R3"));

                _journey1.AddStep(_step2InJourney1);
                _journey1.AddStep(_step3InJourney1);

                context.SaveChangesAsync().Wait();
            }
        }

        [TestMethod]
        public async Task Exists_KnownId_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(_step1InJourney1ForSupplier.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task Exists_UnknownId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoided_KnownVoided_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var step = context.Steps.Single(s => s.Id == _step1InJourney1ForSupplier.Id);
                step.IsVoided = true;
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsVoidedAsync(_step1InJourney1ForSupplier.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoided_KnownNotVoided_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsVoidedAsync(_step1InJourney1ForSupplier.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoided_UnknownId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsAnyStepForSupplier_IncludesSupplierStep_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsAnyStepForSupplier(_step2InJourney1.Id, _step1InJourney1ForSupplier.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsAnyStepForSupplier_NotIncludesSupplierStep_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsAnyStepForSupplier(_step2InJourney1.Id, _step3InJourney1.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsFirstStepOrModeIsNotForSupplier_UpdatingFirstStepToSupplierStep_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsFirstStepOrModeIsNotForSupplier(_journey1.Id, _step1InJourney1ForSupplier.ModeId, _step1InJourney1ForSupplier.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsFirstStepOrModeIsNotForSupplier_UpdatingNotFirstStepToSupplierStep_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsFirstStepOrModeIsNotForSupplier(_journey1.Id, _step1InJourney1ForSupplier.ModeId, _step3InJourney1.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsForSupplier_KnownForSupplier_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsForSupplierAsync(_step1InJourney1ForSupplier.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsForSupplier_KnownNotForSupplier_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsForSupplierAsync(_step3InJourney1.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsForSupplier_UnknownId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsForSupplierAsync(126234, default);
                Assert.IsFalse(result);
            }
        }
    }
}

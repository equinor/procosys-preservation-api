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
        private Step _stepInJourney1;
        private Journey _journey2;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var mode = AddMode(context, "M");
                var responsible = AddResponsible(context, "R");

                _journey1 = AddJourneyWithStep(context, "J1", "Step1", mode, responsible);
                _journey1.AddStep(new Step(TestPlant, "Step3", AddMode(context, "M2"), AddResponsible(context, "R2")));
                _stepInJourney1 = _journey1.Steps.Single();

                // bytte til navn Step1=lov
                // bytte til navn Step2=lov (fordi i annet journey)
                // bytte til navn Step3=ulovlig

                _journey2 = AddJourneyWithStep(context, "J2", "Step2", mode, responsible);

                context.SaveChangesAsync().Wait();
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(_stepInJourney1.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownTitleInJourney_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(_journey1.Id, _stepInJourney1.Title, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_NewTitle_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(_journey1.Id, "AnotherStep", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownTitleInAnotherJourney_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(_journey2.Id, _stepInJourney1.Title, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownVoided_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var step = context.Steps.Single(s => s.Id == _stepInJourney1.Id);
                step.Void();
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsVoidedAsync(_stepInJourney1.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownNotVoided_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsVoidedAsync(_stepInJourney1.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        //[TestMethod]
        //public async Task ExistsInJourneyAsync_SameTitleAsBefore_ReturnsTrue()
        //{
        //    using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
        //    {
        //        var dut = new StepValidator(context);
        //        var result = await dut.ExistsInJourneyAsync();
        //        Assert.IsTrue(result);
        //    }
        //}

        //[TestMethod]
        //public async Task ExistsInJourneyAsync_SameTitleAsAnotherStepInSameJourney_ReturnsFalse()
        //{
        //    using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
        //    {
        //        var dut = new StepValidator(context);
        //        var result = await dut.ExistsInJourneyAsync();
        //        Assert.IsTrue(result);
        //    }
        //}

        //[TestMethod]
        //public async Task ExistsInJourneyAsync_SameTitleAsAnotherStepInAnotherJourney_ReturnsTrue()
        //{
        //    using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
        //    {
        //        var dut = new StepValidator(context);
        //        var result = await dut.ExistsInJourneyAsync();
        //        Assert.IsTrue(result);
        //    }
        //}
    }
}

using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.StepValidators;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class StepValidatorTests : ReadOnlyTestsBase
    {
        private int StepId;
                        
        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _eventDispatcher, _plantProvider))
            {
                StepId = AddJourneyWithStep(context, "J", AddMode(context, "M"), AddResponsible(context, "R")).Steps.First().Id;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(StepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownVoided_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var step = context.Step.Single(s => s.Id == StepId);
                step.Void();
                context.SaveChanges();
            }

            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsVoidedAsync(StepId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownNotVoided_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsVoidedAsync(StepId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new StepValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }
    }
}

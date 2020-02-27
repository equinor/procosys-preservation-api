using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ModeValidators;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class ModeValidatorTests : ReadOnlyTestsBase
    {
        private const string ModeTitle = "TestMode";
        private int ModeId;
                
        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var mode = AddMode(context, ModeTitle);
                var responsible = AddResponsible(context, "R");
                AddJourneyWithStep(context, "J", mode, responsible);
                ModeId = mode.Id;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownTitle_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsAsync(ModeTitle, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsAsync(ModeId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownTitle_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsAsync("XXX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownVoided_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var mode = context.Modes.Single(m => m.Id == ModeId);
                mode.Void();
                context.SaveChanges();
            }

            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.IsVoidedAsync(ModeId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownNotVoided_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.IsVoidedAsync(ModeId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsUsedInStepAsync_KnownId_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.IsUsedInStepAsync(ModeId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsUsedInStepAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.IsUsedInStepAsync(126234, default);
                Assert.IsFalse(result);
            }
        }
    }
}

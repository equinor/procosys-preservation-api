using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.ModeValidators;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class ModeValidatorTests : ReadOnlyTestsBase
    {
        private const string ModeTitle = "TestMode";
        private int _modeId;
        private int _supplierModeId;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var mode = AddMode(context, ModeTitle, false);
                var responsible = AddResponsible(context, "R");
                AddJourneyWithStep(context, "J", "S", mode, responsible);
                _modeId = mode.Id;
                _supplierModeId = AddMode(context, "SUPPLIER", true).Id;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownTitle_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsWithSameTitleAsync(ModeTitle, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsAsync(_modeId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownTitle_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsWithSameTitleAsync("XXX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownVoided_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var mode = context.Modes.Single(m => m.Id == _modeId);
                mode.IsVoided = true;
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.IsVoidedAsync(_modeId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownNotVoided_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.IsVoidedAsync(_modeId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_UnknownId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsUsedInStepAsync_KnownId_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.IsUsedInStepAsync(_modeId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsUsedInStepAsync_UnknownId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.IsUsedInStepAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsAnotherModeForSupplierAsync_KnownId_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsAnotherModeForSupplierAsync(_modeId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAnotherModeForSupplierAsync_UnknownId_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsAnotherModeForSupplierAsync(126234, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsModeForSupplierAsync_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsModeForSupplierAsync(default);
                Assert.IsTrue(result);
            }
        }
    }
}

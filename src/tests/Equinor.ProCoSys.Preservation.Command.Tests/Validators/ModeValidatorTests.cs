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
        private int _nonSupplierModeId;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var mode = AddMode(context, ModeTitle, false);
                var responsible = AddResponsible(context, "R");
                AddJourneyWithStep(context, "J", "S", mode, responsible);
                _nonSupplierModeId = mode.Id;
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleAsync_ShouldReturnTrue_WhenKnownTitle()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsWithSameTitleAsync(ModeTitle, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_ShouldReturnTrue_WhenKnownId()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsAsync(_nonSupplierModeId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleAsync_ShouldReturnFalse_WhenUnknownTitle()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsWithSameTitleAsync("XXX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_ShouldReturnFalse_WhenUnknownId()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_ShouldReturnTrue_WhenKnownVoided()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var mode = context.Modes.Single(m => m.Id == _nonSupplierModeId);
                mode.IsVoided = true;
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.IsVoidedAsync(_nonSupplierModeId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_ShouldReturnFalse_WhenKnownNotVoided()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.IsVoidedAsync(_nonSupplierModeId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_ShouldReturnFalse_WhenUnknownId()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsUsedInStepAsync_ShouldReturnTrue_WhenKnownId()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.IsUsedInStepAsync(_nonSupplierModeId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsUsedInStepAsync_ShouldReturnFalse_WhenUnknownId()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.IsUsedInStepAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsAnotherModeForSupplierAsync_ShouldReturnTrue_WhenKnownIdAndModeWithForSupplierExists()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                AddMode(context, "SUPPLIER", true);
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsAnotherModeForSupplierAsync(_nonSupplierModeId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAnotherModeForSupplierAsync_ShouldReturnTrue_WhenUnknownIdAndModeWithForSupplierExists()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                AddMode(context, "SUPPLIER", true);
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsAnotherModeForSupplierAsync(126234, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAnotherModeForSupplierAsync_ShouldReturnFalse_WhenKnownIdAndNoModeWithForSupplierExists()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsAnotherModeForSupplierAsync(_nonSupplierModeId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsAnotherModeForSupplierAsync_ShouldReturnFalse_WhenUnknownIdAndNoModeWithForSupplierExists()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsAnotherModeForSupplierAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsAnyModeWithForSupplierAsync_ShouldReturnTrue_WhenModeWithForSupplierExists()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                AddMode(context, "SUPPLIER", true);
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsAnyModeWithForSupplierAsync(default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAnyModeWithForSupplierAsync_ShouldReturnFalse_WhenNoModeWithForSupplierExists()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsAnyModeWithForSupplierAsync(default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithForSupplierValueAsync_ShouldReturnTrue_WhenCheckingNonSupplierModeForFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsWithForSupplierValueAsync(_nonSupplierModeId, false, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithForSupplierValueAsync_ShouldReturnFalse_WhenCheckingNonSupplierModeForTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsWithForSupplierValueAsync(_nonSupplierModeId, true, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithForSupplierValueAsync_ShouldReturnTrue_WhenCheckingSupplierModeForTrue()
        {
            int supplierModeId;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                supplierModeId = AddMode(context, "SUPPLIER", true).Id;
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsWithForSupplierValueAsync(supplierModeId, true, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithForSupplierValueAsync_ShouldReturnFalse_WhenCheckingSupplierModeForFalse()
        {
            int supplierModeId;
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                supplierModeId = AddMode(context, "SUPPLIER", true).Id;
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new ModeValidator(context);
                var result = await dut.ExistsWithForSupplierValueAsync(supplierModeId, false, default);
                Assert.IsFalse(result);
            }
        }
    }
}

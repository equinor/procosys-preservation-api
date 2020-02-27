using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.ResponsibleValidators;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class ResponsibleValidatorTests : ReadOnlyTestsBase
    {
        private int ResponsibleId;
                        
        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _eventDispatcher, _plantProvider))
            {
                ResponsibleId = AddResponsible(context, "R").Id;
            }
        }

        [TestMethod]
        public async Task ValidateExists_KnownId_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new ResponsibleValidator(context);
                var result = await dut.ExistsAsync(ResponsibleId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TaskValidateExists_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new ResponsibleValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ValidateIsVoided_KnownVoided_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var responsible = context.Responsibles.Single(r => r.Id == ResponsibleId);
                responsible.Void();
                context.SaveChanges();
            }
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new ResponsibleValidator(context);
                var result = await dut.IsVoidedAsync(ResponsibleId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ValidateIsVoided_KnownNotVoided_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new ResponsibleValidator(context);
                var result = await dut.IsVoidedAsync(ResponsibleId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ValidateIsVoided_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new ResponsibleValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }
    }
}

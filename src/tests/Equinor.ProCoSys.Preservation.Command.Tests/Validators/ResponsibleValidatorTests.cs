using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.ResponsibleValidators;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class ResponsibleValidatorTests : ReadOnlyTestsBase
    {
        private string _responsibleCode;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                _responsibleCode = AddResponsible(context, "R").Code;
            }
        }

        [TestMethod]
        public async Task ExistsAndIsVoidedAsync_KnownCode_Voided_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var responsible = context.Responsibles.Single(r => r.Code == _responsibleCode);
                responsible.IsVoided = true;
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var dut = new ResponsibleValidator(context);
                var result = await dut.ExistsAndIsVoidedAsync(_responsibleCode, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAndIsVoidedAsync_KnownCode_NotVoided_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var dut = new ResponsibleValidator(context);
                var result = await dut.ExistsAndIsVoidedAsync(_responsibleCode, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsAndIsVoidedAsync_UnknownCode_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher,
                _currentUserProvider))
            {
                var dut = new ResponsibleValidator(context);
                var result = await dut.ExistsAndIsVoidedAsync("A", default);
                Assert.IsFalse(result);
            }
        }
    }
}

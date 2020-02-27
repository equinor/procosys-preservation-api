using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class RequirementDefinitionValidatorTests : ReadOnlyTestsBase
    {
        private int ReqDefId;
                        
        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _eventDispatcher, _plantProvider))
            {
                ReqDefId = AddRequirementTypeWith1DefWithoutField(context, "R", "D").RequirementDefinitions.First().Id;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.ExistsAsync(ReqDefId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownVoided_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var reqDef = context.RequirementDefinitions.Single(rd => rd.Id == ReqDefId);
                reqDef.Void();
                context.SaveChanges();
            }
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.IsVoidedAsync(ReqDefId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownNotVoided_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.IsVoidedAsync(ReqDefId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _eventDispatcher, _plantProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }
    }
}

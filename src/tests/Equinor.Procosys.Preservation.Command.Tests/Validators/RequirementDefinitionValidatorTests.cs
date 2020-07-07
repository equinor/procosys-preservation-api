using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class RequirementDefinitionValidatorTests : ReadOnlyTestsBase
    {
        private int _reqDefForAllId;
        private RequirementType _requirementType;
        private int _reqDefForSupplierId;
        private int _reqDefForOtherId;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _requirementType = AddRequirementTypeWith1DefWithoutField(context, "R", "D1", "Other");
                _reqDefForAllId = _requirementType.RequirementDefinitions.First().Id;
                var reqDefForSupplier = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForSuppliersOnly, 1);
                _requirementType.AddRequirementDefinition(reqDefForSupplier);
                var reqDefForOther = new RequirementDefinition(TestPlant, "D3", 2, RequirementUsage.ForOtherThanSuppliers, 1);
                _requirementType.AddRequirementDefinition(reqDefForOther);
                context.SaveChangesAsync().Wait();

                _reqDefForSupplierId = reqDefForSupplier.Id;
                _reqDefForOtherId = reqDefForOther.Id;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.ExistsAsync(_reqDefForAllId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownVoided_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqDef = context.RequirementDefinitions.Single(rd => rd.Id == _reqDefForAllId);
                reqDef.Void();
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.IsVoidedAsync(_reqDefForAllId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownNotVoided_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.IsVoidedAsync(_reqDefForAllId, default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task IsVoidedAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasAnyForSupplierOnlyUsageAsync_UsageForAllRequirement_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int> {_reqDefForAllId};
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.HasAnyForSupplierOnlyUsageAsync(reqIds, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasAnyForSupplierOnlyUsageAsync_UsageForOtherRequirement_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int> {_reqDefForOtherId};
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.HasAnyForSupplierOnlyUsageAsync(reqIds, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasAnyForSupplierOnlyUsageAsync_UsageForSupplierRequirement_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int> {_reqDefForSupplierId};
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.HasAnyForSupplierOnlyUsageAsync(reqIds, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HasAnyForSupplierOnlyUsageAsync_UsageForSupplierAndOtherAndForAllRequirement_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int> {_reqDefForSupplierId, _reqDefForOtherId, _reqDefForAllId};
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.HasAnyForSupplierOnlyUsageAsync(reqIds, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task HasAnyForSupplierOnlyUsageAsync_UnknownRequirement_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int> {0};
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.HasAnyForSupplierOnlyUsageAsync(reqIds, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasAnyForSupplierOnlyUsageAsync_NoRequirements_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int>();
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.HasAnyForSupplierOnlyUsageAsync(reqIds, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task UsageCoversForOtherThanSuppliersAsync_UsageForAllRequirement_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int> {_reqDefForAllId};
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.UsageCoversForOtherThanSuppliersAsync(reqIds, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task UsageCoversForOtherThanSuppliersAsync_UsageForOtherRequirement_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int> {_reqDefForOtherId};
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.UsageCoversForOtherThanSuppliersAsync(reqIds, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task UsageCoversForOtherThanSuppliersAsync_UsageForSupplierRequirement_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int> {_reqDefForSupplierId};
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.UsageCoversForOtherThanSuppliersAsync(reqIds, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task UsageCoversForOtherThanSuppliersAsync_UsageForSupplierAndOtherAndForAllRequirement_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int> {_reqDefForSupplierId, _reqDefForOtherId, _reqDefForAllId};
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.UsageCoversForOtherThanSuppliersAsync(reqIds, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task UsageCoversForOtherThanSuppliersAsync_UnknownRequirement_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int> {0};
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.UsageCoversForOtherThanSuppliersAsync(reqIds, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task UsageCoversForOtherThanSuppliersAsync_NoRequirements_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int>();
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.UsageCoversForOtherThanSuppliersAsync(reqIds, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task UsageCoversBothForSupplierAndOtherAsync_UsageForAllRequirement_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int> {_reqDefForAllId};
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.UsageCoversBothForSupplierAndOtherAsync(reqIds, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task UsageCoversBothForSupplierAndOtherAsync_UsageForOtherRequirement_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int> {_reqDefForOtherId};
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.UsageCoversBothForSupplierAndOtherAsync(reqIds, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task UsageCoversBothForSupplierAndOtherAsync_UsageForSupplierRequirement_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int> {_reqDefForSupplierId};
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.UsageCoversBothForSupplierAndOtherAsync(reqIds, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task UsageCoversBothForSupplierAndOtherAsync_UsageForSupplierAndOtherAndForAllRequirement_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int> {_reqDefForSupplierId, _reqDefForOtherId, _reqDefForAllId};
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.UsageCoversBothForSupplierAndOtherAsync(reqIds, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task UsageCoversBothForSupplierAndOtherAsync_UsageForSupplierAndOtherRequirement_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int> {_reqDefForSupplierId, _reqDefForOtherId};
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.UsageCoversBothForSupplierAndOtherAsync(reqIds, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task UsageCoversBothForSupplierAndOtherAsync_UnknownRequirement_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int> {0};
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.UsageCoversBothForSupplierAndOtherAsync(reqIds, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task UsageCoversBothForSupplierAndOtherAsync_NoRequirements_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqIds = new List<int>();
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.UsageCoversBothForSupplierAndOtherAsync(reqIds, default);
                Assert.IsFalse(result);
            }
        }
    }
}

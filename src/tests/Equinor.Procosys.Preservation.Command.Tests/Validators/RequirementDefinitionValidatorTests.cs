using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.RequirementDefinitionValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class RequirementDefinitionValidatorTests : ReadOnlyTestsBase
    {
        private RequirementType _requirementType;
        private int _numberFieldId;
        private int _cbFieldId;
        private RequirementDefinition _reqDefForSupplier;
        private RequirementDefinition _reqDefForOther;
        private RequirementDefinition _reqDefWithoutField;
        private RequirementDefinition _reqDefForAll;
        private RequirementDefinition _reqDefWithField;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _requirementType = AddRequirementTypeWith1DefWithoutField(context, "R", "D1", RequirementTypeIcon.Other);
                _reqDefForAll = _reqDefWithoutField = _requirementType.RequirementDefinitions.First();
                _reqDefForSupplier = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForSuppliersOnly, 1);
                _requirementType.AddRequirementDefinition(_reqDefForSupplier);
                _reqDefForOther = _reqDefWithField = new RequirementDefinition(TestPlant, "D3", 2, RequirementUsage.ForOtherThanSuppliers, 1);
                _requirementType.AddRequirementDefinition(_reqDefForOther);
                _numberFieldId = AddNumberField(context, _reqDefForOther, "N", "mm", true).Id;
                _cbFieldId = AddCheckBoxField(context, _reqDefForOther, "CB").Id;

                context.SaveChangesAsync().Wait();
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.ExistsAsync(_reqDefForAll.Id, default);
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
                var reqDef = context.RequirementDefinitions.Single(rd => rd.Id == _reqDefForAll.Id);
                reqDef.IsVoided = true;
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.IsVoidedAsync(_reqDefForAll.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownNotVoided_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.IsVoidedAsync(_reqDefForAll.Id, default);
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
                var reqIds = new List<int> {_reqDefForAll.Id};
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
                var reqIds = new List<int> {_reqDefForOther.Id};
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
                var reqIds = new List<int> {_reqDefForSupplier.Id};
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
                var reqIds = new List<int> {_reqDefForSupplier.Id, _reqDefForOther.Id, _reqDefForAll.Id};
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
                var reqIds = new List<int> {_reqDefForAll.Id};
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
                var reqIds = new List<int> {_reqDefForOther.Id};
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
                var reqIds = new List<int> {_reqDefForSupplier.Id};
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
                var reqIds = new List<int> {_reqDefForSupplier.Id, _reqDefForOther.Id, _reqDefForAll.Id};
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
                var reqIds = new List<int> {_reqDefForAll.Id};
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
                var reqIds = new List<int> {_reqDefForOther.Id};
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
                var reqIds = new List<int> {_reqDefForSupplier.Id};
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
                var reqIds = new List<int> {_reqDefForSupplier.Id, _reqDefForOther.Id, _reqDefForAll.Id};
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
                var reqIds = new List<int> {_reqDefForSupplier.Id, _reqDefForOther.Id};
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

        [TestMethod]
        public async Task HasAnyFieldsAsync_NoFields_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.HasAnyFieldsAsync(_reqDefWithoutField.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasAnyFieldsAsync_FieldsExist_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.HasAnyFieldsAsync(_reqDefWithField.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TagRequirementsExistAsync_NoTagRequirements_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.TagRequirementsExistAsync(_reqDefWithoutField.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task TagRequirementsExistAsync_TagRequirementsExists_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var stepMock = new Mock<Step>();
                stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
                var project = AddProject(context, "P", "D");
                var tag = new Tag(TestPlant, TagType.Standard, "TagNo", "Desc", stepMock.Object, new List<TagRequirement>
                {
                    new TagRequirement(TestPlant, 4, _reqDefWithoutField)
                });
                project.AddTag(tag);
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.TagRequirementsExistAsync(_reqDefWithoutField.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TagFunctionRequirementsExistAsync_NoTagFunctionRequirements_ReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.TagFunctionRequirementsExistAsync(_reqDefWithoutField.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task TagFunctionRequirementsExistAsync_TagFunctionRequirementsExists_ReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var tf = AddTagFunction(context, "M", "R");
                tf.AddRequirement(new TagFunctionRequirement(TestPlant, 4, _reqDefWithoutField));
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.TagFunctionRequirementsExistAsync(_reqDefWithoutField.Id, default);
                Assert.IsTrue(result);
            }
        }
    }
}

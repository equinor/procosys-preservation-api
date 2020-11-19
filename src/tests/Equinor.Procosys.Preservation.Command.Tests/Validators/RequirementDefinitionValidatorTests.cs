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
        private RequirementDefinition _reqDefWithTwoFields;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                _requirementType = AddRequirementTypeWith1DefWithoutField(context, "R", "D1", RequirementTypeIcon.Other);
                _reqDefForAll = _reqDefWithoutField = _requirementType.RequirementDefinitions.First();
                _reqDefForSupplier = new RequirementDefinition(TestPlant, "D2", 2, RequirementUsage.ForSuppliersOnly, 1);
                _requirementType.AddRequirementDefinition(_reqDefForSupplier);
                _reqDefForOther = _reqDefWithTwoFields = new RequirementDefinition(TestPlant, "D3", 2, RequirementUsage.ForOtherThanSuppliers, 1);
                _requirementType.AddRequirementDefinition(_reqDefForOther);
                _numberFieldId = AddNumberField(context, _reqDefForOther, "N", "mm", true).Id;
                _cbFieldId = AddCheckBoxField(context, _reqDefForOther, "CB").Id;

                context.SaveChangesAsync().Wait();
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.ExistsAsync(_reqDefForAll.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsFieldAsync_KnownId_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.ExistsFieldAsync(_reqDefWithTwoFields.Id, _numberFieldId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsFieldAsync_UnknownReqDefId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.ExistsFieldAsync(126234, _numberFieldId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsFieldAsync_UnknownFieldId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.ExistsFieldAsync(_reqDefWithTwoFields.Id, 126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownVoided_ShouldReturnTrue()
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
        public async Task IsVoidedAsync_KnownNotVoided_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.IsVoidedAsync(_reqDefForAll.Id, default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task IsVoidedAsync_UnknownId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasAnyForSupplierOnlyUsageAsync_UsageForAllRequirement_ShouldReturnFalse()
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
        public async Task HasAnyForSupplierOnlyUsageAsync_UsageForOtherRequirement_ShouldReturnFalse()
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
        public async Task HasAnyForSupplierOnlyUsageAsync_UsageForSupplierRequirement_ShouldReturnTrue()
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
        public async Task HasAnyForSupplierOnlyUsageAsync_UsageForSupplierAndOtherAndForAllRequirement_ShouldReturnTrue()
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
        public async Task HasAnyForSupplierOnlyUsageAsync_UnknownRequirement_ShouldReturnFalse()
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
        public async Task HasAnyForSupplierOnlyUsageAsync_NoRequirements_ShouldReturnFalse()
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
        public async Task UsageCoversForOtherThanSuppliersAsync_UsageForAllRequirement_ShouldReturnTrue()
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
        public async Task UsageCoversForOtherThanSuppliersAsync_UsageForOtherRequirement_ShouldReturnTrue()
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
        public async Task UsageCoversForOtherThanSuppliersAsync_UsageForSupplierRequirement_ShouldReturnFalse()
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
        public async Task UsageCoversForOtherThanSuppliersAsync_UsageForSupplierAndOtherAndForAllRequirement_ShouldReturnTrue()
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
        public async Task UsageCoversForOtherThanSuppliersAsync_UnknownRequirement_ShouldReturnFalse()
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
        public async Task UsageCoversForOtherThanSuppliersAsync_NoRequirements_ShouldReturnFalse()
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
        public async Task UsageCoversBothForSupplierAndOtherAsync_UsageForAllRequirement_ShouldReturnTrue()
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
        public async Task UsageCoversBothForSupplierAndOtherAsync_UsageForOtherRequirement_ShouldReturnFalse()
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
        public async Task UsageCoversBothForSupplierAndOtherAsync_UsageForSupplierRequirement_ShouldReturnFalse()
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
        public async Task UsageCoversBothForSupplierAndOtherAsync_UsageForSupplierAndOtherAndForAllRequirement_ShouldReturnTrue()
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
        public async Task UsageCoversBothForSupplierAndOtherAsync_UsageForSupplierAndOtherRequirement_ShouldReturnTrue()
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
        public async Task UsageCoversBothForSupplierAndOtherAsync_UnknownRequirement_ShouldReturnFalse()
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
        public async Task UsageCoversBothForSupplierAndOtherAsync_NoRequirements_ShouldReturnFalse()
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
        public async Task HasAnyFieldsAsync_NoFields_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.HasAnyFieldsAsync(_reqDefWithoutField.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task HasAnyFieldsAsync_FieldsExist_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.HasAnyFieldsAsync(_reqDefWithTwoFields.Id, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TagRequirementsExistAsync_NoTagRequirements_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.TagRequirementsExistAsync(_reqDefWithoutField.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task TagRequirementsExistAsync_TagRequirementsExists_ShouldReturnTrue()
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
        public async Task TagFunctionRequirementsExistAsync_NoTagFunctionRequirements_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.TagFunctionRequirementsExistAsync(_reqDefWithoutField.Id, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task TagFunctionRequirementsExistAsync_TagFunctionRequirementsExists_ShouldReturnTrue()
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

        [TestMethod]
        public async Task AllExcludedFieldsAreVoidedAsync_NoExcludedFieldsInUpdateList_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.AllExcludedFieldsAreVoidedAsync(_reqDefWithTwoFields.Id, new List<int>{_cbFieldId, _numberFieldId},  default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task AllExcludedFieldsAreVoidedAsync_ReqDefHasNoFields_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.AllExcludedFieldsAreVoidedAsync(_reqDefWithoutField.Id, new List<int>(),  default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task AllExcludedFieldsAreVoidedAsync_TheExcludedFieldIsNotVoided_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.AllExcludedFieldsAreVoidedAsync(_reqDefWithTwoFields.Id, new List<int>{_numberFieldId},  default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AllExcludedFieldsAreVoidedAsync_TheExcludedFieldIsVoided_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var cbField = context.Fields.Single(f => f.Id == _cbFieldId);
                cbField.IsVoided = true;
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.AllExcludedFieldsAreVoidedAsync(_reqDefWithTwoFields.Id, new List<int>{_numberFieldId},  default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task AllExcludedFieldsAreVoidedAsync_AllExcludedFieldAreVoided_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var cbField = context.Fields.Single(f => f.Id == _cbFieldId);
                cbField.IsVoided = true;
                var numberField = context.Fields.Single(f => f.Id == _numberFieldId);
                numberField.IsVoided = true;
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.AllExcludedFieldsAreVoidedAsync(_reqDefWithTwoFields.Id, new List<int>(),  default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task AnyExcludedFieldsIsInUseAsync_TheExcludedFieldIsNotInUse_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.AnyExcludedFieldsIsInUseAsync(_reqDefWithTwoFields.Id, new List<int>{_numberFieldId},  default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AnyExcludedFieldsIsInUseAsync_TheExcludedFieldHasRecordedPreservation_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var stepMock = new Mock<Step>();
                stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
                var project = AddProject(context, "P", "D");
                var tagRequirement = new TagRequirement(TestPlant, 4, _reqDefWithTwoFields);
                var tag = new Tag(TestPlant, TagType.Standard, "TagNo", "Desc", stepMock.Object, new List<TagRequirement>
                {
                    tagRequirement
                });
                tag.StartPreservation();
                project.AddTag(tag);
                context.SaveChangesAsync().Wait();

                var requirementDefinition = context.RequirementDefinitions.Include(rd => rd.Fields)
                    .Single(rd => rd.Id == _reqDefWithTwoFields.Id);
                tagRequirement.RecordCheckBoxValues(
                    new Dictionary<int, bool> {{_cbFieldId, true}},
                    requirementDefinition);
                context.SaveChangesAsync().Wait();
            }

            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementDefinitionValidator(context);
                var result = await dut.AnyExcludedFieldsIsInUseAsync(_reqDefWithTwoFields.Id, new List<int>{_numberFieldId},  default);
                Assert.IsTrue(result);
            }
        }
    }
}

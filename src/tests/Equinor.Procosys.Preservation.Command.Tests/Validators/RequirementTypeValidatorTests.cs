using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.RequirementTypeValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class RequirementTypeValidatorTests : ReadOnlyTestsBase
    {
        private readonly string _reqTypeCode1 = "Code1";
        private readonly string _reqDefTitle1_1 = "DefTitle1_1";
        private readonly string _reqDefTitle1_2 = "DefTitle1_2";
        private readonly string _reqTypeCode2 = "Code2";
        private readonly string _reqDefTitle2 = "DefTitle2";
        private int _reqTypeId1;
        private int _reqDefId1_2;
        private string _reqTypeTitle1;
        private string _reqTypeTitle2;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var requirementType = AddRequirementTypeWith1DefWithoutField(context, _reqTypeCode1, _reqDefTitle1_1, RequirementTypeIcon.Other);
                _reqTypeId1 = requirementType.Id;
                _reqTypeTitle1 = requirementType.Title;
                
                var requirementDefinition = new RequirementDefinition(TestPlant, _reqDefTitle1_2, 2, RequirementUsage.ForAll, 1);
                requirementType.AddRequirementDefinition(requirementDefinition);
                context.SaveChangesAsync().Wait();
                _reqDefId1_2 = requirementDefinition.Id;

                _reqTypeTitle2 = AddRequirementTypeWith1DefWithoutField(context, _reqTypeCode2, _reqDefTitle2, RequirementTypeIcon.Other).Title;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.ExistsAsync(_reqTypeId1, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownVoided_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqType = context.RequirementTypes.Single(rd => rd.Id == _reqTypeId1);
                reqType.IsVoided = true;
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.IsVoidedAsync(_reqTypeId1, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownNotVoided_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.IsVoidedAsync(_reqTypeId1, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_UnknownId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleAsync_KnownTitle_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.ExistsWithSameTitleAsync(_reqTypeTitle1, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleAsync_KnownTitle_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.ExistsWithSameTitleAsync("XXXXXX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleInAnotherTypeAsync_SameTitleAsAnotherType_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.ExistsWithSameTitleInAnotherTypeAsync(_reqTypeId1, _reqTypeTitle2, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameTitleInAnotherTypeAsync_NewTitle_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.ExistsWithSameTitleInAnotherTypeAsync(_reqTypeId1, "XXXXXSD", default);
                Assert.IsFalse(result);
            }
        }
        [TestMethod]
        public async Task ExistsWithSameCodeAsync_KnownCode_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.ExistsWithSameCodeAsync(_reqTypeCode1, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameCodeAsync_KnownCode_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.ExistsWithSameCodeAsync("XXXXXX", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameCodeInAnotherTypeAsync_SameCodeAsAnotherType_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.ExistsWithSameCodeInAnotherTypeAsync(_reqTypeId1, _reqTypeCode2, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsWithSameCodeInAnotherTypeAsync_NewCode_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.ExistsWithSameCodeInAnotherTypeAsync(_reqTypeId1, "XXXXXSD", default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AnyRequirementDefinitionExistsWithSameTitleAsync_WhenSameTitle_AndNeedUserInputAreEqual_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var fieldTypes = new List<FieldType> { FieldType.Info };
                var dut = new RequirementTypeValidator(context);
                var result = await dut.AnyRequirementDefinitionExistsWithSameTitleAsync(_reqTypeId1, _reqDefTitle1_1, fieldTypes, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task AnyRequirementDefinitionExistsWithSameTitleAsync_WhenSameTitle_ButNeedUserInputDiffer_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var fieldTypes = new List<FieldType>{ FieldType.Attachment };
                var dut = new RequirementTypeValidator(context);
                var result = await dut.AnyRequirementDefinitionExistsWithSameTitleAsync(_reqTypeId1, _reqDefTitle1_1, fieldTypes, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task AnyRequirementDefinitionExistsWithSameTitleAsync_WhenNewTitle_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var fieldTypes = new List<FieldType>();
                var dut = new RequirementTypeValidator(context);
                var result = await dut.AnyRequirementDefinitionExistsWithSameTitleAsync(_reqTypeId1, "XXXXXY", fieldTypes, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task OtherRequirementDefinitionExistsWithSameTitleAsync_WhenSameTitleInOtherDefinition_AndNeedUserInputAreEqual_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var fieldTypes = new List<FieldType> { FieldType.Info };
                var dut = new RequirementTypeValidator(context);
                var result = await dut.OtherRequirementDefinitionExistsWithSameTitleAsync(_reqTypeId1, _reqDefId1_2, _reqDefTitle1_1, fieldTypes, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task OtherRequirementDefinitionExistsWithSameTitleAsync_WhenSameTitleInOtherDefinition_ButNeedUserInputDiffer_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var fieldTypes = new List<FieldType>{ FieldType.Attachment };
                var dut = new RequirementTypeValidator(context);
                var result = await dut.OtherRequirementDefinitionExistsWithSameTitleAsync(_reqTypeId1, _reqDefId1_2, _reqDefTitle1_1, fieldTypes, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task OtherRequirementDefinitionExistsWithSameTitleAsync_WhenNewTitle_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var fieldTypes = new List<FieldType>();
                var dut = new RequirementTypeValidator(context);
                var result = await dut.OtherRequirementDefinitionExistsWithSameTitleAsync(_reqTypeId1, _reqDefId1_2, "XXXXXY", fieldTypes, default);
                Assert.IsFalse(result);
            }
        }
    }
}

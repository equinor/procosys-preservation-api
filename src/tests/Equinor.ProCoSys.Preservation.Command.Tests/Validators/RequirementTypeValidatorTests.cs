using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.Validators.RequirementTypeValidators;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Infrastructure;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class RequirementTypeValidatorTests : ReadOnlyTestsBase
    {
        private readonly string _reqTypeCode1 = "Code1";
        private readonly string _reqDefTitle11 = "DefTitle1_1";
        private readonly string _reqDefTitle12 = "DefTitle1_2";
        private readonly string _reqTypeCode2 = "Code2";
        private readonly string _reqDefTitle2 = "DefTitle2";
        private int _reqTypeId1;
        private int _reqTypeId2;
        private int _reqDefId11;
        private int _reqDefId12;
        private string _reqTypeTitle1;
        private string _reqTypeTitle2;
        private int _infoFieldId11;
        private int _infoFieldId12;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var reqType1 = AddRequirementTypeWith1DefWithoutField(context, _reqTypeCode1, _reqDefTitle11, RequirementTypeIcon.Other);
                _reqTypeId1 = reqType1.Id;
                _reqTypeTitle1 = reqType1.Title;
                var reqDef11 = reqType1.RequirementDefinitions.Single();
                _reqDefId11 = reqDef11.Id;
                _infoFieldId11 = AddInfoField(context, reqDef11, "I").Id;

                var reqDef12 = new RequirementDefinition(TestPlant, _reqDefTitle12, 2, RequirementUsage.ForAll, 1);
                reqType1.AddRequirementDefinition(reqDef12);
                context.SaveChangesAsync().Wait();
                _reqDefId12 = reqDef12.Id;
                _infoFieldId12 = AddInfoField(context, reqDef12, "I").Id;

                var reqType2 = AddRequirementTypeWith1DefWithoutField(context, _reqTypeCode2, _reqDefTitle2, RequirementTypeIcon.Other);
                _reqTypeId2 = reqType2.Id;
                _reqTypeTitle2 = reqType2.Title;
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
        public async Task RequirementDefinitionExistsAsync_KnownIds_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.RequirementDefinitionExistsAsync(_reqTypeId1, _reqDefId11, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task RequirementDefinitionExistsAsync_UnknownIds_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.RequirementDefinitionExistsAsync(_reqTypeId2, _reqDefId11, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task FieldExistsAsync_KnownIds_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.FieldExistsAsync(_reqTypeId1, _reqDefId11, _infoFieldId11, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task FieldExistsAsync_UnknownReqTypeId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.FieldExistsAsync(_reqTypeId2, _reqDefId11, _infoFieldId11, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task FieldExistsAsync_UnknownReqDefId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.FieldExistsAsync(_reqTypeId1, _reqDefId12, _infoFieldId11, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task FieldExistsAsync_UnknownFieldId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new RequirementTypeValidator(context);
                var result = await dut.FieldExistsAsync(_reqTypeId1, _reqDefId11, _infoFieldId12, default);
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
                var result = await dut.AnyRequirementDefinitionExistsWithSameTitleAsync(_reqTypeId1, _reqDefTitle11, fieldTypes, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task AnyRequirementDefinitionExistsWithSameTitleAsync_WhenSameTitle_ButNeedUserInputDiffer_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var fieldTypes = new List<FieldType> { FieldType.Attachment };
                var dut = new RequirementTypeValidator(context);
                var result = await dut.AnyRequirementDefinitionExistsWithSameTitleAsync(_reqTypeId1, _reqDefTitle11, fieldTypes, default);
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
                var result = await dut.OtherRequirementDefinitionExistsWithSameTitleAsync(_reqTypeId1, _reqDefId12, _reqDefTitle11, fieldTypes, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task OtherRequirementDefinitionExistsWithSameTitleAsync_WhenSameTitleInOtherDefinition_ButNeedUserInputDiffer_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var fieldTypes = new List<FieldType> { FieldType.Attachment };
                var dut = new RequirementTypeValidator(context);
                var result = await dut.OtherRequirementDefinitionExistsWithSameTitleAsync(_reqTypeId1, _reqDefId12, _reqDefTitle11, fieldTypes, default);
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
                var result = await dut.OtherRequirementDefinitionExistsWithSameTitleAsync(_reqTypeId1, _reqDefId12, "XXXXXY", fieldTypes, default);
                Assert.IsFalse(result);
            }
        }
    }
}

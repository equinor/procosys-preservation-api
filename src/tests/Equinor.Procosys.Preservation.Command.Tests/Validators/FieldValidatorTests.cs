using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.FieldValidators;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Infrastructure;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.Validators
{
    [TestClass]
    public class FieldValidatorTests : ReadOnlyTestsBase
    {
        private int _infoFieldId;
        private int _attachmentFieldId;
        private int _checkBoxFieldId;
        private int _numberFieldId;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var rd = AddRequirementTypeWith1DefWithoutField(context, "T", "D", RequirementTypeIcon.Other).RequirementDefinitions.Single();

                _infoFieldId = AddInfoField(context, rd, "I").Id;
                _numberFieldId = AddNumberField(context, rd, "N", "mm", true).Id;
                _attachmentFieldId = AddAttachmentField(context, rd, "A").Id;
                _checkBoxFieldId = AddCheckBoxField(context, rd, "C").Id;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.ExistsAsync(_infoFieldId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownVoided_ShouldReturnTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var field = context.Fields.Single(f => f.Id == _infoFieldId);
                field.IsVoided = true;
                context.SaveChangesAsync().Wait();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsVoidedAsync(_infoFieldId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownNotVoided_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsVoidedAsync(_infoFieldId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_UnknownId_ShouldReturnFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsValidForRecordingAsync_ForInfoField_ShouldReturnFalse()
        { 
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidForRecordingAsync(_infoFieldId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsValidForRecordingAsync_ForCheckBoxField_ShouldReturnTrue()
        { 
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidForRecordingAsync(_checkBoxFieldId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsValidForRecordingAsync_ForNumberField_ShouldReturnTrue()
        { 
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidForRecordingAsync(_numberFieldId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsValidForAttachmentAsync_ForAttachmentField_ShouldReturnTrue()
        { 
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidForAttachmentAsync(_attachmentFieldId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsValidForAttachmentAsync_ForInfoField_ShouldReturnFalse()
        { 
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidForAttachmentAsync(_infoFieldId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsValidForAttachmentAsync_ForNumberField_ShouldReturnFalse()
        { 
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidForAttachmentAsync(_numberFieldId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task VerifyFieldTypeAsync_ForInfoField_ShouldReturnFalseForNumber()
        { 
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.VerifyFieldTypeAsync(_infoFieldId, FieldType.Number, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task VerifyFieldTypeAsync_ForInfoField_ShouldReturnTrueForInfo()
        { 
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.VerifyFieldTypeAsync(_infoFieldId, FieldType.Info, default);
                Assert.IsTrue(result);
            }
        }
    }
}

using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.Command.Validators.FieldValidators;
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
                var rd = AddRequirementTypeWith1DefWithoutField(context, "T", "D", "Other").RequirementDefinitions.Single();

                _infoFieldId = AddInfoField(context, rd, "I").Id;
                _numberFieldId = AddNumberField(context, rd, "N", "mm", true).Id;
                _attachmentFieldId = AddAttachmentField(context, rd, "A").Id;
                _checkBoxFieldId = AddCheckBoxField(context, rd, "C").Id;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.ExistsAsync(_infoFieldId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownVoided_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var field = context.Fields.Single(f => f.Id == _infoFieldId);
                field.Void();
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
        public async Task IsVoidedAsync_KnownNotVoided_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsVoidedAsync(_infoFieldId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsValidForRecordingAsync_ForInfoField_ReturnsFalse()
        { 
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidForRecordingAsync(_infoFieldId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsValidForRecordingAsync_ForCheckBoxField_ReturnsTrue()
        { 
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidForRecordingAsync(_checkBoxFieldId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsValidForRecordingAsync_ForNumberField_ReturnsTrue()
        { 
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidForRecordingAsync(_numberFieldId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsValidForAttachmentAsync_ForAttachmentField_ReturnsTrue()
        { 
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidForAttachmentAsync(_attachmentFieldId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsValidForAttachmentAsync_ForInfoField_ReturnsFalse()
        { 
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidForAttachmentAsync(_infoFieldId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsValidForAttachmentAsync_ForNumberField_ReturnsFalse()
        { 
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidForAttachmentAsync(_numberFieldId, default);
                Assert.IsFalse(result);
            }
        }
    }
}

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
        private int _checkBoxFieldId;
        private int _numberFieldId;

        protected override void SetupNewDatabase(DbContextOptions<PreservationContext> dbContextOptions)
        {
            using (var context = new PreservationContext(dbContextOptions, _plantProvider))
            {
                AddPerson(context, _currentUserOid, "Ole", "Lukkøye");

                var rd = AddRequirementTypeWith1DefWithoutField(context, "T", "D").RequirementDefinitions.Single();

                _infoFieldId = AddInfoField(context, rd).Id;
                _numberFieldId = NumberField(context, rd, "mm", true).Id;
                _checkBoxFieldId = CheckBoxField(context, rd).Id;
            }
        }

        [TestMethod]
        public async Task ExistsAsync_KnownId_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.ExistsAsync(_infoFieldId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ExistsAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.ExistsAsync(126234, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownVoided_ReturnsTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var field = context.Fields.Single(f => f.Id == _infoFieldId);
                field.Void();
                context.SaveChanges();
            }
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsVoidedAsync(_infoFieldId, default);
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_KnownNotVoided_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsVoidedAsync(_infoFieldId, default);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task IsVoidedAsync_UnknownId_ReturnsFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsVoidedAsync(126234, default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task IsValidValueAsync_ReturnsTrue_OnNumberField_ForNA()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidValueAsync(_numberFieldId, "NA", default);
                Assert.IsTrue(result);
            }
        }
        
        [TestMethod]
        public async Task IsValidValueAsync_ReturnsTrue_OnNumberField_ForNAWithSlash()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidValueAsync(_numberFieldId, "n/a", default);
                Assert.IsTrue(result);
            }
        }
        
        [TestMethod]
        public async Task IsValidValueAsync_OnNumberField_IsNotCaseSensitive()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidValueAsync(_numberFieldId, "nA", default);
                Assert.IsTrue(result);
                result = await dut.IsValidValueAsync(_numberFieldId, "n/A", default);
                Assert.IsTrue(result);
            }
        }
        
        [TestMethod]
        public async Task IsValidValueAsync_ReturnsTrue_OnNumberField_ForNumber()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidValueAsync(_numberFieldId, "12", default);
                Assert.IsTrue(result);
            }
        }
        
        [TestMethod]
        public async Task IsValidValueAsync_ReturnsFalse_OnNumberField_ForText()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidValueAsync(_numberFieldId, "abc", default);
                Assert.IsFalse(result);
            }
        }
        
        [TestMethod]
        public async Task IsValidValueAsync_ReturnsTrue_OnCheckBoxField_ForTrue()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidValueAsync(_checkBoxFieldId, "True", default);
                Assert.IsTrue(result);
            }
        }
        
        [TestMethod]
        public async Task IsValidValueAsync_OnCheckBoxField_IsNotCaseSensitive()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidValueAsync(_checkBoxFieldId, "TruE", default);
                Assert.IsTrue(result);
            }
        }
        
        [TestMethod]
        public async Task IsValidValueAsync_ReturnsTrue_OnCheckBoxField_ForFalse()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidValueAsync(_checkBoxFieldId, "False", default);
                Assert.IsTrue(result);
            }
        }
        
        [TestMethod]
        public async Task IsValidValueAsync_ReturnsFalse_OnCheckBoxField_ForText()
        {
            using (var context = new PreservationContext(_dbContextOptions, _plantProvider))
            {
                var dut = new FieldValidator(context);
                var result = await dut.IsValidValueAsync(_checkBoxFieldId, "abc", default);
                Assert.IsFalse(result);
            }
        }
    }
}

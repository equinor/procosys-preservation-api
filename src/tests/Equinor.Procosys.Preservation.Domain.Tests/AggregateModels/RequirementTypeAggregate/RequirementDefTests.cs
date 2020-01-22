using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.RequirementTypeAggregate
{
    [TestClass]
    public class RequirementDefTests
    {
        private RequirementDefinition _dut;

        [TestInitialize]
        public void Setup() => _dut = new RequirementDefinition("SchemaA", "TitleA", 4, 10);

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual("SchemaA", _dut.Schema);
            Assert.AreEqual("TitleA", _dut.Title);
            Assert.AreEqual(4, _dut.DefaultIntervalWeeks);
            Assert.AreEqual(10, _dut.SortKey);
            Assert.IsFalse(_dut.IsVoided);
            Assert.AreEqual(0, _dut.Fields.Count);
        }

        [TestMethod]
        public void Constructor_ShouldMakeRequirementDefinitionNotNeedingInput()
            => Assert.IsFalse(_dut.NeedsUserInput);

        [TestMethod]
        public void AddField_ShouldThrowExceptionTest_ForNullField()
        {
            var dut = new RequirementDefinition("", "", 0, 0);

            Assert.ThrowsException<ArgumentNullException>(() => dut.AddField(null));
            Assert.AreEqual(0, dut.Fields.Count);
        }

        [TestMethod]
        public void AddField_ShouldAddFieldToFieldsList()
        {
            var f = new Mock<Field>();

            var dut = new RequirementDefinition("", "", 0, 0);
            dut.AddField(f.Object);

            Assert.AreEqual(1, dut.Fields.Count);
            Assert.IsTrue(dut.Fields.Contains(f.Object));
        }

        [TestMethod]
        public void AddInfoField_ShouldNotCauseRequirementDefinitionNeedingInput()
        {
            var f = new Field("", "", FieldType.Info, 1);

            var dut = new RequirementDefinition("", "", 0, 0);
            Assert.IsFalse(_dut.NeedsUserInput);
            
            dut.AddField(f);

            Assert.IsFalse(_dut.NeedsUserInput);
        }

        [TestMethod]
        public void AddNumberField_ShouldCauseRequirementDefinitionNeedingInput()
        {
            var f = new Field("", "", FieldType.Number, 1, "u", false);

            Assert.IsFalse(_dut.NeedsUserInput);
            
            _dut.AddField(f);

            Assert.IsTrue(_dut.NeedsUserInput);
        }

        [TestMethod]
        public void AddCheckBoxField_ShouldCauseRequirementDefinitionNeedingInput()
        {
            var f = new Field("", "", FieldType.CheckBox, 1);

            Assert.IsFalse(_dut.NeedsUserInput);
            
            _dut.AddField(f);

            Assert.IsTrue(_dut.NeedsUserInput);
        }

        [TestMethod]
        public void AddAttachmentField_ShouldCauseRequirementDefinitionNeedingInput()
        {
            var f = new Field("", "", FieldType.Attachment, 1);

            Assert.IsFalse(_dut.NeedsUserInput);
            
            _dut.AddField(f);

            Assert.IsTrue(_dut.NeedsUserInput);
        }

        [TestMethod]
        public void VoidUnVoid_ShouldToggleIsVoided()
        {
            Assert.IsFalse(_dut.IsVoided);

            _dut.Void();
            Assert.IsTrue(_dut.IsVoided);

            _dut.UnVoid();
            Assert.IsFalse(_dut.IsVoided);
        }
    }
}

using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.RequirementTypeAggregate
{
    [TestClass]
    public class RequirementDefinitionTests
    {
        private const string TestPlant = "PlantA";
        private RequirementDefinition _dut;

        [TestInitialize]
        public void Setup() => _dut = new RequirementDefinition(TestPlant, "TitleA", 4, RequirementUsage.ForAll, 10);

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual(TestPlant, _dut.Plant);
            Assert.AreEqual("TitleA", _dut.Title);
            Assert.AreEqual(4, _dut.DefaultIntervalWeeks);
            Assert.AreEqual(10, _dut.SortKey);
            Assert.AreEqual(RequirementUsage.ForAll, _dut.DefaultUsage);
            Assert.IsFalse(_dut.IsVoided);
            Assert.AreEqual(0, _dut.Fields.Count);
        }

        [TestMethod]
        public void Constructor_ShouldMakeRequirementDefinitionNotNeedingInput()
            => Assert.IsFalse(_dut.NeedsUserInput);

        [TestMethod]
        public void AddField_ShouldThrowExceptionTest_ForNullField()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _dut.AddField(null));
            Assert.AreEqual(0, _dut.Fields.Count);
        }

        [TestMethod]
        public void AddField_ShouldAddFieldToFieldsList()
        {
            var f = new Field(TestPlant, "", FieldType.Info, 1);

            _dut.AddField(f);

            Assert.AreEqual(1, _dut.Fields.Count);
            Assert.IsTrue(_dut.Fields.Contains(f));
        }

        [TestMethod]
        public void AddInfoField_ShouldNotCauseRequirementDefinitionNeedingInput()
        {
            var f = new Field(TestPlant, "", FieldType.Info, 1);
            Assert.IsFalse(_dut.NeedsUserInput);
            
            _dut.AddField(f);
            Assert.IsFalse(_dut.NeedsUserInput);
        }

        [TestMethod]
        public void AddNumberField_ShouldCauseRequirementDefinitionNeedingInput()
        {
            var f = new Field(TestPlant, "", FieldType.Number, 1, "u", false);

            Assert.IsFalse(_dut.NeedsUserInput);
            
            _dut.AddField(f);

            Assert.IsTrue(_dut.NeedsUserInput);
        }

        [TestMethod]
        public void AddCheckBoxField_ShouldCauseRequirementDefinitionNeedingInput()
        {
            var f = new Field(TestPlant, "", FieldType.CheckBox, 1);

            Assert.IsFalse(_dut.NeedsUserInput);
            
            _dut.AddField(f);

            Assert.IsTrue(_dut.NeedsUserInput);
        }

        [TestMethod]
        public void AddAttachmentField_ShouldCauseRequirementDefinitionNeedingInput()
        {
            var f = new Field(TestPlant, "", FieldType.Attachment, 1);

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

        [TestMethod]
        public void OrderedFields_ShouldReturnAllFieldsOrdered()
        {
            var f1 = new Field(TestPlant, "First", FieldType.Info, 10);
            var f2 = new Field(TestPlant, "Second", FieldType.Info, 5);

            _dut.AddField(f1);
            _dut.AddField(f2);

            Assert.AreEqual(_dut.Fields.Count, _dut.OrderedFields(false).Count());
            Assert.AreEqual(f2, _dut.OrderedFields(false).ElementAt(0));
            Assert.AreEqual(f1, _dut.OrderedFields(false).ElementAt(1));
        }

        [TestMethod]
        public void OrderedFields_ShouldNotReturnVoidedFields()
        {
            var f1 = new Field(TestPlant, "First", FieldType.Info, 10);
            var f2 = new Field(TestPlant, "Second", FieldType.Info, 5);

            _dut.AddField(f1);
            _dut.AddField(f2);
            f2.Void();

            Assert.AreEqual(1, _dut.OrderedFields(false).Count());
        }

        [TestMethod]
        public void OrderedFields_ShouldReturnVoidedFields()
        {
            var f1 = new Field(TestPlant, "First", FieldType.Info, 10);
            var f2 = new Field(TestPlant, "Second", FieldType.Info, 5);

            _dut.AddField(f1);
            _dut.AddField(f2);
            f2.Void();

            Assert.AreEqual(_dut.Fields.Count, _dut.OrderedFields(true).Count());
        }
    }
}

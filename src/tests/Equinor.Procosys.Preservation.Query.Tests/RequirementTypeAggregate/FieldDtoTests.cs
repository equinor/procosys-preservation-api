using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.Procosys.Preservation.Query.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.RequirementTypeAggregate
{
    [TestClass]
    public class FieldDtoTests
    {
        [TestMethod]
        public void ConstructorForInfoField_ShouldSetProperties()
        {
            var fieldType = FieldType.Info;

            AssertField(fieldType, false);
        }

        [TestMethod]
        public void ConstructorForAttachmentField_ShouldSetProperties()
        {
            var fieldType = FieldType.Attachment;

            AssertField(fieldType, true);
        }

        [TestMethod]
        public void ConstructorForNumberField_ShouldSetProperties()
        {
            var fieldType = FieldType.Number;

            AssertField(fieldType, true);
        }

        [TestMethod]
        public void ConstructorForCheckBoxField_ShouldSetProperties()
        {
            var fieldType = FieldType.CheckBox;

            AssertField(fieldType, true);
        }

        private static void AssertField(FieldType fieldType, bool needUserInput)
        {
            var f = new FieldDto(1, "LabelA", true, fieldType, 10, "UnitA", true);

            Assert.AreEqual(1, f.Id);
            Assert.AreEqual("LabelA", f.Label);
            Assert.AreEqual("UnitA", f.Unit);
            Assert.AreEqual(fieldType, f.FieldType);
            Assert.AreEqual(10, f.SortKey);
            Assert.IsTrue(f.ShowPrevious.HasValue);
            Assert.IsTrue(f.ShowPrevious.Value);
            Assert.IsTrue(f.IsVoided);
            Assert.AreEqual(needUserInput, f.NeedUserInput);
        }
    }
}

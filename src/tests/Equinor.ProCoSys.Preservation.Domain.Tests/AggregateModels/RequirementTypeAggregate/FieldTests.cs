using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.RequirementTypeAggregate
{
    [TestClass]
    public class FieldTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var f = new Field("PlantA", "LabelA", FieldType.Attachment, 10, "UnitA", true);

            Assert.AreEqual("PlantA", f.Plant);
            Assert.AreEqual("LabelA", f.Label);
            Assert.AreEqual("UnitA", f.Unit);
            Assert.AreEqual(FieldType.Attachment, f.FieldType);
            Assert.AreEqual(10, f.SortKey);
            Assert.IsTrue(f.ShowPrevious.HasValue);
            Assert.IsTrue(f.ShowPrevious.Value);
            Assert.IsFalse(f.IsVoided);
        }

        [TestMethod]
        public void ConstructorForInfoField_ShouldMakeFieldNotNeedingUserInput()
        {
            var f = new Field("", "", FieldType.Info, 10);

            Assert.IsFalse(f.NeedsUserInput);
        }

        [TestMethod]
        public void ConstructorForAttachmentField_ShouldMakeFieldNeedingUserInput()
        {
            var f = new Field("", "", FieldType.Attachment, 10);

            Assert.IsTrue(f.NeedsUserInput);
        }

        [TestMethod]
        public void ConstructorForCheckBoxField_ShouldMakeFieldNeedingUserInput()
        {
            var f = new Field("", "", FieldType.CheckBox, 10);

            Assert.IsTrue(f.NeedsUserInput);
        }

        [TestMethod]
        public void ConstructorForNumberField_ShouldMakeFieldNeedingUserInput()
        {
            var f = new Field("", "", FieldType.Number, 10, "mm", true);

            Assert.IsTrue(f.NeedsUserInput);
        }

        [TestMethod]
        public void Constructor_ShouldSetDefaultValuesForUnitAndShowPrevious_WhenNotGivenAsArgument()
        {
            var f = new Field("PlantA", "LabelA", FieldType.Info, 10);

            Assert.IsFalse(f.ShowPrevious.HasValue);
            Assert.IsNull(f.Unit);
            Assert.IsFalse(f.IsVoided);
        }

        [TestMethod]
        public void ConstructorForNumberField_ShouldThrowException_WhenUnitNotGiven()
            => Assert.ThrowsException<ArgumentException>(() => new Field("", "", FieldType.Number, 10, showPrevious: true));

        [TestMethod]
        public void ConstructorForNumberField_ShouldThrowException_WhenShowPreviousNotGiven()
            => Assert.ThrowsException<ArgumentException>(() => new Field("", "", FieldType.Number, 10, "UnitA"));
    }
}

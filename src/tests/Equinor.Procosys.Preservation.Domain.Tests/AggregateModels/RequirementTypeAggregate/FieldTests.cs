using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.RequirementTypeAggregate
{
    [TestClass]
    public class FieldTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var f = new Field("SchemaA", "LabelA", FieldType.Attachment, 10, "UnitA", true);

            Assert.AreEqual("SchemaA", f.Schema);
            Assert.AreEqual("LabelA", f.Label);
            Assert.AreEqual("UnitA", f.Unit);
            Assert.AreEqual(FieldType.Attachment, f.FieldType);
            Assert.AreEqual(10, f.SortKey);
            Assert.IsTrue(f.ShowPrevious.HasValue);
            Assert.IsTrue(f.ShowPrevious.Value);
            Assert.IsFalse(f.IsVoided);
        }

        [TestMethod]
        public void Constructor_ShouldSetDefaulValuesForUnitAndShowPrevious_WhenNotGivenAsArgument()
        {
            var f = new Field("SchemaA", "LabelA", FieldType.Info, 10);

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

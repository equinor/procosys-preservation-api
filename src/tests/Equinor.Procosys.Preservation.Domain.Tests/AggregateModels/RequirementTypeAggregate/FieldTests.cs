using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.RequirementTypeAggregate
{
    [TestClass]
    public class FieldTests
    {
        [TestMethod]
        public void ConstructorSetsPropertiesTest()
        {
            var f = new Field("SchemaA", "LabelA", "UnitA", true, FieldType.Attachment, 10);

            Assert.AreEqual("SchemaA", f.Schema);
            Assert.AreEqual("LabelA", f.Label);
            Assert.AreEqual("UnitA", f.Unit);
            Assert.AreEqual(FieldType.Attachment, f.FieldType);
            Assert.AreEqual(10, f.SortKey);
            Assert.IsTrue(f.ShowPrevious);
            Assert.IsFalse(f.IsVoided);
        }
    }
}

using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class NumberValueTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties_WhenNullValue()
        {
            var fieldMock = new Mock<Field>();
            fieldMock.SetupGet(f => f.Id).Returns(54);
            var f = new NumberValue("SchemaA", fieldMock.Object, null);

            Assert.AreEqual("SchemaA", f.Schema);
            Assert.AreEqual(54, f.FieldId);
            Assert.IsFalse(f.Value.HasValue);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WhenValue()
        {
            var f = new NumberValue("SchemaA", new Mock<Field>().Object, 1.4);

            Assert.AreEqual("SchemaA", f.Schema);
            Assert.IsTrue(f.Value.HasValue);
            Assert.AreEqual(1.4, f.Value.Value);
        }
    }
}

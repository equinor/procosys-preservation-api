using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests
{
    [TestClass]
    public class SchemaEntityBaseTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new TestableSchemaEntityBase("PlantA");

            Assert.AreEqual("PlantA", dut.Plant);
        }
    }

    public class TestableSchemaEntityBase : PlantEntityBase
    {
        public TestableSchemaEntityBase(string schema)
            : base(schema)
        {
        }

        // The base class is abstract, therefor a sub class is needed to test it.
    }
}

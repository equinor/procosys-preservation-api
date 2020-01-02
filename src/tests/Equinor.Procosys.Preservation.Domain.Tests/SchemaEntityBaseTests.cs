using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests
{
    [TestClass]
    public class SchemaEntityBaseTests
    {
        [TestMethod]
        public void ConstructorSetsPropertiesTest()
        {
            var dut = new TestableSchemaEntityBase("SchemaA");

            Assert.AreEqual("SchemaA", dut.Schema);
        }
    }

    public class TestableSchemaEntityBase : SchemaEntityBase
    {
        public TestableSchemaEntityBase(string schema)
            : base(schema)
        {
        }

        // The base class is abstract, therefor a sub class is needed to test it.
    }
}

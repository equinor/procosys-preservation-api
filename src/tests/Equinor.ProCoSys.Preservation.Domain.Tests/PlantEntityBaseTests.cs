using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Domain.Tests
{
    [TestClass]
    public class PlantEntityBaseTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new TestablePlantEntityBase("PlantA");

            Assert.AreEqual("PlantA", dut.Plant);
        }
    }

    public class TestablePlantEntityBase : PlantEntityBase
    {
        public TestablePlantEntityBase(string schema)
            : base(schema)
        {
        }

        // The base class is abstract, therefor a sub class is needed to test it.
    }
}

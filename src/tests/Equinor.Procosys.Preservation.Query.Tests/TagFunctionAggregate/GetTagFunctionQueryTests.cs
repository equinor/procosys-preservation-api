using Equinor.Procosys.Preservation.Query.TagFunctionAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.TagFunctionAggregate
{
    [TestClass]
    public class GetTagFunctionQueryTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new GetTagFunctionQuery("A", "B");
            Assert.AreEqual("A", dut.TagFunctionCode);
            Assert.AreEqual("B", dut.RegisterCode);
        }
    }
}

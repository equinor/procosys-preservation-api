using Equinor.Procosys.Preservation.Query.GetTagFunction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagFunction
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

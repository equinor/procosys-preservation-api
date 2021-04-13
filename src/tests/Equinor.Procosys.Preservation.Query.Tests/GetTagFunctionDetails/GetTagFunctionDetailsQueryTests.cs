using Equinor.ProCoSys.Preservation.Query.GetTagFunctionDetails;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetTagFunctionDetails
{
    [TestClass]
    public class GetTagFunctionDetailsQueryTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new GetTagFunctionDetailsQuery("A", "B");
            Assert.AreEqual("A", dut.TagFunctionCode);
            Assert.AreEqual("B", dut.RegisterCode);
        }
    }
}

using Equinor.Procosys.Preservation.Query.GetTagFunctionsHavingRequirement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagFunctionsHavingRequirement
{
    [TestClass]
    public class TagFunctionDetailsDtoTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new TagFunctionDto(1, "TFC", "RC");

            Assert.AreEqual(1, dut.Id);
            Assert.AreEqual("TFC", dut.Code);
            Assert.AreEqual("RC", dut.RegisterCode);
        }
    }
}

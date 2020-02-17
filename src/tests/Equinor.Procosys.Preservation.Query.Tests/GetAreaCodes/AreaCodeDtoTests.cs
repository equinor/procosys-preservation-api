using Equinor.Procosys.Preservation.Query.GetAreas;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetAreaCodes
{
    [TestClass]
    public class AreaCodeDtoTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new AreaDto("CodeA", "DescriptionA");

            Assert.AreEqual("CodeA", dut.Code);
            Assert.AreEqual("DescriptionA", dut.Description);
        }
    }
}

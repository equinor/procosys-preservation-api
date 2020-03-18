using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Query.GetTagFunction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagFunction
{
    [TestClass]
    public class TagFunctionDtoTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var reqDto = new RequirementDto(2, 4);

            var dut = new TagFunctionDto(1, "TFC", "D", "RC", true, new List<RequirementDto>{reqDto});

            Assert.AreEqual(1, dut.Id);
            Assert.AreEqual("TFC", dut.Code);
            Assert.AreEqual("D", dut.Description);
            Assert.AreEqual("RC", dut.RegisterCode);
            Assert.IsTrue(dut.IsVoided);
            Assert.AreEqual(1, dut.Requirements.Count());
            Assert.AreEqual(reqDto, dut.Requirements.First());
        }
    }
}

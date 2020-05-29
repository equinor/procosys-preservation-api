using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Query.GetTagFunctionDetails;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTagFunctionDetails
{
    [TestClass]
    public class TagFunctionDetailsDtoTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var reqDto = new RequirementDto(2, 4, 6);

            var dut = new TagFunctionDetailsDto(
                1,
                "TFC",
                "D",
                "RC",
                true,
                new List<RequirementDto>{reqDto},
                "AAAAAAAAABA=");

            Assert.AreEqual(1, dut.Id);
            Assert.AreEqual("TFC", dut.Code);
            Assert.AreEqual("D", dut.Description);
            Assert.AreEqual("RC", dut.RegisterCode);
            Assert.IsTrue(dut.IsVoided);
            Assert.AreEqual(1, dut.Requirements.Count());
            Assert.AreEqual(reqDto, dut.Requirements.First());
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }
    }
}

using Equinor.ProCoSys.Preservation.Query.GetTagFunctionDetails;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetTagFunctionDetails
{
    [TestClass]
    public class RequirementDtoTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new RequirementDto(2, 3, 4);

            Assert.AreEqual(2, dut.Id);
            Assert.AreEqual(3, dut.RequirementDefinitionId);
            Assert.AreEqual(4, dut.IntervalWeeks);
        }
    }
}

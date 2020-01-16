using Equinor.Procosys.Preservation.Query.AllAvailableTagsQuery;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.AllAvailableTagsQuery
{
    [TestClass]
    public class GetAllAvailableTagsQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new GetAllAvailableTagsQuery("ProjectName", "TagNo");

            Assert.AreEqual("ProjectName", dut.ProjectName);
            Assert.AreEqual("TagNo", dut.StartsWithTagNo);
        }
    }
}

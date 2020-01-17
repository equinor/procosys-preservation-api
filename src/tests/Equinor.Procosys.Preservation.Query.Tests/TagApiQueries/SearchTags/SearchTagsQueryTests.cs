using Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.TagApiQueries.SearchTags
{
    [TestClass]
    public class SearchTagsQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new SearchTagsQuery("ProjectName", "TagNo");

            Assert.AreEqual("ProjectName", dut.ProjectName);
            Assert.AreEqual("TagNo", dut.StartsWithTagNo);
        }
    }
}

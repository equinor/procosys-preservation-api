using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.TagApiQueries.SearchTags
{
    [TestClass]
    public class SearchTagsByTagFunctionQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new SearchTagsByTagFunctionQuery("ProjectName", "A", "B");

            Assert.AreEqual("ProjectName", dut.ProjectName);
            Assert.AreEqual("A", dut.TagFunctionCode);
            Assert.AreEqual("B", dut.RegisterCode);
        }
    }
}

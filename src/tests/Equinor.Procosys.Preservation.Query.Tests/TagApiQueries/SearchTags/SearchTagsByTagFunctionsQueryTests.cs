using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Query.TagApiQueries.SearchTags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.TagApiQueries.SearchTags
{
    [TestClass]
    public class SearchTagsByTagFunctionsQueryTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new SearchTagsByTagFunctionsQuery("ProjectName", new List<string>{"A|B"});

            Assert.AreEqual("ProjectName", dut.ProjectName);
            Assert.AreEqual(1, dut.TagFunctionCodeRegisterCodePairs.Count());
            Assert.AreEqual("A|B", dut.TagFunctionCodeRegisterCodePairs.Single());
        }
    }
}

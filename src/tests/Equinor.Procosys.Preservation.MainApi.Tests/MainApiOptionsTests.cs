using Equinor.Procosys.Preservation.MainApi.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.MainApi.Tests
{
    [TestClass]
    public class MainApiOptionsTests
    {
        [TestMethod]
        public void TagSearchPageSize_Is100ByDefault()
        {
            var dut = new MainApiOptions();

            Assert.AreEqual(100, dut.TagSearchPageSize);
        }
    }
}

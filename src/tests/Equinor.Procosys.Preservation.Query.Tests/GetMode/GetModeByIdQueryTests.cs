using Equinor.Procosys.Preservation.Query.GetMode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetMode
{
    [TestClass]
    public class GetModeByIdQueryTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new GetModeByIdQuery(55);

            Assert.AreEqual(55, dut.Id);
        }
    }
}

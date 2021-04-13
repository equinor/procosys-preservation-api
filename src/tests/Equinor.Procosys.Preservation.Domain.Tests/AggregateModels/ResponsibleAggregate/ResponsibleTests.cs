using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.ResponsibleAggregate
{
    [TestClass]
    public class ResponsibleTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new Responsible("PlantA", "CodeA", "DescA");

            Assert.AreEqual("PlantA", dut.Plant);
            Assert.AreEqual("CodeA", dut.Code);
            Assert.AreEqual("DescA", dut.Description);
        }
    }
}

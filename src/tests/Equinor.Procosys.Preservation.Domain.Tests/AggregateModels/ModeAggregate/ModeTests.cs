using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.ModeAggregate
{
    [TestClass]
    public class ModeTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new Mode("PlantA", "TitleA", false);

            Assert.AreEqual("PlantA", dut.Plant);
            Assert.AreEqual("TitleA", dut.Title);
            Assert.AreEqual(false, dut.ForSupplier);
        }
    }
}

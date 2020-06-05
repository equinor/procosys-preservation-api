using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ModeAggregate
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

        [TestMethod]
        public void VoidUnVoid_ShouldToggleIsVoided()
        {
            var dut = new Mode("PlantA", "TitleA", false);
            Assert.IsFalse(dut.IsVoided);

            dut.Void();
            Assert.IsTrue(dut.IsVoided);

            dut.UnVoid();
            Assert.IsFalse(dut.IsVoided);
        }
    }
}

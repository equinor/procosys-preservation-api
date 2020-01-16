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
            var dut = new Mode("SchemaA", "TitleA");

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual("TitleA", dut.Title);
        }

        [TestMethod]
        public void VoidUnVoid_ShouldToggleIsVoided()
        {
            var dut = new Mode("SchemaA", "TitleA");
            Assert.IsFalse(dut.IsVoided);

            dut.Void();
            Assert.IsTrue(dut.IsVoided);

            dut.UnVoid();
            Assert.IsFalse(dut.IsVoided);
        }
    }
}

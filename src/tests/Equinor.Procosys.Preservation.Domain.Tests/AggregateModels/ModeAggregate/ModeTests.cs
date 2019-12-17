using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ModeAggregate
{
    [TestClass]
    public class ModeTests
    {
        [TestMethod]
        public void ConstructorSetsPropertiesTest()
        {
            var dut = new Mode("SchemaA", "TitleA");

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual("TitleA", dut.Title);
        }
    }
}

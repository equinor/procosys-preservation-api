using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ResponsibleAggregate
{
    [TestClass]
    public class ResponsibleTests
    {
        [TestMethod]
        public void ConstructorSetsPropertiesTest()
        {
            Responsible dut = new Responsible("SchemaA", "NameA");

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual("NameA", dut.Name);
        }
    }
}

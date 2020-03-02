using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ResponsibleAggregate
{
    [TestClass]
    public class ResponsibleTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new Responsible("SchemaA", "CodeA", "TitleA");

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual("CodeA", dut.Code);
            Assert.AreEqual("TitleA", dut.Title);
        }
    }
}

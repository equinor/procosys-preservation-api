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
            var dut = new Responsible("PlantA", "CodeA", "TitleA");

            Assert.AreEqual("PlantA", dut.Plant);
            Assert.AreEqual("CodeA", dut.Code);
            Assert.AreEqual("TitleA", dut.Title);
        }
    }
}

using Equinor.Procosys.Preservation.Query.GetDisciplines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetDisciplines
{
    [TestClass]
    public class DisciplineDtoTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var dut = new DisciplineDto("CodeA", "DescriptionA");

            Assert.AreEqual("CodeA", dut.Code);
            Assert.AreEqual("DescriptionA", dut.Description);
        }
    }
}

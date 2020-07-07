using Equinor.Procosys.Preservation.Command.RequirementTypeCommands.CreateRequirementType;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.RequirementTypeCommands.CreateRequirementType
{
    [TestClass]
    public class CreateRequirementTypeCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new CreateRequirementTypeCommand(10, "code", "title");

            Assert.AreEqual("title", dut.Title);
            Assert.AreEqual("code", dut.Code);
        }
    }
}

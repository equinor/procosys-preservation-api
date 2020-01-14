using Equinor.Procosys.Preservation.Command.TagCommands.CreateTag;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands
{
    [TestClass]
    public class CreateTagCommandTests
    {
        [TestMethod]
        public void ConstructorSetsPropertiesTest()
        {
            CreateTagCommand dut = new CreateTagCommand("TagNumberA", "ProjectNumberA", 1, 2, "DescriptionA");

            Assert.AreEqual("TagNumberA", dut.TagNumber);
            Assert.AreEqual("ProjectNumberA", dut.ProjectNumber);
            Assert.AreEqual(1, dut.JourneyId);
            Assert.AreEqual(2, dut.StepId);
            Assert.AreEqual("DescriptionA", dut.Description);
        }
    }
}

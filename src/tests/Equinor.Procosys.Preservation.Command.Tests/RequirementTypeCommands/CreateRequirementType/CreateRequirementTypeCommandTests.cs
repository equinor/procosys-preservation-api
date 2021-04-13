using Equinor.ProCoSys.Preservation.Command.RequirementTypeCommands.CreateRequirementType;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.RequirementTypeCommands.CreateRequirementType
{
    [TestClass]
    public class CreateRequirementTypeCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new CreateRequirementTypeCommand(10, "code", "title", RequirementTypeIcon.Other);

            Assert.AreEqual("title", dut.Title);
            Assert.AreEqual("code", dut.Code);
        }
    }
}

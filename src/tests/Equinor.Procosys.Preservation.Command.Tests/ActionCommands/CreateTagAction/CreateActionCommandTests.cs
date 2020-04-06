using System;
using Equinor.Procosys.Preservation.Command.ActionCommands.CreateAction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.ActionCommands.CreateAction
{
    [TestClass]
    public class CreateActionCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dueTimeUtc = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            var dut = new CreateActionCommand(2, "Title", "Description", dueTimeUtc);

            Assert.AreEqual(2, dut.TagId);
            Assert.AreEqual("Title", dut.Title);
            Assert.AreEqual("Description", dut.Description);
            Assert.AreEqual(dueTimeUtc, dut.DueTimeUtc);
        }
    }
}

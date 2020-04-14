using System;
using Equinor.Procosys.Preservation.Command.ActionCommands.UpdateAction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.ActionCommands.UpdateAction
{
    [TestClass]
    public class UpdateActionCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dueTimeUtc = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            var dut = new UpdateActionCommand(2, 1, "Title", "Description", dueTimeUtc);

            Assert.AreEqual(2, dut.TagId);
            Assert.AreEqual(1, dut.ActionId);
            Assert.AreEqual("Title", dut.Title);
            Assert.AreEqual("Description", dut.Description);
            Assert.AreEqual(dueTimeUtc, dut.DueTimeUtc);
        }
    }
}

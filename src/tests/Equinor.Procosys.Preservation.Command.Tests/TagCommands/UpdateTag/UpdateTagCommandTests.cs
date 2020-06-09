using System;
using Equinor.Procosys.Preservation.Command.TagCommands.UpdateTag;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.UpdateTag
{
    [TestClass]
    public class UpdateTagCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new UpdateTagCommand(2, "Remark", "StorageArea", "AAAAAAAAABA=", Guid.Empty);

            Assert.AreEqual(2, dut.TagId);
            Assert.AreEqual("Remark", dut.Remark);
            Assert.AreEqual("StorageArea", dut.StorageArea);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
            Assert.AreEqual(Guid.Empty, dut.CurrentUserOid);
        }
    }
}

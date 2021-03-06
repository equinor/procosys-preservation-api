﻿using Equinor.ProCoSys.Preservation.Command.ModeCommands.UnvoidMode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Command.Tests.ModeCommands.UnvoidMode
{
    [TestClass]
    public class UnvoidModeCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new UnvoidModeCommand(1, "AAAAAAAAABA=");

            Assert.AreEqual(1, dut.ModeId);
            Assert.AreEqual("AAAAAAAAABA=", dut.RowVersion);
        }
    }
}

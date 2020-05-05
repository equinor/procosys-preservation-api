﻿using Equinor.Procosys.Preservation.Command.ModeCommands.UpdateMode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.ModeCommands.UpdateMode
{
    [TestClass]
    public class UpdateModeCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new UpdateModeCommand(1, "ModeTitle", 12345);
            Assert.AreEqual(1, dut.ModeId);
            Assert.AreEqual("ModeTitle", dut.Title);
        }
    }
}

﻿using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Command.TagCommands.Preserve;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.Preserve
{
    [TestClass]
    public class PreserveCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new PreserveCommand(new List<int>{17});

            Assert.AreEqual(1, dut.TagIds.Count());
            Assert.AreEqual(17, dut.TagIds.First());
        }
    }
}

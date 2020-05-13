﻿using Equinor.Procosys.Preservation.Command.JourneyCommands.UnvoidJourney;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.JourneyCommands.UnvoidJourney
{
    [TestClass]
    public class UnvoidJourneyCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new UnvoidJourneyCommand(2);

            Assert.AreEqual(2, dut.JourneyId);
        }
    }
}

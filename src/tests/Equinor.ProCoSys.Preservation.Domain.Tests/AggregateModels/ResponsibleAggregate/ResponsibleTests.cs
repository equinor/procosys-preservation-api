using System;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.ResponsibleAggregate
{
    [TestClass]
    public class ResponsibleTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new Responsible("PlantA", "CodeA", "DescA");

            Assert.AreEqual("PlantA", dut.Plant);
            Assert.AreEqual("CodeA", dut.Code);
            Assert.AreEqual("DescA", dut.Description);
        }

        [TestMethod]
        public void RenameResponsible_ShouldSetNewCode()
        {
            var newCode = "Code9";
            var dut = new Responsible("PlantA", "CodeA", "DescA");
            Assert.AreNotEqual(newCode, dut.Code);

            dut.RenameResponsible(newCode);

            Assert.AreEqual(newCode, dut.Code);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RenameResponsible_ShouldOnNullCode()
        {
            var dut = new Responsible("PlantA", "CodeA", "DescA");

            dut.RenameResponsible(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RenameResponsible_ShouldOnEmptyCode()
        {
            var dut = new Responsible("PlantA", "CodeA", "DescA");

            dut.RenameResponsible(" ");
        }
    }
}

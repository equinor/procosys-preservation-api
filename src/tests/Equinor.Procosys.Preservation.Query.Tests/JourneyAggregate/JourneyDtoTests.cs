using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Query.JourneyAggregate;
using Equinor.Procosys.Preservation.Query.ModeAggregate;
using Equinor.Procosys.Preservation.Query.ResponsibleAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.JourneyAggregate
{
    [TestClass]
    public class JourneyDtoTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var stepDto = new StepDto(2,
                "S",
                true,
                new ModeDto(3, "M"),
                new ResponsibleDto(4, "RC", "RT"));
            var dut = new JourneyDto(
                1,
                "J",
                true,
                new List<StepDto>
                {
                    stepDto
                });

            Assert.AreEqual(1, dut.Id);
            Assert.AreEqual("J", dut.Title);
            Assert.IsTrue(dut.IsVoided);
            Assert.AreEqual(1, dut.Steps.Count());
            Assert.AreEqual(stepDto, dut.Steps.First());

        }
    }
}

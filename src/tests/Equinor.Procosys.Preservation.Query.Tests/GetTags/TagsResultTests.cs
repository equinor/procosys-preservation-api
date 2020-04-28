using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Query.GetTags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Query.Tests.GetTags
{
    [TestClass]
    public class TagsResultTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var tagDto = new TagDto(
                1,
                ActionStatus.HasOverDue,
                "AreaCode",
                "CallOffNo",
                "CommPkgNo",
                "DisciplineCode",
                true,
                true,
                "McPkgNo",
                "Mode",
                "NextMode",
                "NextResp",
                true,
                true,
                true,
                "PoNo",
                new List<RequirementDto> {new RequirementDto(0, null, default, default, false)},
                "Resp",
                PreservationStatus.Active,
                "SA",
                "TagFunctionCode",
                "TagDesc",
                "TagNo",
                TagType.Standard,
                1234567890);
            var dut = new TagsResult(10, new List<TagDto>{ tagDto });
            Assert.AreEqual(10, dut.MaxAvailable);
            Assert.IsNotNull(dut.Tags);
            Assert.AreEqual(1, dut.Tags.Count());
            Assert.AreEqual(tagDto, dut.Tags.Single());
        }

        [TestMethod]
        public void Constructor_SetsProperties_WhenNoTags()
        {
            var dut = new TagsResult(10, null);
            Assert.AreEqual(10, dut.MaxAvailable);
            Assert.IsNotNull(dut.Tags);
            Assert.AreEqual(0, dut.Tags.Count());
        }
    }
}

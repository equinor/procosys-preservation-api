using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries;
using Equinor.ProCoSys.Preservation.Query.GetTagsQueries.GetTags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Query.Tests.GetTagsQueries.GetTags
{
    [TestClass]
    public class TagsResultTests
    {
        [TestMethod]
        public void Constructor_SetsProperties()
        {
            var tagDto = new TagDto(
                1,
                ActionStatus.HasOverdue,
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
                true,
                true,
                true,
                true,
                true,
                true,
                "PoNo",
                new List<RequirementDto> { new RequirementDto(0, null, RequirementTypeIcon.Other, default, default, false) },
                "Resp",
                "RespDescription",
                PreservationStatus.Active,
                "SA",
                "TagFunctionCode",
                "TagDesc",
                "TagNo",
                TagType.Standard,
                "AAAAAAAAABA=");
            var dut = new TagsResult(10, new List<TagDto> { tagDto });
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

using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class AbstractAreaTagTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties_WithRequirements()
        {
            var dut = new TestAreaTag(
                TagType.PreArea,
                "DisciplineA",
                "AreaA",
                null);

            Assert.AreEqual(TagType.PreArea, dut.TagType);
            Assert.AreEqual("DisciplineA", dut.DisciplineCode);
            Assert.AreEqual("AreaA", dut.AreaCode);
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnPreTagNo_WithDiscipline()
        {
            var dut = new TestAreaTag(
                TagType.PreArea,
                "I",
                null,
                null);

            Assert.AreEqual("#PRE-I", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnSiteTagNo_WithDisciplineAndSuffix()
        {
            var dut = new TestAreaTag(
                TagType.SiteArea,
                "I",
                null,
                null);

            Assert.AreEqual("#SITE-I", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnPreTagNo_WithDisciplineAndSuffix()
        {
            var dut = new TestAreaTag(
                TagType.PreArea,
                "I",
                null,
                "XX");

            Assert.AreEqual("#PRE-I-XX", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnSiteTagNo_WithDiscipline()
        {
            var dut = new TestAreaTag(
                TagType.SiteArea,
                "I",
                null,
                "XX");

            Assert.AreEqual("#SITE-I-XX", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnPreTagNo_WithDisciplineAndArea()
        {
            var dut = new TestAreaTag(
                TagType.PreArea,
                "I",
                "A300",
                null);

            Assert.AreEqual("#PRE-I-A300", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnSiteTagNo_WithDisciplineAndArea()
        {
            var dut = new TestAreaTag(
                TagType.SiteArea,
                "I",
                "A300",
                null);

            Assert.AreEqual("#SITE-I-A300", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnPreTagNo_WithDisciplineAndAreaAndSuffix()
        {
            var dut = new TestAreaTag(
                TagType.PreArea,
                "I",
                "A300",
                "XX");

            Assert.AreEqual("#PRE-I-A300-XX", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnSiteTagNo_WithDisciplineAndAreaAndSuffix()
        {
            var dut = new TestAreaTag(
                TagType.SiteArea,
                "I",
                "A300",
                "XX");

            Assert.AreEqual("#SITE-I-A300-XX", dut.GetTagNo());
        }

        public class TestAreaTag : AbstractAreaTag
        {
            public TestAreaTag(TagType tagType, string disciplineCode, string areaCode, string tagNoSuffix)
            {
                TagType = tagType;
                DisciplineCode = disciplineCode;
                AreaCode = areaCode;
                TagNoSuffix = tagNoSuffix;
            }

            public override TagType TagType { get; }
            public override string DisciplineCode { get; }
            public override string AreaCode { get; }
            public override string TagNoSuffix { get; }
        }
    }
}

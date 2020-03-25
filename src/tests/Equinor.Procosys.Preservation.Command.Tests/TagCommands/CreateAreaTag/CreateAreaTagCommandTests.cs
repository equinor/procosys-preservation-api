using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Command.TagCommands.CreateAreaTag;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.Procosys.Preservation.Command.Tests.TagCommands.CreateAreaTag
{
    [TestClass]
    public class CreateAreaTagCommandTests
    {
        [TestMethod]
        public void Constructor_ShouldSetProperties_WithRequirements()
        {
            var dut = new CreateAreaTagCommand(
                "ProjectNameA",
                TagType.PreArea,
                "DisciplineA",
                "AreaA",
                null,
                2,
                new List<Requirement>{new Requirement(11, 12)},
                "DescriptionA",
                "RemarkA",
                "SA_A");

            Assert.AreEqual("ProjectNameA", dut.ProjectName);
            Assert.AreEqual("DisciplineA", dut.DisciplineCode);
            Assert.AreEqual("AreaA", dut.AreaCode);
            Assert.AreEqual("DescriptionA", dut.Description);
            Assert.AreEqual("RemarkA", dut.Remark);
            Assert.AreEqual("SA_A", dut.StorageArea);
            Assert.AreEqual(2, dut.StepId);
            Assert.AreEqual(1, dut.Requirements.Count());
            var requirement = dut.Requirements.First();
            Assert.AreEqual(11, requirement.RequirementDefinitionId);
            Assert.AreEqual(12, requirement.IntervalWeeks);
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnPreTagNo_WithDiscipline()
        {
            var dut = new CreateAreaTagCommand(
                "",
                TagType.PreArea,
                "I",
                null,
                null,
                0,
                null,
                null,
                null,
                null);

            Assert.AreEqual("#PRE-I", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnSiteTagNo_WithDisciplineAndSuffix()
        {
            var dut = new CreateAreaTagCommand(
                "",
                TagType.SiteArea,
                "I",
                null,
                null,
                0,
                null,
                null,
                null,
                null);

            Assert.AreEqual("#SITE-I", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnPreTagNo_WithDisciplineAndSuffix()
        {
            var dut = new CreateAreaTagCommand(
                "",
                TagType.PreArea,
                "I",
                null,
                "XX",
                0,
                null,
                null,
                null,
                null);

            Assert.AreEqual("#PRE-I-XX", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnSiteTagNo_WithDiscipline()
        {
            var dut = new CreateAreaTagCommand(
                "",
                TagType.SiteArea,
                "I",
                null,
                "XX",
                0,
                null,
                null,
                null,
                null);

            Assert.AreEqual("#SITE-I-XX", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnPreTagNo_WithDisciplineAndArea()
        {
            var dut = new CreateAreaTagCommand(
                "",
                TagType.PreArea,
                "I",
                "A300",
                null,
                0,
                null,
                null,
                null,
                null);

            Assert.AreEqual("#PRE-I-A300", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnSiteTagNo_WithDisciplineAndArea()
        {
            var dut = new CreateAreaTagCommand(
                "",
                TagType.SiteArea,
                "I",
                "A300",
                null,
                0,
                null,
                null,
                null,
                null);

            Assert.AreEqual("#SITE-I-A300", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnPreTagNo_WithDisciplineAndAreaAndSuffix()
        {
            var dut = new CreateAreaTagCommand(
                "",
                TagType.PreArea,
                "I",
                "A300",
                "XX",
                0,
                null,
                null,
                null,
                null);

            Assert.AreEqual("#PRE-I-A300-XX", dut.GetTagNo());
        }

        [TestMethod]
        public void GetTagNo_ShouldReturnSiteTagNo_WithDisciplineAndAreaAndSuffix()
        {
            var dut = new CreateAreaTagCommand(
                "",
                TagType.SiteArea,
                "I",
                "A300",
                "XX",
                0,
                null,
                null,
                null,
                null);

            Assert.AreEqual("#SITE-I-A300-XX", dut.GetTagNo());
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties_WithNullLists()
        {
            var dut = new CreateAreaTagCommand("ProjectNameA",
                TagType.PreArea,
                "DiscA",
                "AreaA",
                null,
                2,
                null,
                null,
                null,
                null);

            Assert.IsNotNull(dut.Requirements);
            Assert.AreEqual(0, dut.Requirements.Count());
            Assert.IsNull(dut.Description);
            Assert.IsNull(dut.Remark);
            Assert.IsNull(dut.StorageArea);
        }
    }
}

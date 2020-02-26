using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class ProjectTests
    {
        private const string TestPlant = "PlantA";
        private readonly Project _dut = new Project(TestPlant, "ProjectNameA", "DescA");

        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual(TestPlant, _dut.Schema);
            Assert.AreEqual("ProjectNameA", _dut.Name);
            Assert.AreEqual("DescA", _dut.Description);
            Assert.IsFalse(_dut.IsClosed);
        }

        [TestMethod]
        public void AddTag_ShouldThrowExceptionTest_ForNullTag()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _dut.AddTag(null));
            Assert.AreEqual(0, _dut.Tags.Count);
        }

        [TestMethod]
        public void AddTag_ShouldAddTagToTagsList()
        {
            var tag = new Tag(TestPlant, TagType.Standard, "", "", "", "", "", "", "", "", "", "", new Mock<Step>().Object, new List<Requirement>{new Mock<Requirement>().Object});

            _dut.AddTag(tag);

            Assert.AreEqual(1, _dut.Tags.Count);
            Assert.IsTrue(_dut.Tags.Contains(tag));
        }

        [TestMethod]
        public void Close_ShouldCloseProject()
        {
            _dut.Close();

            Assert.IsTrue(_dut.IsClosed);
        }
    }
}

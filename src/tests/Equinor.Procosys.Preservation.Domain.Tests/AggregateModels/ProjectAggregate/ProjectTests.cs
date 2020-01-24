using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class ProjectTests
    {
        [TestInitialize]
        public void Setup()
        {
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new Project("SchemaA", "ProjectNameA", "DescA");

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual("ProjectNameA", dut.Name);
            Assert.AreEqual("DescA", dut.Description);
            Assert.IsFalse(dut.IsClosed);
        }

        [TestMethod]
        public void AddTag_ShouldThrowExceptionTest_ForNullTag()
        {
            var dut = new Project("", "", "");

            Assert.ThrowsException<ArgumentNullException>(() => dut.AddTag(null));
            Assert.AreEqual(0, dut.Tags.Count);
        }

        [TestMethod]
        public void AddTag_ShouldAddTagToTagsList()
        {
            var tagMock = new Mock<Tag>();

            var dut = new Project("", "", "");
            dut.AddTag(tagMock.Object);

            Assert.AreEqual(1, dut.Tags.Count);
            Assert.IsTrue(dut.Tags.Contains(tagMock.Object));
        }

        [TestMethod]
        public void Close_ShouldCloseProject()
        {
            var dut = new Project("", "", "");

            dut.Close();

            Assert.IsTrue(dut.IsClosed);
        }
    }
}

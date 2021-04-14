using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class ProjectTests
    {
        private const string TestPlant = "PlantA";
        private Project _dut;
        private Mock<Step> _stepMock;
        private Mock<TagRequirement> _reqMock;
        private Tag _tag;

        [TestInitialize]
        public void Setup()
        {
            _stepMock = new Mock<Step>();
            _stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
            _reqMock = new Mock<TagRequirement>();
            _reqMock.SetupGet(r => r.Plant).Returns(TestPlant);
            _tag = new Tag(TestPlant, TagType.Standard, "", "", _stepMock.Object, new List<TagRequirement>{_reqMock.Object});
            _dut = new Project(TestPlant, "ProjectNameA", "DescA");
            _dut.AddTag(_tag);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual(TestPlant, _dut.Plant);
            Assert.AreEqual("ProjectNameA", _dut.Name);
            Assert.AreEqual("DescA", _dut.Description);
            Assert.IsFalse(_dut.IsClosed);
        }

        [TestMethod]
        public void AddTag_ShouldThrowExceptionTest_WhenTagNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dut.AddTag(null));

        [TestMethod]
        public void AddTag_ShouldAddTagToTagsList()
        {
            var newTag = new Tag(TestPlant, TagType.Standard, "", "", _stepMock.Object, new List<TagRequirement>{_reqMock.Object});

            _dut.AddTag(newTag);

            Assert.AreEqual(2, _dut.Tags.Count);
            Assert.IsTrue(_dut.Tags.Contains(newTag));
        }

        [TestMethod]
        public void RemoveTag_ShouldThrowExceptionTest_WhenTagNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dut.RemoveTag(null));

        [TestMethod]
        public void RemoveTag_ShouldThrowExceptionTest_WhenTagIsNotVoided()
            => Assert.ThrowsException<Exception>(() => _dut.RemoveTag(_tag));

        [TestMethod]
        public void RemoveTag_ShouldRemoveTagFromTagsList()
        {
            // Arrange
            Assert.AreEqual(1, _dut.Tags.Count);
            _tag.IsVoided = true;

            // Act
            _dut.RemoveTag(_tag);

            // Assert
            Assert.AreEqual(0, _dut.Tags.Count);
        }

        [TestMethod]
        public void Close_ShouldCloseProject()
        {
            _dut.Close();

            Assert.IsTrue(_dut.IsClosed);
        }
    }
}

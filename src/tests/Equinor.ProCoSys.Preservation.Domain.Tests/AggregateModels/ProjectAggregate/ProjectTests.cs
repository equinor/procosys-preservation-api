using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class ProjectTests
    {
        private const string TestPlant = "PlantA";
        private const string CommPkgNo = "C1";
        private const string McPkgNo = "M1";
        private const string CommPkgNo2 = "C2";
        private const string McPkgNo2 = "M1";
        private Project _dut;
        private Mock<Step> _stepMock;
        private Mock<TagRequirement> _reqMock;
        private Tag _tag1;
        private Tag _tag2;

        [TestInitialize]
        public void Setup()
        {
            _stepMock = new Mock<Step>();
            _stepMock.SetupGet(s => s.Plant).Returns(TestPlant);
            _reqMock = new Mock<TagRequirement>();
            _reqMock.SetupGet(r => r.Plant).Returns(TestPlant);
            _tag1 = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "Tag1", "", _stepMock.Object,
                new List<TagRequirement> { _reqMock.Object })
            { McPkgNo = McPkgNo, CommPkgNo = CommPkgNo };
            _tag2 = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "Tag2", "", _stepMock.Object,
                    new List<TagRequirement> { _reqMock.Object })
            { McPkgNo = McPkgNo2, CommPkgNo = CommPkgNo2 };
            _dut = new Project(TestPlant, "ProjectNameA", "DescA", new Guid("aec8297b-b010-4c5d-91e0-7b1c8664ced8"));
            _dut.AddTag(_tag1);
            _dut.AddTag(_tag2);
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
            var newTag = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "", "", _stepMock.Object, new List<TagRequirement> { _reqMock.Object });

            _dut.AddTag(newTag);

            Assert.AreEqual(3, _dut.Tags.Count);
            Assert.IsTrue(_dut.Tags.Contains(newTag));
        }

        [TestMethod]
        public void RemoveTag_ShouldThrowExceptionTest_WhenTagNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dut.RemoveTag(null));

        [TestMethod]
        public void RemoveTag_ShouldThrowExceptionTest_WhenTagIsNotVoided()
            => Assert.ThrowsException<Exception>(() => _dut.RemoveTag(_tag1));

        [TestMethod]
        public void RemoveTag_ShouldRemoveTagFromTagsList()
        {
            // Arrange
            Assert.AreEqual(2, _dut.Tags.Count);
            _tag1.IsVoided = true;

            // Act
            _dut.RemoveTag(_tag1);

            // Assert
            Assert.AreEqual(1, _dut.Tags.Count);
        }

        [TestMethod]
        public void Close_ShouldCloseProject()
        {
            _dut.Close();

            Assert.IsTrue(_dut.IsClosed);
        }

        [TestMethod]
        public void RenameMcPkg_ShouldRenameOnAffectedTags()
        {
            // Arrange & Assert
            Assert.AreEqual(McPkgNo, _tag1.McPkgNo);
            Assert.AreEqual(CommPkgNo, _tag1.CommPkgNo);
            Assert.AreEqual(McPkgNo2, _tag2.McPkgNo);
            Assert.AreEqual(CommPkgNo2, _tag2.CommPkgNo);
            var toMcPkg = "New name";

            // Act
            _dut.RenameMcPkg(CommPkgNo, McPkgNo, toMcPkg);

            // Assert
            Assert.AreEqual(toMcPkg, _tag1.McPkgNo);
            Assert.AreEqual(CommPkgNo, _tag1.CommPkgNo);
            Assert.AreEqual(McPkgNo2, _tag2.McPkgNo);
            Assert.AreEqual(CommPkgNo2, _tag2.CommPkgNo);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RenameMcPkg_ShouldFailIfEmptyParameter()
        {
            // Act
            _dut.RenameMcPkg(CommPkgNo, "", "NewName");
            _dut.RenameMcPkg(CommPkgNo, McPkgNo, "");
            _dut.RenameMcPkg("", McPkgNo, "NewName");
        }

        [TestMethod]
        public void MoveMcPkg_ShouldRenameOnAffectedTags()
        {
            // Arrange & Assert 
            Assert.AreEqual(McPkgNo, _tag1.McPkgNo);
            Assert.AreEqual(CommPkgNo, _tag1.CommPkgNo);
            Assert.AreEqual(McPkgNo2, _tag2.McPkgNo);
            Assert.AreEqual(CommPkgNo2, _tag2.CommPkgNo);
            var toCommPkg = "New name";

            // Act
            _dut.MoveMcPkg(McPkgNo, CommPkgNo, toCommPkg);

            // Assert
            Assert.AreEqual(McPkgNo, _tag1.McPkgNo);
            Assert.AreEqual(toCommPkg, _tag1.CommPkgNo);
            Assert.AreEqual(McPkgNo2, _tag2.McPkgNo);
            Assert.AreEqual(CommPkgNo2, _tag2.CommPkgNo);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MoveMcPkg_ShouldFailIfEmptyParameter()
        {
            // Act
            _dut.MoveMcPkg(" ", CommPkgNo, "ToCommPkg");
            _dut.MoveMcPkg(McPkgNo, " ", "ToCommPkg");
            _dut.MoveMcPkg(McPkgNo, CommPkgNo, " ");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddTag_ShouldFailIfAlreadyExists()
        {
            // Arrange
            var aNewTag = new Tag(TestPlant, TagType.Standard, Guid.NewGuid(), "Tag1", "", _stepMock.Object,
                new List<TagRequirement> {_reqMock.Object});

            // Act
            _dut.AddTag(aNewTag);
        }

        [TestMethod]
        public void AddTag_ShouldAddProjectTagAddedEvent()
        {
            // Arrange
            var eventTypes = _dut.DomainEvents.Select(e => e.GetType()).ToList();

            // Assert
            CollectionAssert.Contains(eventTypes, typeof(ChildEntityAddedEvent<Project, Tag>));
        }
    }
}

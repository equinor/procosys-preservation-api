using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.TagAggregate
{
    [TestClass]
    public class TagTests
    {
        private Mock<Step> _stepMock;
        private Mock<Requirement> _requirementMock;
        // ReSharper disable once CollectionNeverUpdated.Local
        readonly List<Requirement> _emptyRequirements = new List<Requirement>();
        readonly List<Requirement> _requirements = new List<Requirement>();

        [TestInitialize]
        public void Setup()
        {
            _stepMock = new Mock<Step>();
            _stepMock.SetupGet(x => x.Id).Returns(3);
            _requirementMock = new Mock<Requirement>();
            _requirements.Add(_requirementMock.Object);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var tag = new Tag("SchemaA", "TagNoA", "ProjectNoA", _stepMock.Object, _requirements);

            Assert.AreEqual("SchemaA", tag.Schema);
            Assert.AreEqual("TagNoA", tag.TagNo);
            Assert.AreEqual("ProjectNoA", tag.ProjectNo);
            Assert.AreEqual(_stepMock.Object.Id, tag.StepId);
            Assert.AreEqual(1, tag.Requirements.Count);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenStepNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(()
                => new Tag("", "", "", null, _requirements));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementsNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(()
                => new Tag("", "", "", _stepMock.Object, null));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenEmptyListOfRequirementsNotGiven()
            => Assert.ThrowsException<Exception>(()
                => new Tag("", "", "", _stepMock.Object, _emptyRequirements));

        [TestMethod]
        public void SetStep_ShouldSetStepId()
        {
            var tag = new Tag("", "", "", _stepMock.Object, _requirements);

            var newStep = new Mock<Step>();
            newStep.SetupGet(x => x.Id).Returns(4);
            tag.SetStep(newStep.Object);

            Assert.AreEqual(newStep.Object.Id, tag.StepId);
        }

        [TestMethod]
        public void SetStep_ShouldThrowException_WhenStepNotGiven()
        {
            var tag = new Tag("", "", "", _stepMock.Object, _requirements);

            Assert.ThrowsException<ArgumentNullException>(() => tag.SetStep(null));
        }

        [TestMethod]
        public void SetRequirement_ShouldThrowException_WhenRequirementNotGiven()
        {
            var tag = new Tag("", "", "", _stepMock.Object, _requirements);

            Assert.ThrowsException<ArgumentNullException>(() => tag.AddRequirement(null));
        }
    }
}

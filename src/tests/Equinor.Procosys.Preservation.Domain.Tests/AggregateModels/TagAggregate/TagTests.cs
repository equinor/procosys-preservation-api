using System;
using System.Collections.Generic;
using System.Linq;
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
        private DateTime _utcNow;

        [TestInitialize]
        public void Setup()
        {
            _stepMock = new Mock<Step>();
            _stepMock.SetupGet(x => x.Id).Returns(3);
            _requirementMock = new Mock<Requirement>();
            _requirementMock.SetupGet(x => x.Id).Returns(4);
            _requirements.Add(_requirementMock.Object);
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new Tag("SchemaA",
                "TagNoA",
                "ProjectNameA", 
                "AreaCodeA", 
                "CalloffA", 
                "DisciplineA", 
                "McPkgA", 
                "CommPkgA", 
                "PurchaseOrderA", 
                "TagFunctionCodeA", 
                _stepMock.Object,
                _requirements);

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual("TagNoA", dut.TagNo);
            Assert.AreEqual("ProjectNameA", dut.ProjectName);
            Assert.AreEqual("AreaCodeA", dut.AreaCode);
            Assert.AreEqual("CalloffA", dut.Calloff);
            Assert.AreEqual("DisciplineA", dut.DisciplineCode);
            Assert.AreEqual("McPkgA", dut.McPkgNo);
            Assert.AreEqual("PurchaseOrderA", dut.PurchaseOrderNo);
            Assert.AreEqual("TagFunctionCodeA", dut.TagFunctionCode);
            Assert.AreEqual(_stepMock.Object.Id, dut.StepId);
            Assert.AreEqual(1, dut.Requirements.Count);
            Assert.AreEqual(_requirementMock.Object.Id, dut.Requirements.First().Id);
            Assert.AreEqual(PreservationStatus.NotStarted, dut.Status);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenStepNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(()
                => new Tag("", "", "", "", "", "", "", "", "", "", null, _requirements));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementsNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(()
                => new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, null));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenEmptyListOfRequirementsGiven()
            => Assert.ThrowsException<Exception>(()
                => new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, _emptyRequirements));

        [TestMethod]
        public void SetStep_ShouldSetStepId()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, _requirements);

            var newStep = new Mock<Step>();
            newStep.SetupGet(x => x.Id).Returns(4);
            dut.SetStep(newStep.Object);

            Assert.AreEqual(newStep.Object.Id, dut.StepId);
        }

        [TestMethod]
        public void SetStep_ShouldThrowException_WhenStepNotGiven()
        {
            var tag = new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, _requirements);

            Assert.ThrowsException<ArgumentNullException>(() => tag.SetStep(null));
        }

        [TestMethod]
        public void AddRequirement_ShouldThrowException_WhenRequirementNotGiven()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, _requirements);

            Assert.ThrowsException<ArgumentNullException>(() => dut.AddRequirement(null));
        }

        [TestMethod]
        public void StartPreservation_ShouldSetStatusActive()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, _requirements);
            Assert.AreEqual(PreservationStatus.NotStarted, dut.Status);

            dut.StartPreservation(_utcNow);

            Assert.AreEqual(PreservationStatus.Active, dut.Status);
        }
    }
}

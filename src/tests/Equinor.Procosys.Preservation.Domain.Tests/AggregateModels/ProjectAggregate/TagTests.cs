using System;
using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.ProjectAggregate
{
    [TestClass]
    public class TagTests
    {
        private RequirementDefinition _reqDefNeedInput;
        private RequirementDefinition _reqDefNotNeedInput;
        private Mock<Step> _stepMock;
        private Requirement _reqNotNeedInput;
        private Requirement _reqNeedInput;
        // ReSharper disable once CollectionNeverUpdated.Local
        private readonly List<Requirement> _reqsNotNeedInput = new List<Requirement>();
        private readonly List<Requirement> _reqsNeedInput = new List<Requirement>();
        private DateTime _utcNow;

        [TestInitialize]
        public void Setup()
        {
            _stepMock = new Mock<Step>();
            _stepMock.SetupGet(x => x.Id).Returns(3);
            _reqDefNeedInput = new RequirementDefinition("", "", 1, 0);
            _reqDefNeedInput.AddField(new Field("", "", FieldType.CheckBox, 0));
            _reqDefNotNeedInput = new RequirementDefinition("", "", 1, 0);
            _reqDefNotNeedInput.AddField(new Field("", "", FieldType.Info, 0));
            _reqNotNeedInput = new Requirement("", 0, _reqDefNotNeedInput);
            _reqNeedInput = new Requirement("", 0, _reqDefNeedInput);
            _reqsNotNeedInput.Add(_reqNotNeedInput);
            _reqsNeedInput.Add(_reqNeedInput);
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new Tag("SchemaA",
                "TagNoA",
                "DescA", 
                "AreaCodeA", 
                "CalloffA", 
                "DisciplineA", 
                "McPkgA", 
                "CommPkgA", 
                "PurchaseOrderA", 
                "TagFunctionCodeA", 
                _stepMock.Object,
                _reqsNotNeedInput);

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual("TagNoA", dut.TagNo);
            Assert.AreEqual("DescA", dut.Description);
            Assert.AreEqual("AreaCodeA", dut.AreaCode);
            Assert.AreEqual("CalloffA", dut.Calloff);
            Assert.AreEqual("DisciplineA", dut.DisciplineCode);
            Assert.AreEqual("McPkgA", dut.McPkgNo);
            Assert.AreEqual("PurchaseOrderA", dut.PurchaseOrderNo);
            Assert.AreEqual("TagFunctionCodeA", dut.TagFunctionCode);
            Assert.AreEqual(_stepMock.Object.Id, dut.StepId);
            Assert.AreEqual(1, dut.Requirements.Count);
            Assert.AreEqual(PreservationStatus.NotStarted, dut.Status);
        }

        [TestMethod]
        public void Constructor_ShouldNotMakeTagReadyToBePreserved()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, _reqsNotNeedInput);

            Assert.IsFalse(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenStepNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(()
                => new Tag("", "", "", "", "", "", "", "", "", "", null, _reqsNotNeedInput));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenRequirementsNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(()
                => new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, null));

        [TestMethod]
        public void Constructor_ShouldThrowException_WhenEmptyListOfRequirementsGiven()
            => Assert.ThrowsException<Exception>(()
                => new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, new List<Requirement>()));

        [TestMethod]
        public void SetStep_ShouldSetStepId()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, _reqsNotNeedInput);

            var newStep = new Mock<Step>();
            newStep.SetupGet(x => x.Id).Returns(4);
            dut.SetStep(newStep.Object);

            Assert.AreEqual(newStep.Object.Id, dut.StepId);
        }

        [TestMethod]
        public void SetStep_ShouldThrowException_WhenStepNotGiven()
        {
            var tag = new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, _reqsNotNeedInput);

            Assert.ThrowsException<ArgumentNullException>(() => tag.SetStep(null));
        }

        [TestMethod]
        public void AddRequirement_ShouldThrowException_WhenRequirementNotGiven()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, _reqsNotNeedInput);

            Assert.ThrowsException<ArgumentNullException>(() => dut.AddRequirement(null));
        }

        [TestMethod]
        public void StartPreservation_ShouldSetStatusActive()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, _reqsNotNeedInput);
            Assert.AreEqual(PreservationStatus.NotStarted, dut.Status);

            dut.StartPreservation(_utcNow);

            Assert.AreEqual(PreservationStatus.Active, dut.Status);
        }

        [TestMethod]
        public void StartPreservation_ShouldMakeTagReadyToBePreserved_WhenNoRequirementNeedInput()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, _reqsNotNeedInput);
            Assert.AreEqual(PreservationStatus.NotStarted, dut.Status);

            dut.StartPreservation(_utcNow);

            Assert.IsTrue(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void StartPreservation_ShouldNotMakeTagReadyToBePreserved_WhenRequirementNeedInput()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, _reqsNeedInput);
            Assert.AreEqual(PreservationStatus.NotStarted, dut.Status);

            dut.StartPreservation(_utcNow);

            Assert.IsFalse(dut.ReadyToBePreserved);
        }

        [TestMethod]
        public void Preserve_ShouldThrowException_WhenNotReadyToBePreserved()
        {
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", _stepMock.Object, _reqsNeedInput);
            dut.StartPreservation(_utcNow);
            Assert.IsFalse(dut.ReadyToBePreserved);

            Assert.ThrowsException<Exception>(() => dut.Preserve(_utcNow, new Mock<Person>().Object, false));
        }
    }
}

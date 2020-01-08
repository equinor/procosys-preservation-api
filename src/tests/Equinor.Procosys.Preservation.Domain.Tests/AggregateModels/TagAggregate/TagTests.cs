using System;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.TagAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.TagAggregate
{
    [TestClass]
    public class TagTests
    {
        [TestMethod]
        public void ConstructorSetsPropertiesTest()
        {
            var step = new Mock<Step>();
            step.SetupGet(x => x.Id).Returns(3);
            var dut = new Tag("SchemaA", "TagNumberA", "ProjectNumberA", "DescriptionA", "AreaCodeA", "CalloffA", "DisciplineA", "McPkgA", "PurchaseOrderA", "TagFunctionCodeA", step.Object);

            Assert.AreEqual("SchemaA", dut.Schema);
            Assert.AreEqual("TagNumberA", dut.TagNumber);
            Assert.AreEqual("ProjectNumberA", dut.ProjectNumber);
            Assert.AreEqual("DescriptionA", dut.Description);
            Assert.AreEqual("AreaCodeA", dut.AreaCode);
            Assert.AreEqual("CalloffA", dut.CalloffNumber);
            Assert.AreEqual("DisciplineA", dut.DisciplineCode);
            Assert.AreEqual("McPkgA", dut.McPkcNumber);
            Assert.AreEqual("PurchaseOrderA", dut.PurchaseOrderNumber);
            Assert.AreEqual("TagFunctionCodeA", dut.TagFunctionCode);
            Assert.AreEqual(step.Object.Id, dut.StepId);
        }

        [TestMethod]
        public void ConstructorThrowsExceptionIsStepIsNotSetTest() => Assert.ThrowsException<ArgumentNullException>(() => new Tag("", "", "", "", "", "", "", "", "", "", null));

        [TestMethod]
        public void SetStepSetsStepIdTest()
        {
            var step = new Mock<Step>();
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", step.Object);

            var newStep = new Mock<Step>();
            newStep.SetupGet(x => x.Id).Returns(3);
            dut.SetStep(newStep.Object);

            Assert.AreEqual(newStep.Object.Id, dut.StepId);
        }

        [TestMethod]
        public void SetStepThrowsExceptionIfStepIsNullTest()
        {
            var step = new Mock<Step>();
            var dut = new Tag("", "", "", "", "", "", "", "", "", "", step.Object);

            Assert.ThrowsException<ArgumentNullException>(() => dut.SetStep(null));
        }
    }
}

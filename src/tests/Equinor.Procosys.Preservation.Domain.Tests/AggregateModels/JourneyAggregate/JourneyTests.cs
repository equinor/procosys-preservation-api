﻿using System;
using System.Linq;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.Procosys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests.AggregateModels.JourneyAggregate
{
    [TestClass]
    public class JourneyTests
    {
        private Journey _dutWithNoSteps;
        private Journey _dutWith3Steps;
        private Step _stepA;
        private Step _stepB;
        private Step _stepC;
        private int _stepAId;
        private int _stepBId;
        private int _stepCId;
        private const string TestPlant = "PlantA";

        [TestInitialize]
        public void Setup()
        {
            _dutWithNoSteps = new Journey(TestPlant, "TitleA");
            _dutWith3Steps = new Journey(TestPlant, "TitleB");
            _stepAId = 10033;
            _stepBId = 3;
            _stepCId = 967;

            var m = new Mode(TestPlant, "M", false);
            var r = new Responsible(TestPlant, "RC", "RD");
            
            _stepA = new Step(TestPlant, "SA", m, r);
            _stepA.SetProtectedIdForTesting(_stepAId);
            _stepB = new Step(TestPlant, "SB", m, r);
            _stepB.SetProtectedIdForTesting(_stepBId);
            _stepC = new Step(TestPlant, "SC", m, r);
            _stepC.SetProtectedIdForTesting(_stepCId);
            
            _dutWith3Steps.AddStep(_stepA);
            _dutWith3Steps.AddStep(_stepB);
            _dutWith3Steps.AddStep(_stepC);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual(TestPlant, _dutWithNoSteps.Plant);
            Assert.AreEqual("TitleA", _dutWithNoSteps.Title);
            Assert.IsFalse(_dutWithNoSteps.IsVoided);
            Assert.AreEqual(0, _dutWithNoSteps.Steps.Count);
        }
        
        [TestMethod]
        public void Constructor_ShouldThrowException_WhenTitleNotGiven() =>
            Assert.ThrowsException<ArgumentNullException>(() =>
                new Journey(TestPlant, null)
            );

        [TestMethod]
        public void AddStep_ShouldThrowException_WhenStepNotGiven()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _dutWithNoSteps.AddStep(null));
            Assert.AreEqual(0, _dutWithNoSteps.Steps.Count);
        }

        [TestMethod]
        public void AddStep_ShouldAddStepToStepsList()
        {
            var step = new Mock<Step>();
            step.SetupGet(s => s.Plant).Returns(TestPlant);

            _dutWithNoSteps.AddStep(step.Object);

            Assert.AreEqual(1, _dutWithNoSteps.Steps.Count);
            Assert.IsTrue(_dutWithNoSteps.Steps.Contains(step.Object));
        }

        [TestMethod]
        public void AddStep_ShouldSetIncreasedSortKey()
        {
            var step1 = new Mock<Step>();
            step1.SetupGet(s => s.Plant).Returns(TestPlant);
            var step2 = new Mock<Step>();
            step2.SetupGet(s => s.Plant).Returns(TestPlant);

            _dutWithNoSteps.AddStep(step1.Object);
            _dutWithNoSteps.AddStep(step2.Object);

            Assert.AreEqual(1, step1.Object.SortKey);
            Assert.AreEqual(2, step2.Object.SortKey);
        }

        [TestMethod]
        public void VoidUnVoid_ShouldToggleIsVoided()
        {
            Assert.IsFalse(_dutWithNoSteps.IsVoided);

            _dutWithNoSteps.Void();
            Assert.IsTrue(_dutWithNoSteps.IsVoided);

            _dutWithNoSteps.UnVoid();
            Assert.IsFalse(_dutWithNoSteps.IsVoided);
        }

        [TestMethod]
        public void GetNextStep_ShouldReturnStepWithNextSortKey()
        {
            Assert.AreEqual(_stepB, _dutWith3Steps.GetNextStep(_stepAId));
            Assert.AreEqual(_stepC, _dutWith3Steps.GetNextStep(_stepBId));
            Assert.IsNull(_dutWith3Steps.GetNextStep(_stepCId));
        }

        [TestMethod]
        public void OrderedSteps_ShouldReturnStepOrderedBySortKey()
        {
            var steps = _dutWith3Steps.OrderedSteps().ToList();

            Assert.AreEqual(_stepA, steps.ElementAt(0));
            Assert.AreEqual(_stepB, steps.ElementAt(1));
            Assert.AreEqual(_stepC, steps.ElementAt(2));
        }

        [TestMethod]
        public void AreAdjacent_ShouldReturnFalse_WhenStepAreNotAdjacent()
        {
            Assert.IsFalse(_dutWith3Steps.AreAdjacentSteps(_stepAId, _stepCId));
        }

        [TestMethod]
        public void AreAdjacent_ShouldReturnTrue_WhenStepAreAdjacent()
        {
            Assert.IsTrue(_dutWith3Steps.AreAdjacentSteps(_stepAId, _stepBId));
            Assert.IsTrue(_dutWith3Steps.AreAdjacentSteps(_stepBId, _stepCId));
        }

        [TestMethod]
        public void SwapSteps_ShouldThrowException_WhenStepAreNotAdjacent()
            => Assert.ThrowsException<Exception>(() => _dutWith3Steps.SwapSteps(_stepAId, _stepCId));

        [TestMethod]
        public void SwapSteps_ShouldSwapOrderingOfAdjacentSteps()
        {
            _dutWith3Steps.SwapSteps(_stepAId, _stepBId);

            var steps = _dutWith3Steps.OrderedSteps().ToList();

            Assert.AreEqual(_stepB, steps.ElementAt(0));
            Assert.AreEqual(_stepA, steps.ElementAt(1));
            Assert.AreEqual(_stepC, steps.ElementAt(2));
            
            _dutWith3Steps.SwapSteps(_stepCId, _stepAId);

            steps = _dutWith3Steps.OrderedSteps().ToList();

            Assert.AreEqual(_stepB, steps.ElementAt(0));
            Assert.AreEqual(_stepC, steps.ElementAt(1));
            Assert.AreEqual(_stepA, steps.ElementAt(2));
        }
    }
}

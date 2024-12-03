using System;
using System.Linq;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.Test.Common;
using Equinor.ProCoSys.Preservation.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.JourneyAggregate
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
        private Mode _mode;
        private Responsible _responsible;
        private const string TestPlant = "PlantA";

        [TestInitialize]
        public void Setup()
        {
            var utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            var timeProvider = new ManualTimeProvider(utcNow);
            TimeService.SetProvider(timeProvider);
            
            _dutWithNoSteps = new Journey(TestPlant, "TitleA");
            _dutWith3Steps = new Journey(TestPlant, "TitleB");
            _stepAId = 10033;
            _stepBId = 3;
            _stepCId = 967;

            _mode = new Mode(TestPlant, "M", false);
            _responsible = new Responsible(TestPlant, "RC", "RD");
            
            _stepA = new Step(TestPlant, "SA", _mode, _responsible);
            _stepA.SetProtectedIdForTesting(_stepAId);
            _stepB = new Step(TestPlant, "SB", _mode, _responsible);
            _stepB.SetProtectedIdForTesting(_stepBId);
            _stepC = new Step(TestPlant, "SC", _mode, _responsible);
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
            => Assert.ThrowsException<ArgumentNullException>(() => _dutWithNoSteps.AddStep(null));

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
        public void RemoveStep_ShouldRemoveStepFromStepsList()
        {
            // Arrange
            Assert.AreEqual(3, _dutWith3Steps.Steps.Count);
            _stepA.IsVoided = true;

            // Act
            _dutWith3Steps.RemoveStep(_stepA);

            // Assert
            Assert.AreEqual(2, _dutWith3Steps.Steps.Count);
            Assert.IsFalse(_dutWith3Steps.Steps.Contains(_stepA));
        }

        [TestMethod]
        public void RemoveStep_ShouldThrowException_WhenStepNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dutWithNoSteps.RemoveStep(null));

        [TestMethod]
        public void RemoveStep_ShouldThrowExceptionWhenStepIsNotVoided()
            => Assert.ThrowsException<Exception>(() => _dutWithNoSteps.RemoveStep(_stepA));

        [TestMethod]
        public void AddStep_ShouldSetNextAvailableSortKey()
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
        public void AddStep_ShouldSetNextAvailableSortKey_AfterDeletingSteps()
        {
            // Arrange
            var stepA = _dutWith3Steps.Steps.ElementAt(0);
            Assert.AreEqual(1, stepA.SortKey);
            var stepB = _dutWith3Steps.Steps.ElementAt(1);
            Assert.AreEqual(2, stepB.SortKey);
            var stepC = _dutWith3Steps.Steps.ElementAt(2);
            Assert.AreEqual(3, stepC.SortKey);

            stepA.IsVoided = true;
            _dutWith3Steps.RemoveStep(stepA);

            var stepD = new Step(TestPlant, "SD", _mode, _responsible);

            // Act
            _dutWith3Steps.AddStep(stepD);

            // Arrange
            Assert.AreEqual(4, stepD.SortKey);
        }
        
        [TestMethod]
        public void AddStep_ShouldAddChildAddedEvent()
        {
            _dutWithNoSteps.AddStep(_stepA);
            
            var eventTypes = _dutWithNoSteps.DomainEvents.Select(e => e.GetType()).ToList();
            CollectionAssert.Contains(eventTypes, typeof(ChildAddedEvent<Journey, Step>));
        }

        [TestMethod]
        public void GetNextStep_ShouldReturnedWithNextSortKey()
        {
            Assert.AreEqual(_stepB, _dutWith3Steps.GetNextStep(_stepAId));
            Assert.AreEqual(_stepC, _dutWith3Steps.GetNextStep(_stepBId));
            Assert.IsNull(_dutWith3Steps.GetNextStep(_stepCId));
        }

        [TestMethod]
        public void HasNextStep_ShouldReturnedTrue_WhenNextStepExists()
        {
            Assert.IsTrue(_dutWith3Steps.HasNextStep(_stepAId));
            Assert.IsTrue(_dutWith3Steps.HasNextStep(_stepBId));
            Assert.IsFalse(_dutWith3Steps.HasNextStep(_stepCId));
        }

        [TestMethod]
        public void OrderedSteps_ShouldReturnedOrderedBySortKey()
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
        
        [TestMethod]
        public void SwapSteps_ShouldAddChildModifiedEvents()
        {
            _dutWith3Steps.SwapSteps(_stepAId, _stepBId);
            
            var eventTypes = _dutWith3Steps.DomainEvents.Select(e => e.GetType()).ToList();
            var childModifiedEvent = eventTypes.FindAll(e => e == typeof(ChildModifiedEvent<Journey, Step>));
            Assert.AreEqual(2, childModifiedEvent.Count);
        }

        [TestMethod]
        public void VoidStep_ShouldVoidStep()
        {
            var stepToVoid = _dutWith3Steps.Steps.First();
            Assert.IsFalse(stepToVoid.IsVoided);

            _dutWith3Steps.VoidStep(stepToVoid.Id, "AAAAAAAAABA=");

            Assert.IsTrue(stepToVoid.IsVoided);
        }

        [TestMethod]
        public void VoidStep_ShouldReturnVoidedStep()
        {
            var stepToVoid = _dutWith3Steps.Steps.First();

            var step = _dutWith3Steps.VoidStep(stepToVoid.Id, "AAAAAAAAABA=");

            Assert.AreEqual(step, stepToVoid);
        }
        
        [TestMethod]
        public void VoidStep_ShouldAddChildModifiedEvent()
        {
            var stepToVoid = _dutWith3Steps.Steps.First();

            _dutWith3Steps.VoidStep(stepToVoid.Id, "AAAAAAAAABA=");
            
            var eventTypes = _dutWith3Steps.DomainEvents.Select(e => e.GetType()).ToList();
            CollectionAssert.Contains(eventTypes, typeof(ChildModifiedEvent<Journey, Step>));
        }

        [TestMethod]
        public void UnvoidStep_ShouldUnvoidStep()
        {
            var stepToUnvoid = _dutWith3Steps.Steps.First();
            stepToUnvoid.IsVoided = true;

            Assert.IsTrue(stepToUnvoid.IsVoided);

            _dutWith3Steps.UnvoidStep(stepToUnvoid.Id, "AAAAAAAAABA=");

            Assert.IsFalse(stepToUnvoid.IsVoided);
        }

        [TestMethod]
        public void UnvoidStep_ShouldReturnUnvoidedStep()
        {
            var stepToUnvoid = _dutWith3Steps.Steps.First();
            stepToUnvoid.IsVoided = true;

            var step = _dutWith3Steps.UnvoidStep(stepToUnvoid.Id, "AAAAAAAAABA=");

            Assert.AreEqual(step, stepToUnvoid);
        }
        
        [TestMethod]
        public void UnvoidStep_ShouldAddChildModifiedEvent()
        {
            var stepToUnvoid = _dutWith3Steps.Steps.First();

            _dutWith3Steps.UnvoidStep(stepToUnvoid.Id, "AAAAAAAAABA=");
            
            var eventTypes = _dutWith3Steps.DomainEvents.Select(e => e.GetType()).ToList();
            CollectionAssert.Contains(eventTypes, typeof(ChildModifiedEvent<Journey, Step>));
        }
        
        [TestMethod]
        public void SetCreated_ShouldAddPlantEntityCreatedEvent()
        {
            // Arrange
            var person = new Person(Guid.Empty, "Espen", "Askeladd");
            
            // Act
            _dutWithNoSteps.SetCreated(person);
            var eventTypes = _dutWithNoSteps.DomainEvents.Select(e => e.GetType()).ToList();

            // Assert
            CollectionAssert.Contains(eventTypes, typeof(CreatedEvent<Journey>));
        }
        
        [TestMethod]
        public void SetModified_ShouldAddPlantEntityModifiedEvent()
        {
            // Arrange
            var person = new Person(Guid.Empty, "Espen", "Askeladd");
            
            // Act
            _dutWithNoSteps.SetModified(person);
            var eventTypes = _dutWithNoSteps.DomainEvents.Select(e => e.GetType()).ToList();

            // Assert
            CollectionAssert.Contains(eventTypes, typeof(ModifiedEvent<Journey>));
        }
        
        [TestMethod]
        public void SetRemoved_ShouldAddPlantEntityDeletedEvent()
        {
            // Act
            _dutWithNoSteps.SetRemoved();
            var eventTypes = _dutWithNoSteps.DomainEvents.Select(e => e.GetType()).ToList();

            // Assert
            CollectionAssert.Contains(eventTypes, typeof(DeletedEvent<Journey>));
        }
    }
}

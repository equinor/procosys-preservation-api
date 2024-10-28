using System;
using System.Linq;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.RequirementTypeAggregate
{
    [TestClass]
    public class RequirementTypeTests
    {
        private const string TestPlant = "PlantA";
        private RequirementType _dut;
        private RequirementDefinition _rd;

        [TestInitialize]
        public void Setup()
        {
            var utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            var timeProvider = new ManualTimeProvider(utcNow);
            TimeService.SetProvider(timeProvider);
            
            _dut = new RequirementType(TestPlant, "CodeA", "TitleA", RequirementTypeIcon.Other, 10);
            _rd = new RequirementDefinition(TestPlant, "RD1", 4, RequirementUsage.ForAll, 0);
            _dut.AddRequirementDefinition(_rd);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            Assert.AreEqual(TestPlant, _dut.Plant);
            Assert.AreEqual("CodeA", _dut.Code);
            Assert.AreEqual("TitleA", _dut.Title);
            Assert.AreEqual(RequirementTypeIcon.Other, _dut.Icon);
            Assert.AreEqual(10, _dut.SortKey);
            Assert.IsFalse(_dut.IsVoided);
            Assert.AreEqual(1, _dut.RequirementDefinitions.Count);
        }

        [TestMethod]
        public void AddRequirementDefinition_ShouldThrowExceptionTest_WhenRequirementDefinitionNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dut.AddRequirementDefinition(null));

        [TestMethod]
        public void AddRequirementDefinition_ShouldAddRequirementDefinitionToRequirementDefinitionsList()
        {
            var rd = new RequirementDefinition(TestPlant, "RD2", 4, RequirementUsage.ForAll, 0);

            _dut.AddRequirementDefinition(rd);

            Assert.AreEqual(2, _dut.RequirementDefinitions.Count);
            Assert.IsTrue(_dut.RequirementDefinitions.Contains(rd));
        }
        
        [TestMethod]
        public void AddRequirementDefinition_ShouldAddRequirementTypeRequirementDefinitionAddedEvent()
        {
            var rd = new RequirementDefinition(TestPlant, "RD2", 4, RequirementUsage.ForAll, 0);

            _dut.AddRequirementDefinition(rd);

            var eventTypes = _dut.DomainEvents.Select(e => e.GetType()).ToList();
            CollectionAssert.Contains(eventTypes, typeof(EntityAddedChildEntityEvent<RequirementType, RequirementDefinition>));
        }

        [TestMethod]
        public void RequirementDefinitions_ShouldThrowExceptionTest_WhenRequirementDefinitionNotGiven()
            => Assert.ThrowsException<ArgumentNullException>(() => _dut.RemoveRequirementDefinition(null));

        [TestMethod]
        public void RequirementDefinitions_ShouldThrowExceptionTest_WhenRequirementDefinitionIsNotVoided()
            => Assert.ThrowsException<Exception>(() => _dut.RemoveRequirementDefinition(_rd));

        [TestMethod]
        public void RemoveRequirementDefinition_ShouldRemoveRequirementDefinitionFromRequirementDefinitionsList()
        {
            _rd.IsVoided = true;
            _dut.RemoveRequirementDefinition(_rd);

            Assert.AreEqual(0, _dut.RequirementDefinitions.Count);
        }
        
        [TestMethod]
        public void RemoveRequirementDefinition_ShouldAddPlantEntityDeletedEvent()
        {
            _rd.IsVoided = true;
            _dut.RemoveRequirementDefinition(_rd);
            
            var eventTypes = _dut.DomainEvents.Select(e => e.GetType()).ToList();

            CollectionAssert.Contains(eventTypes, typeof(PlantEntityDeletedEvent<RequirementDefinition>));
        }
        
        [TestMethod]
        public void SetCreated_ShouldAddPlantEntityCreatedEvent()
        {
            var person = new Person(Guid.Empty, "Espen", "Askeladd");
            
            _dut.SetCreated(person);
            var eventTypes = _dut.DomainEvents.Select(e => e.GetType()).ToList();

            // Assert
            CollectionAssert.Contains(eventTypes, typeof(PlantEntityCreatedEvent<RequirementType>));
        }
        
        [TestMethod]
        public void SetModified_ShouldAddPlantEntityModifiedEvent()
        {
            var person = new Person(Guid.Empty, "Espen", "Askeladd");
            
            _dut.SetModified(person);
            var eventTypes = _dut.DomainEvents.Select(e => e.GetType()).ToList();

            // Assert
            CollectionAssert.Contains(eventTypes, typeof(PlantEntityModifiedEvent<RequirementType>));
        }
    }
}

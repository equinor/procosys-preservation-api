using System;
using System.Linq;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.Preservation.Domain.Tests.AggregateModels.ModeAggregate
{
    [TestClass]
    public class ModeTests
    {
        [TestInitialize]
        public void Setup()
        {
            var utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            var timeProvider = new ManualTimeProvider(utcNow);
            TimeService.SetProvider(timeProvider);
        }

        [TestMethod]
        public void Constructor_ShouldSetProperties()
        {
            var dut = new Mode("PlantA", "TitleA", false);

            Assert.AreEqual("PlantA", dut.Plant);
            Assert.AreEqual("TitleA", dut.Title);
            Assert.AreEqual(false, dut.ForSupplier);
        }
        
        [TestMethod]
        public void SetCreated_ShouldAddPlantEntityCreatedEvent()
        {
            var dut = new Mode("PlantA", "TitleA", false);
            var person = new Person(Guid.Empty, "Espen", "Askeladd");
            
            // Act
            dut.SetCreated(person);
            var eventTypes = dut.DomainEvents.Select(e => e.GetType()).ToList();

            // Assert
            CollectionAssert.Contains(eventTypes, typeof(CreatedEvent<Mode>));
        }
        
        [TestMethod]
        public void SetModified_ShouldAddPlantEntityModifiedEvent()
        {
            var dut = new Mode("PlantA", "TitleA", false);
            var person = new Person(Guid.Empty, "Espen", "Askeladd");
            
            // Act
            dut.SetModified(person);
            var eventTypes = dut.DomainEvents.Select(e => e.GetType()).ToList();

            // Assert
            CollectionAssert.Contains(eventTypes, typeof(ModifiedEvent<Mode>));
        }
        
        [TestMethod]
        public void SetRemoved_ShouldAddPlantEntityDeletedEvent()
        {
            var dut = new Mode("PlantA", "TitleA", false);
            
            // Act
            dut.SetRemoved();
            var eventTypes = dut.DomainEvents.Select(e => e.GetType()).ToList();

            // Assert
            CollectionAssert.Contains(eventTypes, typeof(DeletedEvent<Mode>));
        }
    }
}

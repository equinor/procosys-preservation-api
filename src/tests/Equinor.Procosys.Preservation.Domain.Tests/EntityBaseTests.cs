using System;
using System.Linq;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests
{
    [TestClass]
    public class EntityBaseTests
    {
        [TestMethod]
        public void ReturningEmptyDomainEventsListTest()
        {
            var dut = new TestableEntityBase();
            Assert.IsNotNull(dut.DomainEvents);
        }

        [TestMethod]
        public void DomainEventIsAddedToListTest()
        {
            var dut = new TestableEntityBase();
            var domainEvent = new Mock<INotification>();
            dut.AddDomainEvent(domainEvent.Object);

            Assert.IsTrue(dut.DomainEvents.Contains(domainEvent.Object));
        }

        [TestMethod]
        public void DomainEventIsRemovedFromListTest()
        {
            var dut = new TestableEntityBase();
            var domainEvent = new Mock<INotification>();
            dut.AddDomainEvent(domainEvent.Object);
            dut.RemoveDomainEvent(domainEvent.Object);

            Assert.IsFalse(dut.DomainEvents.Contains(domainEvent.Object));
        }

        [TestMethod]
        public void DomainEventsAreClearedTest()
        {
            var dut = new TestableEntityBase();
            var domainEvent1 = new Mock<INotification>();
            dut.AddDomainEvent(domainEvent1.Object);
            var domainEvent2 = new Mock<INotification>();
            dut.AddDomainEvent(domainEvent2.Object);

            dut.ClearDomainEvents();

            Assert.AreEqual(0, dut.DomainEvents.Count);
        }

        public class TestableEntityBase : EntityBase
        {
            // The base class is abstract, therefor a sub class is needed to test it.
        }
    }
}

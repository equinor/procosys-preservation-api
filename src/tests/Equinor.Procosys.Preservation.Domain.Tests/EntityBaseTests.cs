using System.Linq;
using Equinor.Procosys.Preservation.Test.Common.ExtentionMethods;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Domain.Tests
{
    [TestClass]
    public class EntityBaseTests
    {
        private const ulong OldRowVersion = 123;
        private const ulong NewRowVersion = 456;

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

        [TestMethod]
        public void GetRowVersion_ShouldReturnLastSetRowVersion()
        {
            var dut = new TestableEntityBase();
            dut.SetProtectedRowVersionForTesting(123);
            dut.GetRowVersion().Equals(123);
        }    
        
        [TestMethod]
        public void SetRowVersion_ShouldSucceed()
        {
            var dut = new TestableEntityBase();
            dut.SetProtectedRowVersionForTesting(OldRowVersion);

            dut.SetRowVersion(NewRowVersion);
        }

        [TestMethod]
        public void SetRowVersion_ByReplacingByteArrayContent_ShouldSucceed()
        {
            unsafe
            {
                var dut = new TestableEntityBase();

                dut.SetProtectedRowVersionForTesting(OldRowVersion);
                var originalByteArrayPointer = dut.GetRowVersionPointer();

                dut.SetRowVersion(NewRowVersion);
                var newByteArrayPointer = dut.GetRowVersionPointer();

                Assert.IsTrue(originalByteArrayPointer == newByteArrayPointer, "Reference equals should be true as we replace the array content, not the entire byte array");
            }
        }

        [TestMethod]
        public void SetRowVersion_ByReplacingByteArray_ShouldFail()
        {
            unsafe
            {
                var dut = new TestableEntityBase();

                dut.SetProtectedRowVersionForTesting(OldRowVersion);
                var originalByteArray = dut.GetRowVersionPointer();

                dut.SetProtectedRowVersionForTesting(NewRowVersion);
                var newByteArray = dut.GetRowVersionPointer();

                Assert.IsFalse(originalByteArray == newByteArray, "Reference equals should be false as we replace the byte array");
            }
        }

        public class TestableEntityBase : EntityBase
        {
            // The base class is abstract, therefor a sub class is needed to test it.
        }
    }
}

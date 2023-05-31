using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers
{
    [TestClass]
    public class EventDispatcherTests
    {
        [TestMethod]
        public void Constructor_ThrowsException_IFMediatorIsNull_Test()
            => Assert.ThrowsException<ArgumentNullException>(() => new EventDispatcher(null));

        [TestMethod]
        public async Task DispatchDomainEventsAsync_SendsOutEvents_Test()
        {
            var mediator = new Mock<IMediator>();
            var dut = new EventDispatcher(mediator.Object);
            var entities = new List<TestableEntityBase>();

            for (var i = 0; i < 3; i++)
            {
                var entity = new Mock<TestableEntityBase>();
                entity.Object.AddDomainEvent(new TestableDomainEvent());
                entity.Object.AddPostSaveDomainEvent(new Mock<IPostSaveDomainEvent>().Object);
                entities.Add(entity.Object);
            }
            await dut.DispatchDomainEventsAsync(entities);

            mediator.Verify(x
                => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Exactly(3));

            entities.ForEach(e => Assert.AreEqual(0, e.DomainEvents.Count));
            entities.ForEach(e => Assert.AreEqual(1, e.PostSaveDomainEvents.Count));
        }

        [TestMethod]
        public async Task DispatchPostSaveAsync_SendsOutEvents_Test()
        {
            var mediator = new Mock<IMediator>();
            var dut = new EventDispatcher(mediator.Object);
            var entities = new List<TestableEntityBase>();

            for (var i = 0; i < 3; i++)
            {
                var entity = new Mock<TestableEntityBase>();
                entity.Object.AddDomainEvent(new TestableDomainEvent());
                entity.Object.AddPostSaveDomainEvent(new Mock<IPostSaveDomainEvent>().Object);
                entities.Add(entity.Object);
            }
            await dut.DispatchPostSaveEventsAsync(entities);

            mediator.Verify(x
                => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Exactly(3));

            entities.ForEach(e => Assert.AreEqual(1, e.DomainEvents.Count));
            entities.ForEach(e => Assert.AreEqual(0, e.PostSaveDomainEvents.Count));
        }

        [TestMethod]
        public async Task DispatchPreSaveAsync_ThrowsException_IfListIsEmptyAsync_Test()
        {
            var mediator = new Mock<IMediator>();
            var dut = new EventDispatcher(mediator.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                dut.DispatchDomainEventsAsync(null));
        }

        [TestMethod]
        public async Task DispatchPostSaveAsync_ThrowsException_IfListIsEmptyAsync_Test()
        {
            var mediator = new Mock<IMediator>();
            var dut = new EventDispatcher(mediator.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                dut.DispatchPostSaveEventsAsync(null));
        }
    }
}

public class TestableEntityBase : EntityBase
{
    // The base class is abstract, therefor a sub class is needed to test it.
}

public class TestableDomainEvent : DomainEvent
{
    public TestableDomainEvent() : base("Test")
    {
    }
}

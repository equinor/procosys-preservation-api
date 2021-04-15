using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.EventHandlers;
using Equinor.ProCoSys.Preservation.Domain;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.EventHandlers
{
    [TestClass]
    public class EventDispatcherTests
    {
        [TestMethod]
        public void Constructor_ThrowsException_IFMediatorIsNull_Test() => Assert.ThrowsException<ArgumentNullException>(() => new EventDispatcher(null));

        [TestMethod]
        public async Task DispatchAsync_SendsOutEvents_Test()
        {
            var mediator = new Mock<IMediator>();
            var dut = new EventDispatcher(mediator.Object);
            var entities = new List<TestableEntityBase>();
            for (var i = 0; i < 3; i++)
            {
                var entity = new Mock<TestableEntityBase>();
                entity.Object.AddDomainEvent(new Mock<INotification>().Object);
                entities.Add(entity.Object);
            }
            await dut.DispatchAsync(entities, default);

            mediator.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [TestMethod]
        public async Task DispatchAsync_ThrowsException_IfListIsEmptyAsync_Test()
        {
            var mediator = new Mock<IMediator>();
            var dut = new EventDispatcher(mediator.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                  dut.DispatchAsync(null));
        }

        public class TestableEntityBase : EntityBase
        {
            // The base class is abstract, therefor a sub class is needed to test it.
        }
    }
}

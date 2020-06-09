using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Equinor.Procosys.Preservation.Infrastructure.Tests
{
    public class ContextHelper
    {
        public ContextHelper()
        {
            DbOptions = new DbContextOptions<PreservationContext>();
            EventDispatcherMock = new Mock<IEventDispatcher>();
            PlantProviderMock = new Mock<IPlantProvider>();
            ContextMock = new Mock<PreservationContext>(DbOptions, PlantProviderMock.Object, EventDispatcherMock.Object);
        }

        public DbContextOptions<PreservationContext> DbOptions { get; }
        public Mock<IEventDispatcher> EventDispatcherMock { get; }
        public Mock<IPlantProvider> PlantProviderMock { get; }
        public Mock<PreservationContext> ContextMock { get; }
    }
}

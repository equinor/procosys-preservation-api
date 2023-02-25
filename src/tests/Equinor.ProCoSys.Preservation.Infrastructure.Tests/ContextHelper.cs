using Equinor.ProCoSys.Auth.Misc;
using Equinor.ProCoSys.Preservation.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Equinor.ProCoSys.Preservation.Infrastructure.Tests
{
    public class ContextHelper
    {
        public ContextHelper()
        {
            DbOptions = new DbContextOptions<PreservationContext>();
            EventDispatcherMock = new Mock<IEventDispatcher>();
            PlantProviderMock = new Mock<IPlantProvider>();
            CurrentUserProviderMock = new Mock<ICurrentUserProvider>();
            ContextMock = new Mock<PreservationContext>(DbOptions, PlantProviderMock.Object, EventDispatcherMock.Object, CurrentUserProviderMock.Object);
        }

        public DbContextOptions<PreservationContext> DbOptions { get; }
        public Mock<IEventDispatcher> EventDispatcherMock { get; }
        public Mock<IPlantProvider> PlantProviderMock { get; }
        public Mock<PreservationContext> ContextMock { get; }
        public Mock<ICurrentUserProvider> CurrentUserProviderMock { get; }
    }
}

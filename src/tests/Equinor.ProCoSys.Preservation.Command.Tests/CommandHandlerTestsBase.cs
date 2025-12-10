using System;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests
{
    [TestClass]
    public abstract class CommandHandlerTestsBase
    {
        protected readonly Guid CurrentUserOid = new Guid("12345678-1234-1234-1234-123456789123");
        protected Mock<ICurrentUserProvider> CurrentUserProviderMock;
        protected const string TestPlant = "TestPlant";
        protected Mock<IUnitOfWork> UnitOfWorkMock;
        protected Mock<IPlantProvider> PlantProviderMock;
        protected ManualTimeProvider TimeProvider;
        protected DateTime UtcNow;
        protected readonly Guid ProjectProCoSysGuid = new Guid("aec8297b-b010-4c5d-91e0-7b1c8664ced8");

        [TestInitialize]
        public void BaseSetup()
        {
            CurrentUserProviderMock = new Mock<ICurrentUserProvider>();
            CurrentUserProviderMock
                .Setup(x => x.GetCurrentUserOid())
                .Returns(CurrentUserOid);
            UnitOfWorkMock = new Mock<IUnitOfWork>();
            PlantProviderMock = new Mock<IPlantProvider>();
            PlantProviderMock
                .Setup(x => x.Plant)
                .Returns(TestPlant);
            UtcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            TimeProvider = new ManualTimeProvider(UtcNow);
            TimeService.SetProvider(TimeProvider);
        }
    }
}

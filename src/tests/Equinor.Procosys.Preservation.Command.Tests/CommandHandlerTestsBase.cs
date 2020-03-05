using System;
using Equinor.Procosys.Preservation.Domain;
using Equinor.Procosys.Preservation.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests
{
    [TestClass]
    public abstract class CommandHandlerTestsBase
    {
        protected const string TestPlant = "TestPlant";
        protected Mock<IUnitOfWork> UnitOfWorkMock;
        protected Mock<IPlantProvider> PlantProviderMock;
        protected ManualTimeProvider _timeProvider;
        protected DateTime _utcNow;

        [TestInitialize]
        public void BaseSetup()
        {
            UnitOfWorkMock = new Mock<IUnitOfWork>();
            PlantProviderMock = new Mock<IPlantProvider>();
            PlantProviderMock
                .Setup(x => x.Plant)
                .Returns(TestPlant);
            _utcNow = new DateTime(2020, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            _timeProvider = new ManualTimeProvider(_utcNow);
        }
    }
}

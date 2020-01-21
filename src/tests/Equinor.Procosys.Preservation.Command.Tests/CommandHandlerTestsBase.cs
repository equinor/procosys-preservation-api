using Equinor.Procosys.Preservation.Domain;
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

        [TestInitialize]
        public void BaseSetup()
        {
            UnitOfWorkMock = new Mock<IUnitOfWork>();
            PlantProviderMock = new Mock<IPlantProvider>();
            PlantProviderMock
                .Setup(x => x.Plant)
                .Returns(TestPlant);
        }
    }
}

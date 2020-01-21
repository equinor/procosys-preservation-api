using Equinor.Procosys.Preservation.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Command.Tests
{
    [TestClass]
    public class CommandHandlerTestsBase
    {
        protected const string TestPlant = "TestPlant";
        protected Mock<IUnitOfWork> _unitOfWorkMock;
        protected Mock<IPlantProvider> _plantProviderMock;

        [TestInitialize]
        public void BaseSetup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _plantProviderMock = new Mock<IPlantProvider>();
            _plantProviderMock
                .Setup(x => x.Plant)
                .Returns(TestPlant);
        }
    }
}

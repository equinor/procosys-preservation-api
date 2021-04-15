using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.TagFunctionCommands.UnvoidTagFunction;
using Equinor.ProCoSys.Preservation.Domain;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagFunctionCommands.UnvoidTagFunction
{
    [TestClass]
    public class UnvoidTagFunctionCommandHandlerTests : CommandHandlerTestsBase
    {
        private TagFunction _tagFunction;
        private UnvoidTagFunctionCommand _command;
        private UnvoidTagFunctionCommandHandler _dut;
        private readonly string _rowVersion = "AAAAAAAAABA=";

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var tagFunctionRepositoryMock = new Mock<ITagFunctionRepository>();
            const string TagFunctionCode = "TFC";
            const string RegisterCode = "RC";

            _tagFunction = new TagFunction(TestPlant, "ReqDefinitionTitle", "TagFunctionDescription", "MAIN_EQUIP");
            _tagFunction.IsVoided = true;

            tagFunctionRepositoryMock.Setup(r => r.GetByCodesAsync(TagFunctionCode, RegisterCode))
                .Returns(Task.FromResult(_tagFunction));

            _command = new UnvoidTagFunctionCommand(TagFunctionCode, RegisterCode, _rowVersion);
            _dut = new UnvoidTagFunctionCommandHandler(
                tagFunctionRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingUnvoidTagFunctionCommand_ShouldUnvoidTagFunction()
        {
            // Arrange
            Assert.IsTrue(_tagFunction.IsVoided);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsFalse(_tagFunction.IsVoided);
        }

        [TestMethod]
        public async Task HandlingUnvoidTagFunctionCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _tagFunction.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingUnvoidTagFunctionCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}

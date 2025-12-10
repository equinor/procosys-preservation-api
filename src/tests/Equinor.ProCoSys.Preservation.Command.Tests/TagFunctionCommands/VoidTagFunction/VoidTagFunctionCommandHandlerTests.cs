using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Preservation.Command.TagFunctionCommands.VoidTagFunction;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.TagFunctionCommands.VoidTagFunction
{
    [TestClass]
    public class VoidTagFunctionCommandHandlerTests : CommandHandlerTestsBase
    {
        private TagFunction _tagFunction;
        private VoidTagFunctionCommand _command;
        private VoidTagFunctionCommandHandler _dut;
        private readonly string _rowVersion = "AAAAAAAAABA=";

        [TestInitialize]
        public void Setup()
        {
            // Arrange
            var tagFunctionRepositoryMock = new Mock<ITagFunctionRepository>();
            const string TagFunctionCode = "TFC";
            const string RegisterCode = "RC";

            _tagFunction = new TagFunction(TestPlant, "ReqDefinitionTitle", "TagFunctionDescription", "MAIN_EQUIP");

            tagFunctionRepositoryMock.Setup(r => r.GetByCodesAsync(TagFunctionCode, RegisterCode))
                .Returns(Task.FromResult(_tagFunction));

            _command = new VoidTagFunctionCommand(TagFunctionCode, RegisterCode, _rowVersion);
            _dut = new VoidTagFunctionCommandHandler(
                tagFunctionRepositoryMock.Object,
                UnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task HandlingVoidTagFunctionCommand_ShouldVoidTagFunction()
        {
            // Arrange
            Assert.IsFalse(_tagFunction.IsVoided);

            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsTrue(_tagFunction.IsVoided);
        }

        [TestMethod]
        public async Task HandlingVoidTagFunctionCommand_ShouldSetAndReturnRowVersion()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(_rowVersion, result.Data);
            Assert.AreEqual(_rowVersion, _tagFunction.RowVersion.ConvertToString());
        }

        [TestMethod]
        public async Task HandlingVoidTagFunctionCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);

            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}

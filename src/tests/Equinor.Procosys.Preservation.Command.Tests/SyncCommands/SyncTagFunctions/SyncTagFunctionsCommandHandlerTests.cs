using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.Preservation.Command.SyncCommands.SyncTagFunctions;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.TagFunctionAggregate;
using Equinor.ProCoSys.Preservation.MainApi.TagFunction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.Preservation.Command.Tests.SyncCommands.SyncTagFunctions
{
    [TestClass]
    public class SyncTagFunctionsCommandHandlerTests : CommandHandlerTestsBase
    {
        private const string TagFunctionCode1 = "TagFunction1";
        private const string TagFunctionCode2 = "TagFunction2";
        private const string TagFunctionRegCode1 = "TagFunctionR1";
        private const string TagFunctionRegCode2 = "TagFunctionR2";
        private const string OldTagFunctionDescription1 = "OldTagFunction1Description";
        private const string OldTagFunctionDescription2 = "OldTagFunction2Description";
        private const string NewTagFunctionDescription1 = "NewTagFunction1Description";
        private const string NewTagFunctionDescription2 = "NewTagFunction2Description";

        private Mock<ITagFunctionRepository> _tagFunctionRepositoryMock;
        private Mock<ITagFunctionApiService> _tagFunctionApiServiceMock;

        private SyncTagFunctionsCommand _command;
        private SyncTagFunctionsCommandHandler _dut;
        private TagFunction _tagFunction1;
        private TagFunction _tagFunction2;
        private ProcosysTagFunction _mainTagFunction1;
        private ProcosysTagFunction _mainTagFunction2;

        [TestInitialize]
        public void Setup()
        {
            // Assert tagFunctions in preservation
            _tagFunction1 = new TagFunction(TestPlant, TagFunctionCode1, OldTagFunctionDescription1, TagFunctionRegCode1);
            _tagFunction2 = new TagFunction(TestPlant, TagFunctionCode2, OldTagFunctionDescription2, TagFunctionRegCode2);

            _tagFunctionRepositoryMock = new Mock<ITagFunctionRepository>();
            _tagFunctionRepositoryMock
                .Setup(p => p.GetAllAsync())
                .Returns(Task.FromResult(new List<TagFunction> {_tagFunction1, _tagFunction2}));

            // Assert tagFunctions in main
            _mainTagFunction1 = new ProcosysTagFunction
            {
                Code = TagFunctionCode1,
                Description = NewTagFunctionDescription1,
                RegisterCode = TagFunctionRegCode1
            };
            _mainTagFunction2 = new ProcosysTagFunction
            {
                Code = TagFunctionCode2,
                Description = NewTagFunctionDescription2,
                RegisterCode = TagFunctionRegCode2
            };

            _tagFunctionApiServiceMock = new Mock<ITagFunctionApiService>();
            _tagFunctionApiServiceMock
                .Setup(x => x.TryGetTagFunctionAsync(TestPlant, TagFunctionCode1, TagFunctionRegCode1))
                .Returns(Task.FromResult(_mainTagFunction1));
            _tagFunctionApiServiceMock
                .Setup(x => x.TryGetTagFunctionAsync(TestPlant, TagFunctionCode2, TagFunctionRegCode2))
                .Returns(Task.FromResult(_mainTagFunction2));


            _command = new SyncTagFunctionsCommand();

            _dut = new SyncTagFunctionsCommandHandler(
                _tagFunctionRepositoryMock.Object,
                UnitOfWorkMock.Object,
                PlantProviderMock.Object,
                _tagFunctionApiServiceMock.Object);
        }

        [TestMethod]
        public async Task HandlingTagFunctionsCommand_ShouldUpdateTagFunctions()
        {
            // Act
            var result = await _dut.Handle(_command, default);

            // Assert
            Assert.AreEqual(0, result.Errors.Count);
            AssertTagFunctionProperties(_mainTagFunction1, _tagFunction1);
            AssertTagFunctionProperties(_mainTagFunction2, _tagFunction2);
        }

        [TestMethod]
        public async Task HandlingTagFunctionsCommand_ShouldSave()
        {
            // Act
            await _dut.Handle(_command, default);
            
            // Assert
            UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        private void AssertTagFunctionProperties(ProcosysTagFunction mainTagFunction, TagFunction tagFunction)
            => Assert.AreEqual(mainTagFunction.Description, tagFunction.Description);
    }
}

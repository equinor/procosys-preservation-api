using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Exceptions;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Equinor.Procosys.Preservation.MainApi.Tag;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.MainApi.Tests.Tag
{
    [TestClass]
    public class MainApiTagServiceTests
    {
        private Mock<IMainApiClient> _mainApiClient;
        private Mock<IPlantApiService> _plantApiService;
        private ProcosysTagSearchResult _searchResultWithOneItem;
        private ProcosysTagSearchResult _searchResultWithThreeItems;
        private ProcosysTagDetailsResult _tagDetailsResult;

        [TestInitialize]
        public void Setup()
        {
            _mainApiClient = new Mock<IMainApiClient>();
            _plantApiService = new Mock<IPlantApiService>();
            _plantApiService
                .Setup(x => x.IsPlantValidAsync("PCS$TESTPLANT"))
                .Returns(Task.FromResult(true));

            _searchResultWithOneItem = new ProcosysTagSearchResult
            {
                Items = new List<ProcosysTagOverview>
                        {
                            new ProcosysTagOverview
                            {
                                Description = "Description1",
                                Id = 111111111,
                                TagNo = "TagNo1"
                            }
                        },
                MaxAvailable = 1
            };

            _searchResultWithThreeItems = new ProcosysTagSearchResult
                {
                    Items = new List<ProcosysTagOverview>
                        {
                            new ProcosysTagOverview
                            {
                                Description = "Description1",
                                Id = 111111111,
                                TagNo = "TagNo1"
                            },
                            new ProcosysTagOverview
                            {
                                Description = "Description2",
                                Id = 222222222,
                                TagNo = "TagNo2"
                            },
                            new ProcosysTagOverview
                            {
                                Description = "Description3",
                                Id = 333333333,
                                TagNo = "TagNo3"
                            },
                        },
                    MaxAvailable = 1
                };

            _tagDetailsResult = new ProcosysTagDetailsResult
            {
                Tag = new ProcosysTagDetails
                {
                    AreaCode = "AreaCode",
                    CallOffNo = "CallOffNo",
                    CommPkgNo = "CommPkgNo",
                    Description = "Description1",
                    DisciplineCode = "DisciplineCode",
                    McPkgNo = "McPkgNo",
                    PurchaseOrderNo = "PurchaseOrderNo",
                    TagFunctionCode = "TagFunctionCode",
                    TagNo = "TagNo1"
                }
            };
        }

        [TestMethod]
        public async Task GetTagsReturnsCorrectNumberOfTagsTestAsync()
        {
            // Arrange
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult(_searchResultWithThreeItems));
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object);

            // Act
            var result = await dut.GetTags("PCS$TESTPLANT", "TestProject", "A");

            // Assert
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public async Task GetTagsThrowsExceptionIfPlantIsInvalidTestAsync()
        {
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dut.GetTags("INVALIDPLANT", "TestProject", "A"));
        }

        [TestMethod]
        public async Task GetTagsThrowsExceptionIfResultIsInvalidTestAsync()
        {
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult<ProcosysTagSearchResult>(null));
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object);

            await Assert.ThrowsExceptionAsync<InvalidResultException>(async () => await dut.GetTags("PCS$TESTPLANT", "TestProject", "A"));
        }

        [TestMethod]
        public async Task GetTagsReturnsCorrectPropertiesTestAsync()
        {
            // Arrange
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult(_searchResultWithThreeItems));
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object);

            // Act
            var result = await dut.GetTags("PCS$TESTPLANT", "TestProject", "TagNo");

            // Assert
            var tag = result.First();
            Assert.AreEqual("Description1", tag.Description);
            Assert.AreEqual(111111111, tag.Id);
            Assert.AreEqual("TagNo1", tag.TagNo);
        }

        [TestMethod]
        public async Task GetTagDetailsSetsCorrectPropertiesTestAsync()
        {
            // Arrange
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult(_searchResultWithOneItem));
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<ProcosysTagDetailsResult>(It.IsAny<string>()))
                .Returns(Task.FromResult(_tagDetailsResult));
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object);

            // Act
            var result = await dut.GetTagDetails("PCS$TESTPLANT", "TestProject", "111111111");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("AreaCode", result.AreaCode);
            Assert.AreEqual("CallOffNo", result.CallOffNo);
            Assert.AreEqual("CommPkgNo", result.CommPkgNo);
            Assert.AreEqual("Description1", result.Description);
            Assert.AreEqual("DisciplineCode", result.DisciplineCode);
            Assert.AreEqual("McPkgNo", result.McPkgNo);
            Assert.AreEqual("PurchaseOrderNo", result.PurchaseOrderNo);
            Assert.AreEqual("TagFunctionCode", result.TagFunctionCode);
            Assert.AreEqual("TagNo1", result.TagNo);
        }

        [TestMethod]
        public async Task GetTagDetailsThrowsExceptionIfPlantIsInvalidTestAsync()
        {
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dut.GetTagDetails("INVALIDPLANT", "TestProject", "TagNo1"));
        }

        [TestMethod]
        public async Task GetTagDetailsThrowsExceptionIfResultContainsMultipleElementsTestAsync()
        {
            // Arrange
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult(_searchResultWithThreeItems));
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object);

            await Assert.ThrowsExceptionAsync<InvalidResultException>(async () => await dut.GetTagDetails("PCS$TESTPLANT", "TestProject", "TagNo1"));
        }

        [TestMethod]
        public async Task GetTagDetailsThrowsExceptionIfResultIsNullTestAsync()
        {
            // Arrange
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult(_searchResultWithOneItem));
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<ProcosysTagDetailsResult>(It.IsAny<string>()))
                .Returns(Task.FromResult<ProcosysTagDetailsResult>(null));
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object);

            await Assert.ThrowsExceptionAsync<InvalidResultException>(async () => await dut.GetTagDetails("PCS$TESTPLANT", "TestProject", "TagNo1"));
        }
    }
}

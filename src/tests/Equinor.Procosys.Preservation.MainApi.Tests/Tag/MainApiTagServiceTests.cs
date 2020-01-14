using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
using Equinor.Procosys.Preservation.MainApi.Exceptions;
using Equinor.Procosys.Preservation.MainApi.Plant;
using Equinor.Procosys.Preservation.MainApi.Tag;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.MainApi.Tests.Tag
{
    [TestClass]
    public class MainApiTagServiceTests
    {
        private Mock<ILogger<MainApiTagService>> _logger;
        private Mock<IOptionsMonitor<MainApiOptions>> _mainApiOptions;
        private Mock<IMainApiClient> _mainApiClient;
        private Mock<IPlantApiService> _plantApiService;
        private ProcosysTagSearchResult _searchResultWithOneItem;
        private ProcosysTagSearchResult _searchResultWithThreeItems;
        private ProcosysTagDetailsResult _tagDetailsResult;

        [TestInitialize]
        public void Setup()
        {
            _logger = new Mock<ILogger<MainApiTagService>>();
            _mainApiOptions = new Mock<IOptionsMonitor<MainApiOptions>>();
            _mainApiOptions
                .Setup(x => x.CurrentValue)
                .Returns(new MainApiOptions { ApiVersion = "4.0", BaseUrl = "http://example.com" });
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
        public async Task GetTags_ReturnsCorrectNumberOfTags_TestAsync()
        {
            // Arrange
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult(_searchResultWithThreeItems));
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

            // Act
            var result = await dut.GetTags("PCS$TESTPLANT", "TestProject", "A");

            // Assert
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public async Task GetTags_ThrowsException_WhenPlantIsInvalid_TestAsync()
        {
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dut.GetTags("INVALIDPLANT", "TestProject", "A"));
        }

        [TestMethod]
        public async Task GetTags_ReturnsNull_WhenResultIsInvalid_TestAsync()
        {
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult<ProcosysTagSearchResult>(null));
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

            var result = await dut.GetTags("PCS$TESTPLANT", "TestProject", "A");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetTags_ReturnsCorrectProperties_TestAsync()
        {
            // Arrange
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult(_searchResultWithThreeItems));
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

            // Act
            var result = await dut.GetTags("PCS$TESTPLANT", "TestProject", "TagNo");

            // Assert
            var tag = result.First();
            Assert.AreEqual("Description1", tag.Description);
            Assert.AreEqual(111111111, tag.Id);
            Assert.AreEqual("TagNo1", tag.TagNo);
        }

        [TestMethod]
        public async Task GetTagDetails_SetsCorrectProperties_TestAsync()
        {
            // Arrange
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult(_searchResultWithOneItem));
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<ProcosysTagDetailsResult>(It.IsAny<string>()))
                .Returns(Task.FromResult(_tagDetailsResult));
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

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
        public async Task GetTagDetails_ThrowsException_IfPlantIsInvalid_TestAsync()
        {
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dut.GetTagDetails("INVALIDPLANT", "TestProject", "TagNo1"));
        }

        [TestMethod]
        public async Task GetTagDetails_ThrowsException_WhenResultContainsMultipleElements_TestAsync()
        {
            // Arrange
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult(_searchResultWithThreeItems));
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

            await Assert.ThrowsExceptionAsync<InvalidResultException>(async () => await dut.GetTagDetails("PCS$TESTPLANT", "TestProject", "TagNo1"));
        }

        [TestMethod]
        public async Task GetTagDetails_ReturnsNull_WhenResultIsNull_TestAsync()
        {
            // Arrange
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult(_searchResultWithOneItem));
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<ProcosysTagDetailsResult>(It.IsAny<string>()))
                .Returns(Task.FromResult<ProcosysTagDetailsResult>(null));
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

            var result = await dut.GetTagDetails("PCS$TESTPLANT", "TestProject", "TagNo1");

            Assert.IsNull(result);
        }
    }
}

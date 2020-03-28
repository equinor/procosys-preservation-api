using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Preservation.MainApi.Client;
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
        private Mock<IBearerTokenApiClient> _mainApiClient;
        private Mock<IPlantApiService> _plantApiService;
        private ProcosysTagSearchResult _searchResultWithOneItem;
        private ProcosysTagSearchResult _searchResultWithThreeItems;
        private List<ProcosysTagDetails> _tagDetails;

        [TestInitialize]
        public void Setup()
        {
            _logger = new Mock<ILogger<MainApiTagService>>();
            _mainApiOptions = new Mock<IOptionsMonitor<MainApiOptions>>();
            _mainApiOptions
                .Setup(x => x.CurrentValue)
                .Returns(new MainApiOptions { ApiVersion = "4.0", BaseAddress = "http://example.com", TagSearchPageSize = 2 });
            _mainApiClient = new Mock<IBearerTokenApiClient>();
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

            _tagDetails = new List<ProcosysTagDetails>
            {
                new ProcosysTagDetails
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
        public async Task GetTags_GetsAllPagesAndReturnsCorrectNumberOfTags_TestAsync()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.QueryAndDeserialize<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult(_searchResultWithThreeItems))
                .Returns(Task.FromResult(_searchResultWithOneItem))
                .Returns(Task.FromResult<ProcosysTagSearchResult>(null));
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

            // Act
            var result = await dut.SearchTagsByTagNoAsync("PCS$TESTPLANT", "TestProject", "A");

            // Assert
            Assert.AreEqual(4, result.Count());
        }

        [TestMethod]
        public async Task GetTags_ThrowsException_WhenPlantIsInvalid_TestAsync()
        {
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dut.SearchTagsByTagNoAsync("INVALIDPLANT", "TestProject", "A"));
        }

        [TestMethod]
        public async Task GetTags_ReturnsEmptyList_WhenResultIsInvalid_TestAsync()
        {
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult<ProcosysTagSearchResult>(null));
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

            var result = await dut.SearchTagsByTagNoAsync("PCS$TESTPLANT", "TestProject", "A");

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetTags_ReturnsCorrectProperties_TestAsync()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.QueryAndDeserialize<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult(_searchResultWithThreeItems))
                .Returns(Task.FromResult<ProcosysTagSearchResult>(null));
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

            // Act
            var result = await dut.SearchTagsByTagNoAsync("PCS$TESTPLANT", "TestProject", "TagNo");

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
                .SetupSequence(x => x.QueryAndDeserialize<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult(_searchResultWithOneItem))
                .Returns(Task.FromResult<ProcosysTagSearchResult>(null));
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<List<ProcosysTagDetails>>(It.IsAny<string>()))
                .Returns(Task.FromResult(_tagDetails));
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

            // Act
            var result = await dut.GetTagDetailsAsync("PCS$TESTPLANT", "TestProject", new List<string>{"111111111"});

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            var tag = result.First();
            Assert.AreEqual("AreaCode", tag.AreaCode);
            Assert.AreEqual("CallOffNo", tag.CallOffNo);
            Assert.AreEqual("CommPkgNo", tag.CommPkgNo);
            Assert.AreEqual("Description1", tag.Description);
            Assert.AreEqual("DisciplineCode", tag.DisciplineCode);
            Assert.AreEqual("McPkgNo", tag.McPkgNo);
            Assert.AreEqual("PurchaseOrderNo", tag.PurchaseOrderNo);
            Assert.AreEqual("TagFunctionCode", tag.TagFunctionCode);
            Assert.AreEqual("TagNo1", tag.TagNo);
        }

        [TestMethod]
        public async Task GetTagDetails_ThrowsException_IfPlantIsInvalid_TestAsync()
        {
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dut.GetTagDetailsAsync("INVALIDPLANT", "TestProject", new List<string>{"TagNo1"}));
        }

        [TestMethod]
        public async Task GetTagDetails_ReturnsNull_WhenResultIsNull_TestAsync()
        {
            // Arrange
            _mainApiClient
                .SetupSequence(x => x.QueryAndDeserialize<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult(_searchResultWithOneItem))
                .Returns(Task.FromResult<ProcosysTagSearchResult>(null));
            _mainApiClient
                .Setup(x => x.QueryAndDeserialize<List<ProcosysTagDetails>>(It.IsAny<string>()))
                .Returns(Task.FromResult<List<ProcosysTagDetails>>(null));
            var dut = new MainApiTagService(_mainApiClient.Object, _plantApiService.Object, _mainApiOptions.Object, _logger.Object);

            var result = await dut.GetTagDetailsAsync("PCS$TESTPLANT", "TestProject", new List<string>{"TagNo1"});

            Assert.IsNull(result);
        }
    }
}

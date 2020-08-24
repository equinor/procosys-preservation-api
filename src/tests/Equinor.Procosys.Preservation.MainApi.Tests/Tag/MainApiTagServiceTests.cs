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
        private Mock<IPlantCache> _plantCache;
        private ProcosysTagSearchResult _searchPage1WithThreeItems;
        private ProcosysTagSearchResult _searchPage2WithOneItem;
        private List<ProcosysTagDetails> _tagDetails;
        private MainApiTagService _dut;

        [TestInitialize]
        public void Setup()
        {
            _logger = new Mock<ILogger<MainApiTagService>>();
            _mainApiOptions = new Mock<IOptionsMonitor<MainApiOptions>>();
            _mainApiOptions
                .Setup(x => x.CurrentValue)
                .Returns(new MainApiOptions { ApiVersion = "4.0", BaseAddress = "http://example.com", TagSearchPageSize = 3 });
            _mainApiClient = new Mock<IBearerTokenApiClient>();
            _plantCache = new Mock<IPlantCache>();
            _plantCache
                .Setup(x => x.IsValidPlantForCurrentUserAsync("PCS$TESTPLANT"))
                .Returns(Task.FromResult(true));

            _searchPage2WithOneItem = new ProcosysTagSearchResult
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
                MaxAvailable = 4
            };

            _searchPage1WithThreeItems = new ProcosysTagSearchResult
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
                    MaxAvailable = 4
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
                    RegisterCode = "RegisterCode",
                    TagNo = "TagNo1"
                }
            };

            _mainApiClient
                .SetupSequence(x => x.QueryAndDeserializeAsync<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult(_searchPage1WithThreeItems))
                .Returns(Task.FromResult(_searchPage2WithOneItem));

            _dut = new MainApiTagService(_mainApiClient.Object, _plantCache.Object, _mainApiOptions.Object, _logger.Object);
        }

        [TestMethod]
        public async Task SearchTagsByTagNo_ShouldGetAllPagesAndReturnsCorrectNumberOfTags()
        {
            // Act
            var result = await _dut.SearchTagsByTagNoAsync("PCS$TESTPLANT", "TestProject", "A");

            // Assert
            Assert.AreEqual(4, result.Count);
        }

        [TestMethod]
        public async Task SearchTagsByTagNo_ShouldThrowsException_WhenPlantIsInvalid()
            => await Assert.ThrowsExceptionAsync<ArgumentException>(async ()
                => await _dut.SearchTagsByTagNoAsync("INVALIDPLANT", "TestProject", "A"));

        [TestMethod]
        public async Task SearchTagsByTagNo_ShouldReturnEmptyList_WhenResultIsInvalid()
        {
            _mainApiClient
                .Setup(x => x.QueryAndDeserializeAsync<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult<ProcosysTagSearchResult>(null));

            var result = await _dut.SearchTagsByTagNoAsync("PCS$TESTPLANT", "TestProject", "A");

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task SearchTagsByTagNo_ShouldReturnCorrectProperties()
        {
            // Act
            var result = await _dut.SearchTagsByTagNoAsync("PCS$TESTPLANT", "TestProject", "TagNo");

            // Assert
            var tag = result.First();
            Assert.AreEqual("Description1", tag.Description);
            Assert.AreEqual(111111111, tag.Id);
            Assert.AreEqual("TagNo1", tag.TagNo);
        }

        [TestMethod]
        public async Task SearchTagsByTagFunctions_ShouldGetAllPagesAndReturnsCorrectNumberOfTags()
        {
            // Act
            var result = await _dut.SearchTagsByTagFunctionsAsync("PCS$TESTPLANT", "TestProject", new List<string>{"M"});

            // Assert
            Assert.AreEqual(4, result.Count);
        }

        [TestMethod]
        public async Task SearchTagsByTagFunctions_ShouldThrowsException_WhenPlantIsInvalid()
            => await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _dut.SearchTagsByTagFunctionsAsync("INVALIDPLANT", "TestProject", new List<string>{"M"}));

        [TestMethod]
        public async Task SearchTagsByTagFunctions_ShouldReturnEmptyList_WhenResultIsInvalid()
        {
            _mainApiClient
                .Setup(x => x.QueryAndDeserializeAsync<ProcosysTagSearchResult>(It.IsAny<string>()))
                .Returns(Task.FromResult<ProcosysTagSearchResult>(null));

            var result = await _dut.SearchTagsByTagFunctionsAsync("PCS$TESTPLANT", "TestProject", new List<string>{"M"});

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task SearchTagsByTagFunctions_ShouldReturnCorrectProperties()
        {
            // Act
            var result = await _dut.SearchTagsByTagFunctionsAsync("PCS$TESTPLANT", "TestProject", new List<string>{"M"});

            // Assert
            var tag = result.First();
            Assert.AreEqual("Description1", tag.Description);
            Assert.AreEqual(111111111, tag.Id);
            Assert.AreEqual("TagNo1", tag.TagNo);
        }

        [TestMethod]
        public async Task GetTagDetails_ThrowsException_IfPlantIsInvalid()
            => await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
                await _dut.GetTagDetailsAsync("INVALIDPLANT", "TestProject", new List<string> {"TagNo1"}));

        [TestMethod]
        public async Task GetTagDetails_ShouldReturnEmptyList_WhenResultIsEmptyList()
        {
            // Arrange
            _mainApiClient
                .Setup(x => x.QueryAndDeserializeAsync<List<ProcosysTagDetails>>(It.IsAny<string>()))
                .Returns(Task.FromResult(new List<ProcosysTagDetails>()));

            // Act
            var result = await _dut.GetTagDetailsAsync("PCS$TESTPLANT", "TestProject", new List<string>{"TagNo1"});

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetTagDetails_SetsCorrectProperties()
        {
            // Arrange
            _mainApiClient
                .Setup(x => x.QueryAndDeserializeAsync<List<ProcosysTagDetails>>(It.IsAny<string>()))
                .Returns(Task.FromResult(_tagDetails));

            // Act
            var result = await _dut.GetTagDetailsAsync("PCS$TESTPLANT", "TestProject", new List<string>{"111111111"});

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            var tag = result.First();
            var details = _tagDetails.First();
            Assert.AreEqual(details.AreaCode, tag.AreaCode);
            Assert.AreEqual(details.CallOffNo, tag.CallOffNo);
            Assert.AreEqual(details.CommPkgNo, tag.CommPkgNo);
            Assert.AreEqual(details.Description, tag.Description);
            Assert.AreEqual(details.DisciplineCode, tag.DisciplineCode);
            Assert.AreEqual(details.McPkgNo, tag.McPkgNo);
            Assert.AreEqual(details.PurchaseOrderNo, tag.PurchaseOrderNo);
            Assert.AreEqual(details.TagFunctionCode, tag.TagFunctionCode);
            Assert.AreEqual(details.RegisterCode, tag.RegisterCode);
            Assert.AreEqual(details.TagNo, tag.TagNo);
        }

        [TestMethod]
        public async Task GetTagDetails_ShouldCallMainApiClientManyTimes_WhenManyTagNos()
        {
            // Arrange
            _mainApiClient
                .Setup(x => x.QueryAndDeserializeAsync<List<ProcosysTagDetails>>(It.IsAny<string>()))
                .Returns(Task.FromResult(_tagDetails));
            var allTagNos = new List<string>();
            for (var i = 0; i < 60; i++)
            {
                allTagNos.Add($"Tag{i}");
            }

            // Act
            await _dut.GetTagDetailsAsync("PCS$TESTPLANT", "TestProject", allTagNos);

            // Assert
            _mainApiClient.Verify(x => x.QueryAndDeserializeAsync<List<ProcosysTagDetails>>(It.IsAny<string>()),
                Times.Exactly(2));
        }
    }
}
